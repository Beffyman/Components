using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Beffyman.Components.Internal;
using Beffyman.Components.Manager;

namespace Beffyman.Components
{
	public sealed class ArcheType
	{
		internal HashSet<Type> _componentTypes { get; }
		public readonly Type[] ComponentTypes;

		public ArcheType(Type[] componentTypes)
		{
			_componentTypes = componentTypes.ToHashSet(TypeEqualityComparer.Instance);
			ComponentTypes = componentTypes;
		}

		public bool HasComponent<T>() where T : IComponent
		{
			return _componentTypes.Contains(typeof(T));
		}

		public bool HasComponent(Type type)
		{
			return _componentTypes.Contains(type);
		}

		public bool IsArcheType(IEnumerable<Type> types)
		{
			return _componentTypes.SetEquals(types);
		}
	}
}
