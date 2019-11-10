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
						_archeTypeMap[currentArcheType].TryRemove(entity);
					}

					if (archeType.ComponentTypes.Length > 0)
					{
						_entityArcheTypes[entity] = archeType;

						var entities = _archeTypeMap.GetOrAdd(archeType, (arch) => new ConcurrentHashSet<Entity>(Environment.ProcessorCount, (int)Options.InitialPoolSize, EntityEqualityComparer.Instance));

						entities.Add(entity);
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
					entities.Add(entity);
				}
			}
		}

		private ConcurrentHashSet<Entity> CreateArcheTypeEntitySet()
		{
			return new ConcurrentHashSet<Entity>(Environment.ProcessorCount, (int)Options.InitialPoolSize, EntityEqualityComparer.Instance);
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

		internal ConcurrentHashSet<Entity> GetEntities(ArcheType archeType)
		{
			if (_archeTypeMap.TryGetValue(archeType, out ConcurrentHashSet<Entity> entities))
			{
				return entities;
			}

			return null;
		}

		internal ConcurrentHashSet<Entity> GetEntities(Type[] components)
		{
			var archeType = GetArcheType(components);

			if (_archeTypeMap.TryGetValue(archeType, out ConcurrentHashSet<Entity> entities))
			{
				return entities;
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
