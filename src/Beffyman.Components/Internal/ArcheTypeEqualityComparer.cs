using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Beffyman.Components.Internal
{
	internal sealed class ArcheTypeEqualityComparer : IEqualityComparer<ArcheType>
	{
		public static readonly ArcheTypeEqualityComparer Instance = new ArcheTypeEqualityComparer();

		public bool Equals(ArcheType x, ArcheType y)
		{
			return x._componentTypes.SetEquals(y._componentTypes);
		}

		public int GetHashCode(ArcheType obj)
		{
			int hc = 0;
			for (int i = 0; i < obj.ComponentTypes.Length; i++)
			{
				hc ^= obj.ComponentTypes[i].GetHashCode();
			}

			return hc;
		}
	}
}
