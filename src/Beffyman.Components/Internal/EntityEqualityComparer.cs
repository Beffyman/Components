using System;
using System.Collections.Generic;
using System.Text;

namespace Beffyman.Components.Internal
{
	internal sealed class EntityEqualityComparer : IEqualityComparer<Entity>
	{
		public static readonly EntityEqualityComparer Instance = new EntityEqualityComparer();

		public bool Equals(Entity x, Entity y)
		{
			return x.Id == y.Id;
		}

		public int GetHashCode(Entity obj)
		{
			return unchecked((int)obj.Id);
		}
	}
}
