using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Beffyman.Components.Internal;
using Beffyman.Components.Manager;

namespace Beffyman.Components
{
	public sealed class ArcheType : IEquatable<ArcheType>
	{
		internal Dictionary<int, ArcheType> ChildArcheTypes { get; }
		internal Dictionary<int, ArcheType> ParentArcheTypes { get; }


		internal Type[] _componentTypesArray { get; }
		internal HashSet<Type> _componentTypes { get; }

		public int Length { get; }
		public int HashCode { get; }


		public ArcheType(Type[] componentTypes)
		{
			_componentTypesArray = componentTypes;
			_componentTypes = componentTypes?.ToHashSet(TypeEqualityComparer.Instance) ?? new HashSet<Type>(TypeEqualityComparer.Instance);
			Length = _componentTypes?.Count ?? 0;
			HashCode = ArcheTypeEqualityComparer.GetHashCode(_componentTypesArray);
			ChildArcheTypes = new Dictionary<int, ArcheType>();
			ParentArcheTypes = new Dictionary<int, ArcheType>();
		}

		public ArcheType(HashSet<Type> componentTypes)
		{
			_componentTypesArray = componentTypes.ToArray();
			_componentTypes = componentTypes;
			Length = _componentTypes?.Count ?? 0;
			HashCode = ArcheTypeEqualityComparer.GetHashCode(_componentTypesArray);
			ChildArcheTypes = new Dictionary<int, ArcheType>();
			ParentArcheTypes = new Dictionary<int, ArcheType>();
		}

		public bool HasComponent<T>() where T : IComponent
		{
			return _componentTypes.Contains(typeof(T));
		}

		public bool HasComponent(Type type)
		{
			return _componentTypes.Contains(type);
		}

		internal void AddParent(ArcheType archetype)
		{
			ParentArcheTypes.Add(ArcheTypeEqualityComparer.Instance.GetHashCode(archetype), archetype);
		}

		public bool IsParentArcheTypeOf(Type[] types)
		{
			return ParentArcheTypes.ContainsKey(ArcheTypeEqualityComparer.GetHashCode(types));
		}

		public bool IsParentArcheTypeOf(ArcheType archetype)
		{
			return ParentArcheTypes.ContainsKey(ArcheTypeEqualityComparer.Instance.GetHashCode(archetype));
		}

		/// <summary>
		/// Yields all children back from the nested children of this archetype
		/// </summary>
		/// <returns></returns>
		internal IEnumerable<ArcheType> GetAllChildren()
		{
			if (ChildArcheTypes.Count > 0)
			{
				foreach (var kv in ChildArcheTypes)
				{
					yield return kv.Value;

					foreach (var child in kv.Value.GetAllChildren())
					{
						yield return child;
					}
				}
			}
		}

		internal void AddChild(ArcheType archetype)
		{
			ChildArcheTypes.Add(ArcheTypeEqualityComparer.Instance.GetHashCode(archetype), archetype);
		}

		public bool IsChildArcheTypeOf(Type[] types)
		{
			return ChildArcheTypes.ContainsKey(ArcheTypeEqualityComparer.GetHashCode(types));
		}

		public bool IsChildArcheTypeOf(ArcheType archetype)
		{
			return ChildArcheTypes.ContainsKey(ArcheTypeEqualityComparer.Instance.GetHashCode(archetype));
		}

		public bool Equals(ArcheType other)
		{
			return ArcheTypeEqualityComparer.Instance.Equals(this, other);
		}

		public override bool Equals(object obj)
		{
			return Equals((ArcheType)obj);
		}

		public override int GetHashCode()
		{
			return HashCode;
		}
	}
}
