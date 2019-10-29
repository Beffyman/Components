using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Beffyman.Components.Internal;
using ConcurrentCollections;

namespace Beffyman.Components.Manager
{
	public partial class EntityManager
	{
		public ArcheType CreateArcheType(Type[] types)
		{
			var archeType = new ArcheType(types);
			if (!_archTypes.Add(archeType))
			{
				return archeType;
			}

			//Idk how to make this an O(1), hashset doesn't have a "get if same or add" method like ConcurrentDictionary's GetOrAdd
#warning perf issue
			return _archTypes.SingleOrDefault(x => ArcheTypeEqualityComparer.Instance.Equals(x, archeType));
		}
	}
}
