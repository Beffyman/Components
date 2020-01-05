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

		private readonly Dictionary<Type, ConcurrentDictionary<Entity, IComponent>> _components;

		private readonly ComponentSystemBase[] _componentSystems;
		private readonly Dictionary<Type, ComponentSystemBase> _indexedComponentSystems;

		private readonly object _archeTypeLock = new object();
		private readonly ConcurrentDictionary<int, ArcheType> _archeTypes;
		private readonly Dictionary<ArcheType, ConcurrentDictionary<Entity, Dictionary<Type, IComponent>>> _archeTypeEntityMap;
		//private readonly Dictionary<ArcheType, ConcurrentHashSet<Entity>> _archeTypeEntities;
		private readonly ConcurrentDictionary<Entity, ArcheType> _entityArcheTypeMap;

		public EntityManagerOptions Options { get; }

		private bool _firstUpdate = true;

		public EntityManager(EntityManagerOptions options = null)
		{
			Options = options ?? new EntityManagerOptions();

			_entityPool = InfiniteScaleObjectPool<Entity>.Create(Options.InitialPoolSize);

			_entities = new ConcurrentHashSet<Entity>(Environment.ProcessorCount, (int)Options.InitialPoolSize, EntityEqualityComparer.Instance);

			_components = new Dictionary<Type, ConcurrentDictionary<Entity, IComponent>>(TypeEqualityComparer.Instance);
			_componentSystems = LoadSystemComponents();
			_indexedComponentSystems = _componentSystems.ToDictionary(x => x.GetType(), x => x, TypeEqualityComparer.Instance);

			_archeTypes = new ConcurrentDictionary<int, ArcheType>();
			_archeTypeEntityMap = new Dictionary<ArcheType, ConcurrentDictionary<Entity, Dictionary<Type, IComponent>>>(ArcheTypeEqualityComparer.Instance);
			//_archeTypeEntities = new Dictionary<ArcheType, ConcurrentHashSet<Entity>>(ArcheTypeEqualityComparer.Instance);
			_entityArcheTypeMap = new ConcurrentDictionary<Entity, ArcheType>(EntityEqualityComparer.Instance);

			LoadArcheTypes();
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
