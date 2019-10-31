using System;
using System.Collections.Generic;
using System.Text;

namespace Beffyman.Components.Internal
{
#if NETSTANDARD2_0
	internal static class LinqExtensions
	{
		/// <summary>
		/// This method doesn't exist in netstandard2.0 linq
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="equalityComparer"></param>
		/// <returns></returns>
		public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T> equalityComparer)
		{
			return new HashSet<T>(source, equalityComparer);
		}
	}
#endif
}
