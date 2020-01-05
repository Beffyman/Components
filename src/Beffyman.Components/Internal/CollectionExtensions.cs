using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beffyman.Components.Internal
{
	internal static class CollectionExtensions
	{
		public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key, Func<TValue> addFunc)
			where TValue : class
		{
			TValue val;
			if (!source.TryGetValue(key, out val))
			{
				val = addFunc();
				source.Add(key, val);
			}

			return val;
		}


		public static HashSet<T>[] ToPowerHashSet<T>(this T[] seq, IEqualityComparer<T> equalityComparer)
		{
			var powerSet = new HashSet<T>[1 << seq.Length];
			powerSet[0] = new HashSet<T>(equalityComparer); // starting only with empty set
			for (int i = 0; i < seq.Length; i++)
			{
				var cur = seq[i];
				int count = 1 << i; // doubling list each time
				for (int j = 0; j < count; j++)
				{
					var source = powerSet[j];
					var destination = powerSet[count + j] = new HashSet<T>(equalityComparer);

					foreach (var q in source)
					{
						destination.Add(q);
					}

					destination.Add(cur);
				}
			}
			return powerSet;
		}
	}
}
