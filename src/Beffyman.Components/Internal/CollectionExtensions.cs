using System;
using System.Collections.Generic;
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
	}
}
