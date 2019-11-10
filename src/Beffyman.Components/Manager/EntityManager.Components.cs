using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Beffyman.Components.Internal;

namespace Beffyman.Components.Manager
{
	public partial class EntityManager
	{
		public bool HasComponent<T>(Entity entity) where T : class, IComponent, new() => HasComponent(entity, typeof(T));
		public bool HasComponent(Entity entity, Type type)
		{
			if (entity == null || type == null)
			{
				return false;
			}

			lock (entity)
			{
				if (_components.TryGetValue(type, out ConcurrentDictionary<Entity, IComponent> componentsOfT))
				{
					return componentsOfT.ContainsKey(entity);
				}

				return false;
			}
		}


		public IComponent GetComponent(Entity entity, Type type)
		{
			if (entity == null || type == null)
			{
				return default;
			}

			lock (entity)
			{
				if (_components.TryGetValue(type, out ConcurrentDictionary<Entity, IComponent> componentsOfT))
				{
					if (componentsOfT.TryGetValue(entity, out IComponent component))
					{
						return component;
					}
				}

				return default;
			}
		}

		public T GetComponent<T>(Entity entity) where T : class, IComponent, new()
		{
			if (entity == null)
			{
				return default;
			}

			lock (entity)
			{
				var type = typeof(T);
				if (_components.TryGetValue(type, out ConcurrentDictionary<Entity, IComponent> componentsOfT))
				{
					if (componentsOfT.TryGetValue(entity, out IComponent component))
					{
						if (component is T typedComponent)
						{
							return typedComponent;
						}
					}
				}

				return default;
			}
		}

		public IComponent AddComponent(Entity entity, Type type)
		{
			if (entity == null || type == null)
			{
				return default;
			}

			lock (entity)
			{
				var componentDictionary = _components.GetOrAdd(type, (t) => CreateComponentDictionary(t));
				var component = componentDictionary.GetOrAdd(entity, (e) => CreateComponent(e));

				RemapEntityArcheType(entity);

				return component;
			}

			static ConcurrentDictionary<Entity, IComponent> CreateComponentDictionary(Type t) => new ConcurrentDictionary<Entity, IComponent>(EntityEqualityComparer.Instance);
			IComponent CreateComponent(Entity e)
			{
				return Activator.CreateInstance(type) as IComponent;
			}
		}

		public T AddComponent<T>(Entity entity) where T : class, IComponent, new()
		{
			if (entity == null)
			{
				return default;
			}

			lock (entity)
			{
				var type = typeof(T);
				var componentDictionary = _components.GetOrAdd(type, (t) => CreateComponentDictionary(t));
				var component = componentDictionary.GetOrAdd(entity, (e) => CreateComponent(e));

				RemapEntityArcheType(entity);

				if (component is T typedComponent)
				{
					return typedComponent;
				}

				return default(T);
			}


			static ConcurrentDictionary<Entity, IComponent> CreateComponentDictionary(Type t) => new ConcurrentDictionary<Entity, IComponent>(EntityEqualityComparer.Instance);
			IComponent CreateComponent(Entity e)
			{
				return new T();
			}
		}



		public T AddComponent<T>(Entity entity, T addedComponent, bool overwrite = false) where T : class, IComponent, new()
		{
			if (entity == null)
			{
				return default;
			}

			lock (entity)
			{
				var type = typeof(T);
				var componentDictionary = _components.GetOrAdd(type, (t) => CreateComponentDictionary(t));

				if (componentDictionary.TryGetValue(entity, out IComponent component))
				{
					if (component is T typedComponent)
					{
						if (overwrite)
						{
							if (componentDictionary.TryRemove(entity, out _))
							{
								if (componentDictionary.TryAdd(entity, addedComponent))
								{
									RemapEntityArcheType(entity);
									return addedComponent;
								}
								else
								{
									return AddComponent(entity, addedComponent, overwrite);
								}
							}
							else
							{
								return AddComponent(entity, addedComponent, overwrite);
							}
						}
						else
						{
							RemapEntityArcheType(entity);
							return typedComponent;
						}
					}
				}
				else
				{
					if (componentDictionary.TryAdd(entity, addedComponent))
					{
						RemapEntityArcheType(entity);
						return addedComponent;
					}
					else
					{
						return AddComponent(entity, addedComponent, overwrite);
					}
				}

				RemapEntityArcheType(entity);
				return default(T);
			}



			static ConcurrentDictionary<Entity, IComponent> CreateComponentDictionary(Type t) => new ConcurrentDictionary<Entity, IComponent>(EntityEqualityComparer.Instance);
		}


		public bool RemoveComponent<T>(Entity entity) where T : class, IComponent, new() => RemoveComponent(entity, typeof(T));
		public bool RemoveComponent(Entity entity, Type type)
		{
			if (entity == null || type == null)
			{
				return false;
			}

			lock (entity)
			{
				if (_components.TryGetValue(type, out ConcurrentDictionary<Entity, IComponent> componentsOfT))
				{
					if (componentsOfT.TryRemove(entity, out IComponent component))
					{
						if (component is IDisposable disposable)
						{
							disposable.Dispose();
						}

						RemapEntityArcheType(entity);
						return true;
					}
				}

				RemapEntityArcheType(entity);
				return false;
			}
		}

		public void RemoveAllComponents(Entity entity)
		{
			foreach (var typedComponents in _components)
			{
				RemoveComponent(entity, typedComponents.Key);
			}
		}

		public IEnumerable<IComponent> GetComponents(Entity entity)
		{
			lock (entity)
			{
				foreach (var typedComponents in _components)
				{
					if (typedComponents.Value.TryGetValue(entity, out IComponent component))
					{
						yield return component;
					}
				}
			}
		}

		public Type[] GetComponentTypes(Entity entity)
		{
			lock (entity)
			{
				//Should try and use a Pool here, but returning it may be tricky if this method isn't locked down
				Type[] types = new Type[_components.Count];

				int i = 0;
				foreach (var typedComponents in _components)
				{
					if (typedComponents.Value.ContainsKey(entity))
					{
						types[i++] = typedComponents.Key;
					}
				}

				return types;
			}
		}

		private void DestroyComponents()
		{
			foreach (var typedComponents in _components)
			{
				typedComponents.Value.Clear();
			}

			_components.Clear();
		}
	}
}
