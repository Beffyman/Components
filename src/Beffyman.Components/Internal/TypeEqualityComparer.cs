using System;
using System.Collections.Generic;
using System.Text;

namespace Beffyman.Components.Internal
{
	internal sealed class TypeEqualityComparer : IEqualityComparer<Type>
	{
		public static readonly TypeEqualityComparer Instance = new TypeEqualityComparer();

		public bool Equals(Type x, Type y)
		{
			return x.GUID == y.GUID;
		}

		public int GetHashCode(Type obj)
		{
			return obj.GetHashCode();
		}
	}
}
