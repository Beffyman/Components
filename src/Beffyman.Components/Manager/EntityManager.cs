using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Beffyman.Components.Internal;
using ConcurrentCollections;

namespace Beffyman.Components.Manager
{
	public partial class EntityManager
	{
		private readonly InfiniteScaleObjectPool<Entity> _entityPool;
		private readonly ConcurrentHashSet<Entity> _entities;
		private readonly ConcurrentDictionary<Type, ConcurrentDictionary<Entity, IComponent>> _components;
		public readonly EntityManagerOptions Options;

		public EntityManager(EntityManagerOptions options = null)
		{
			Options = options ?? new EntityManagerOptions();
			_entityPool = InfiniteScaleObjectPool<Entity>.Create(Options.InitialPoolSize);
			_entities = new ConcurrentHashSet<Entity>();
			_components = new ConcurrentDictionary<Type, ConcurrentDictionary<Entity, IComponent>>();
		}

		public void Update()
		{

		}
	}
}
