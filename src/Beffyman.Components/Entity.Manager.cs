using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Beffyman.Components
{
	//? This entire file is basically just a wrapper around EntityManager.Components.cs which makes it provide the entity argument from the calling entity

	public partial class Entity
	{
		public bool HasComponent<T>() where T : class, IComponent, new() => HasComponent(typeof(T));
		public bool HasComponent(Type type)
		{
			return Manager.HasComponent(this, type);
		}


		public IComponent GetComponent(Type type)
		{
			return Manager.GetComponent(this, type);
		}

		public T GetComponent<T>() where T : class, IComponent, new()
		{
			return Manager.GetComponent<T>(this);
		}

		public IComponent AddComponent(Type type)
		{
			return Manager.AddComponent(this, type);
		}

		public T AddComponent<T>() where T : class, IComponent, new()
		{
			return Manager.AddComponent<T>(this);
		}



		public T AddComponent<T>(T addedComponent, bool overwrite = false) where T : class, IComponent, new()
		{
			return Manager.AddComponent<T>(this, addedComponent, overwrite);
		}


		public bool RemoveComponent<T>() where T : class, IComponent, new() => RemoveComponent(typeof(T));
		public bool RemoveComponent(Type type)
		{
			return Manager.RemoveComponent(this, type);
		}

		public void RemoveAllComponents()
		{
			Manager.RemoveAllComponents(this);
		}

		public IEnumerable<IComponent> GetComponents()
		{
			return Manager.GetComponents(this);
		}

		public IEnumerable<Type> GetComponentTypes()
		{
			return Manager.GetComponentTypes(this);
		}

		internal Dictionary<Type, IComponent> GetEntityComponentsByArcheType(ArcheType archeType)
		{
			return Manager.GetEntityComponentsByArcheType(archeType, this);
		}

	}
}
