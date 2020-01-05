using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Beffyman.Components.Internal;
using Beffyman.Components.Systems;
using ConcurrentCollections;

namespace Beffyman.Components.Manager
{
	public partial class EntityManager
	{
		private void LoadArcheTypes()
		{
			//First we want to fetch all Components defined in the assemblies/types loaded
			Type[] allComponentTypes = null;
			IEnumerable<Assembly> assemblies = Options.ComponentSystemAssemblies;
			if (assemblies == null && Options.ComponentSystemTypes == null)
			{
				assemblies = new Assembly[] { Assembly.GetEntryAssembly(), Assembly.GetExecutingAssembly(), Assembly.GetCallingAssembly() };
			}

			if (assemblies != null)
			{
				assemblies = assemblies.Distinct().Where(x => x != typeof(EntityManager).Assembly).ToArray();

				allComponentTypes = assemblies.SelectMany(x => x.DefinedTypes)
					.Where(x => typeof(IComponent).IsAssignableFrom(x))
					.Cast<Type>()
					.ToArray();
			}
			else
			{
				allComponentTypes = Options.ComponentSystemTypes.Distinct().ToArray();
			}

			//Map the dictionary with the components
			foreach (var componentType in allComponentTypes)
			{
				_components.Add(componentType, new ConcurrentDictionary<Entity, IComponent>(EntityEqualityComparer.Instance));
			}


			//Then we are going to find all initial ArcheTypes to have that initially loaded to avoid any system contention on start
			var systemJobParamTypes = JobComponentSystem.GetJobParameterTypes(_componentSystems.Select(x => x.GetType()));


			foreach (var types in systemJobParamTypes)
			{
				CreateArcheType(types);
			}
		}

		public ArcheType CreateArcheType(Type[] types)
		{
			var index = ArcheTypeEqualityComparer.GetHashCode(types);

			ArcheType archeType;

			if (_archeTypes.TryGetValue(index, out archeType))
			{
				return archeType;
			}

			lock (_archeTypeLock)
			{
				//Check again in case it was waiting for the lock
				if (_archeTypes.TryGetValue(index, out archeType))
				{
					return archeType;
				}

				archeType = new ArcheType(types);

				//Allocation is fine as this shouldn't be called often
				//We loop through all existing archetypes and assign this new one into them if it is related
				//We also remap the global collections if a relation is found, this requires an O(n) loop where n are all the types included in the archetype
				bool reassignEntities = false;

				foreach (var kv in _archeTypes)
				{
					if (archeType._componentTypes.IsSupersetOf(kv.Value._componentTypes))
					{
						archeType.AddChild(kv.Value);
						reassignEntities = true;
					}
					else if (archeType._componentTypes.IsSubsetOf(kv.Value._componentTypes))
					{
						archeType.AddParent(kv.Value);
						reassignEntities = true;
					}
				}

				var archeTypeEntityMap = new ConcurrentDictionary<Entity, Dictionary<Type, IComponent>>(EntityEqualityComparer.Instance);

				_archeTypeEntityMap.Add(archeType, archeTypeEntityMap);
				//_archeTypeEntities.Add(archeType, new ConcurrentHashSet<Entity>(EntityEqualityComparer.Instance));

				//If we made any relation changes to the archetypes we need to reassign the entities
				if (reassignEntities)
				{
					for (int i = 0; i < types.Length; i++)
					{
						var type = types[i];

						if (_components.TryGetValue(type, out ConcurrentDictionary<Entity, IComponent> entityComponent))
						{
							foreach (var entityComponentMap in entityComponent)
							{
								var typeComponent = archeTypeEntityMap.GetOrAdd(entityComponentMap.Key, (e) => new Dictionary<Type, IComponent>(TypeEqualityComparer.Instance));

								typeComponent.Add(type, entityComponentMap.Value);
							}
						}
					}
				}


				if (_archeTypes.TryAdd(index, archeType))
				{
					return archeType;
				}
				else
				{
					throw new InvalidOperationException("Seems like a race condition among the archeType creation?");
				}
			}
		}

		/// <summary>
		/// Called after component operations to re-map entity to archetype lookups
		/// </summary>
		/// <param name="entity"></param>
		private void RemapEntityArcheType(Entity entity)
		{
			//Get the "new" components
			var types = GetComponentTypes(entity);

			//Get the "new" archetype
			var archeTypeIndex = ArcheTypeEqualityComparer.GetHashCode(types);

			ArcheType newArcheType;

			//We just made a new archetype, gotta remap!
			if (!_archeTypes.TryGetValue(archeTypeIndex, out newArcheType))
			{
				//Creating a new archetype handles the remapping of the components, no need for second block, just return
				newArcheType = CreateArcheType(types);
				return;
			}

			lock (entity)
			{
				if (_entityArcheTypeMap.TryGetValue(entity, out ArcheType previousArchetype))
				{
					if (!previousArchetype.IsParentArcheTypeOf(newArcheType))
					{
						//Remove the entity from the old archeType as it isn't related to the new archetype
						_archeTypeEntityMap[previousArchetype].Remove(entity, out _);
					}
				}

				//Add all types from the new archetype into the map
				for (int i = 0; i < newArcheType._componentTypes.Count; i++)
				{
					var typeAdded = newArcheType._componentTypesArray[i];
					var component = _components[typeAdded][entity];

					var entityComponentMap = _archeTypeEntityMap[newArcheType].GetOrAdd(entity, (e) => new Dictionary<Type, IComponent>(TypeEqualityComparer.Instance));
					entityComponentMap.Add(typeAdded, component);
				}
			}
		}

		internal Dictionary<Type, IComponent> GetEntityComponentsByArcheType(Entity entity, ArcheType archeType)
		{
			if (_archeTypeEntityMap.TryGetValue(archeType, out ConcurrentDictionary<Entity, Dictionary<Type, IComponent>> entityComponents)
				&& entityComponents.TryGetValue(entity, out Dictionary<Type, IComponent> entityComponentMap))
			{
				return entityComponentMap;
			}

			return null;
		}

		internal ArcheType GetArcheType(Entity entity)
		{
			if (_entityArcheTypeMap.TryGetValue(entity, out ArcheType val))
			{
				return val;
			}
			else
			{
				throw new InvalidOperationException("Archetype mapping for entity should always exist");
			}
		}


		internal ArcheType GetArcheType(Type[] componentTypes)
		{
			int hash = ArcheTypeEqualityComparer.GetHashCode(componentTypes);

			if (_archeTypes.TryGetValue(hash, out ArcheType archeType))
			{
				return archeType;
			}
			else
			{
				return null;
			}
		}

		internal int GetArcheTypeEntityCount(ArcheType archeType)
		{
			if (_archeTypeEntityMap.TryGetValue(archeType, out ConcurrentDictionary<Entity, Dictionary<Type, IComponent>> entities))
			{
				return entities.Count;
			}
			else
			{
				return 0;
			}
		}

		internal void GetEntities(ArcheType archeType, ref Entity[] array)
		{
#error I don't think there is a good way to make this thread safe as well as not need any allocations
			archeType.GetAllChildren()

		}


		private void DestroyArcheTypes()
		{
			_archeTypes.Clear();

			foreach (var archeType in _archeTypeEntityMap)
			{
				foreach (var entities in archeType.Value)
				{
					entities.Value.Clear();
				}
				archeType.Value.Clear();
			}
			_archeTypeEntityMap.Clear();


			_entityArcheTypeMap.Clear();
		}
	}
}
