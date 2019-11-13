using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Beffyman.Components.Internal;
using ConcurrentCollections;

namespace Beffyman.Components.Manager
{
	public partial class EntityManager
	{
		internal void RemapEntityArcheType(Entity entity)
		{
			var archeType = GetArcheType(entity);

			//Was this entity added to the archetypes before?
			if (_entityArcheTypes.ContainsKey(entity))
			{
				var currentArcheType = _entityArcheTypes[entity];

				if (!ArcheTypeEqualityComparer.Instance.Equals(currentArcheType, archeType))
				{
					if (_archeTypeMap.ContainsKey(currentArcheType))
					{
						_archeTypeMap[currentArcheType].Remove(entity, out _);
					}

					if (archeType.ComponentTypes.Length > 0)
					{
						_entityArcheTypes[entity] = archeType;

						var entities = _archeTypeMap.GetOrAdd(archeType, (arch) => CreateArcheTypeEntitySet());

						Dictionary<Type, IComponent> entityComponents = entities.GetOrAdd(entity, () => CreateComponentTypeIndex());

						foreach (var component in archeType.ComponentTypes)
						{
							entityComponents.TryAdd(component, _components[component][entity]);
						}
					}
					else
					{
						_entityArcheTypes.TryRemove(entity, out ArcheType _);
					}
				}
			}
			//Entity is new to archetypes, just add it
			else
			{
				if (archeType.ComponentTypes.Length > 0)
				{
					_entityArcheTypes.TryAdd(entity, archeType);
					var entities = _archeTypeMap.GetOrAdd(archeType, (arch) => CreateArcheTypeEntitySet());

					Dictionary<Type, IComponent> entityComponents = null;

					if (!entities.TryGetValue(entity, out entityComponents))
					{
						entityComponents = CreateComponentTypeIndex();
						entities.Add(entity, entityComponents);
					}

					foreach (var component in archeType.ComponentTypes)
					{
						entityComponents.TryAdd(component, _components[component][entity]);
					}
				}
			}
		}

		private static Dictionary<Type, IComponent> CreateComponentTypeIndex()
		{
			return new Dictionary<Type, IComponent>(TypeEqualityComparer.Instance);
		}

		private Dictionary<Entity, Dictionary<Type, IComponent>> CreateArcheTypeEntitySet()
		{
			return new Dictionary<Entity, Dictionary<Type, IComponent>>((int)Options.InitialPoolSize, EntityEqualityComparer.Instance);
		}


		internal ArcheType GetArcheType(Entity entity)
		{
			if (_entityArcheTypes.TryGetValue(entity, out ArcheType at))
			{
				return at;
			}
			else
			{
				return CreateArcheType(entity);
			}
		}

		private ArcheType CreateArcheType(Entity entity)
		{
			var types = GetComponentTypes(entity);

			int hashCode = ArcheTypeEqualityComparer.GetHashCode(types);
#warning can probably remove allocation if customizing set to be only for this type
			var acheType = _archTypes.GetOrAddByHash(hashCode, (hash) => new ArcheType(types));

			_archeTypeMap.TryAdd(acheType, CreateArcheTypeEntitySet());

			return acheType;
		}

		internal ArcheType GetArcheType(Type[] components)
		{
			int hashCode = ArcheTypeEqualityComparer.GetHashCode(components);

			return _archTypes.GetByHash(hashCode);
		}

		internal int GetArcheTypeEntityCount(ArcheType archeType)
		{
			if (_archeTypeMap.TryGetValue(archeType, out Dictionary<Entity, Dictionary<Type, IComponent>> entities))
			{
				return entities.Count;
			}

			return default;
		}

		internal void GetEntities(ArcheType archeType, Entity[] mappedArray)
		{
			if (_archeTypeMap.TryGetValue(archeType, out Dictionary<Entity, Dictionary<Type, IComponent>> entities))
			{
				entities.Keys.CopyTo(mappedArray, 0);
			}
		}

		internal Dictionary<Type, IComponent> GetEntityComponentsByArcheType(ArcheType archeType, Entity entity)
		{
			if (_archeTypeMap.TryGetValue(archeType, out Dictionary<Entity, Dictionary<Type, IComponent>> entities))
			{
				if (entities.TryGetValue(entity, out Dictionary<Type, IComponent> components))
				{
					return components;
				}
			}

			return null;
		}

		private void DestroyArcheTypes()
		{
			_archeTypeMap.Clear();
			_entityArcheTypes.Clear();
		}
	}
}
