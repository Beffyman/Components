using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Beffyman.Components.Internal;

namespace Beffyman.Components.Manager
{
	public partial class EntityManager
	{
		private readonly InfiniteScaleObjectPool<Entity> _entityPool;
		private readonly ConcurrentStack<Entity> _entitiesToBeAdded;
		private readonly ConcurrentStack<Entity> _entitiesToBeDestroyed;

		private readonly List<Entity> _entities;

		public IReadOnlyList<Entity> Entities => _entities;


		private readonly ConcurrentDictionary<Type, ConcurrentDictionary<Entity, IComponent>> _components;

		public readonly EntityManagerOptions Options;

		public EntityManager(EntityManagerOptions options = null)
		{
			Options = options ?? new EntityManagerOptions();

			_entitiesToBeAdded = new ConcurrentStack<Entity>();
			_entitiesToBeDestroyed = new ConcurrentStack<Entity>();
			_entityPool = InfiniteScaleObjectPool<Entity>.Create(Options.InitialPoolSize);

			_entities = new List<Entity>();

			_components = new ConcurrentDictionary<Type, ConcurrentDictionary<Entity, IComponent>>();
		}

		public void Update()
		{
			FlushEntityCollections();


		}
	}
}
