using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Beffyman.Components.Manager
{
	public partial class EntityManager
	{
		public bool HasComponent<T>(in Entity entity) where T : class, IComponent, new() => HasComponent(entity, typeof(T));
		public bool HasComponent(in Entity entity, Type type)
		{
			if (entity == null || type == null)
			{
				return false;
			}

			if (_components.TryGetValue(type, out ConcurrentDictionary<Entity, IComponent> componentsOfT))
			{
				return componentsOfT.ContainsKey(entity);
			}

			return false;
		}


		public IComponent GetComponent(in Entity entity, Type type)
		{
			if (entity == null || type == null)
			{
				return default;
			}

			if (_components.TryGetValue(type, out ConcurrentDictionary<Entity, IComponent> componentsOfT))
			{
				if (componentsOfT.TryGetValue(entity, out IComponent component))
				{
					return component;
				}
			}

			return default;
		}

		public T GetComponent<T>(in Entity entity) where T : class, IComponent, new()
		{
			if (entity == null)
			{
				return default;
			}

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

		public IComponent AddComponent(in Entity entity, Type type)
		{
			if (entity == null || type == null)
			{
				return default;
			}

			var componentDictionary = _components.GetOrAdd(type, CreateComponentDictionary);
			return componentDictionary.GetOrAdd(entity, CreateComponent);

			static ConcurrentDictionary<Entity, IComponent> CreateComponentDictionary(Type t) => new ConcurrentDictionary<Entity, IComponent>();
			IComponent CreateComponent(Entity e)
			{
				return Activator.CreateInstance(type) as IComponent;
			}
		}

		public T AddComponent<T>(in Entity entity) where T : class, IComponent, new()
		{
			if (entity == null)
			{
				return default;
			}

			var type = typeof(T);
			var componentDictionary = _components.GetOrAdd(type, CreateComponentDictionary);
			var component = componentDictionary.GetOrAdd(entity, CreateComponent);

			if (component is T typedComponent)
			{
				return typedComponent;
			}

			return default(T);

			static ConcurrentDictionary<Entity, IComponent> CreateComponentDictionary(Type t) => new ConcurrentDictionary<Entity, IComponent>();
			IComponent CreateComponent(Entity e)
			{
				return new T();
			}
		}

		public bool RemoveComponent<T>(in Entity entity) where T : class, IComponent, new() => RemoveComponent(entity, typeof(T));
		public bool RemoveComponent(in Entity entity, Type type)
		{
			if (entity == null || type == null)
			{
				return false;
			}

			if (_components.TryGetValue(type, out ConcurrentDictionary<Entity, IComponent> componentsOfT))
			{
				if (componentsOfT.TryRemove(entity, out IComponent component))
				{
					if (component is IDisposable disposable)
					{
						disposable.Dispose();
					}

					return true;
				}
			}

			return false;
		}

		public void RemoveAllComponents(in Entity entity)
		{
			foreach (var typedComponents in _components)
			{
				RemoveComponent(entity, typedComponents.Key);
			}
		}

		public IEnumerable<IComponent> GetComponents(Entity entity)
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
}
