using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Beffyman.Components.Internal;
using Beffyman.Components.Systems;
using ConcurrentCollections;

namespace Beffyman.Components.Manager
{
	public partial class EntityManager
	{
		private readonly InfiniteScaleObjectPool<Entity> _entityPool;


		internal ConcurrentHashSet<Entity> _entities { get; }
		public IReadOnlyCollection<Entity> Entities => _entities;

		private readonly ConcurrentDictionary<Type, ConcurrentDictionary<Entity, IComponent>> _components;

		private readonly ComponentSystemBase[] _componentSystems;
		private readonly Dictionary<Type, ComponentSystemBase> _indexedComponentSystems;

		private readonly ConcurrentSet<ArcheType> _archTypes;
		private readonly ConcurrentDictionary<ArcheType, ConcurrentHashSet<Entity>> _archeTypeMap;
		private readonly ConcurrentDictionary<Entity, ArcheType> _entityArcheTypes;

		public EntityManagerOptions Options { get; }

		private bool _firstUpdate = true;

		public EntityManager(EntityManagerOptions options = null)
		{
			Options = options ?? new EntityManagerOptions();

			_entityPool = InfiniteScaleObjectPool<Entity>.Create(Options.InitialPoolSize);

			_entities = new ConcurrentHashSet<Entity>(Environment.ProcessorCount, (int)Options.InitialPoolSize, EntityEqualityComparer.Instance);

			_components = new ConcurrentDictionary<Type, ConcurrentDictionary<Entity, IComponent>>(Environment.ProcessorCount, (int)Options.InitialPoolSize, TypeEqualityComparer.Instance);
			_componentSystems = LoadSystemComponents();
			_indexedComponentSystems = _componentSystems.ToDictionary(x => x.GetType(), x => x, TypeEqualityComparer.Instance);

			_archTypes = new ConcurrentSet<ArcheType>(Environment.ProcessorCount, (int)Options.InitialPoolSize, ArcheTypeEqualityComparer.Instance);
			_archeTypeMap = new ConcurrentDictionary<ArcheType, ConcurrentHashSet<Entity>>(Environment.ProcessorCount, (int)Options.InitialPoolSize, ArcheTypeEqualityComparer.Instance);
			_entityArcheTypes = new ConcurrentDictionary<Entity, ArcheType>(Environment.ProcessorCount, (int)Options.InitialPoolSize, EntityEqualityComparer.Instance);

			//Assign empty archetype to collections
			_archTypes.Add(ArcheType.Empty);
			_archeTypeMap.TryAdd(ArcheType.Empty, CreateArcheTypeEntitySet());
		}

		public void Update(in UpdateStep step)
		{
			UpdateSystems(step);
		}

		public void FixedUpdate(in UpdateStep step)
		{
			FixedUpdateSystems(step);
		}

		public void Destroy()
		{
			DestroySystems();
			DestroyComponents();
			DestroyEntities();
		}
	}
}
