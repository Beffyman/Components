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
#warning Can probably utilize ArrayPool somewhere...

		private readonly InfiniteScaleObjectPool<Entity> _entityPool;
		private readonly ConcurrentHashSet<Entity> _entities;

		private readonly ConcurrentDictionary<Type, ConcurrentDictionary<Entity, IComponent>> _components;

		private readonly ComponentSystemBase[] _componentSystems;
		private readonly Dictionary<Type, ComponentSystemBase> _indexedComponentSystems;

		private readonly ConcurrentHashSet<ArcheType> _archTypes;
		private readonly ConcurrentDictionary<Entity, ArcheType> _entityArcheTypes;

		public readonly EntityManagerOptions Options;

		private bool _firstUpdate = true;

		public EntityManager(EntityManagerOptions options = null)
		{
			Options = options ?? new EntityManagerOptions();

			_entityPool = InfiniteScaleObjectPool<Entity>.Create(Options.InitialPoolSize);

			_entities = new ConcurrentHashSet<Entity>(Environment.ProcessorCount, (int)Options.InitialPoolSize, EntityEqualityComparer.Instance);

			_components = new ConcurrentDictionary<Type, ConcurrentDictionary<Entity, IComponent>>(Environment.ProcessorCount, (int)Options.InitialPoolSize, TypeEqualityComparer.Instance);
			_componentSystems = LoadSystemComponents();
			_indexedComponentSystems = _componentSystems.ToDictionary(x => x.GetType(), x => x, TypeEqualityComparer.Instance);

			_archTypes = new ConcurrentHashSet<ArcheType>(Environment.ProcessorCount, (int)Options.InitialPoolSize, ArcheTypeEqualityComparer.Instance);
			_entityArcheTypes = new ConcurrentDictionary<Entity, ArcheType>(Environment.ProcessorCount, (int)Options.InitialPoolSize, EntityEqualityComparer.Instance);
		}

		public void Update(in UpdateStep step)
		{
			UpdateSystems(step);
		}

		public void Destroy()
		{
			DestroySystems();
			DestroyComponents();
			DestroyEntities();
		}
	}
}
