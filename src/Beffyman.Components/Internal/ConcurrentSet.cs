using System;
using System.Buffers;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Linq;

namespace Beffyman.Components.Internal
{
	/// <summary>
	/// Fake Set implementation which uses a backing <see cref="ConcurrentDictionary{TKey, TValue}"/>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	internal class ConcurrentSet<T> : ISet<T> where T : class
	{
		private readonly ConcurrentDictionary<int, T> _dictionary;
		private readonly IEqualityComparer<T> _comparer;

		public ConcurrentSet(int concurrencyLevel, int capacity, IEqualityComparer<T> comparer)
		{
			_dictionary = new ConcurrentDictionary<int, T>(concurrencyLevel, capacity);
			_comparer = comparer;
		}
		public ConcurrentSet(IEqualityComparer<T> comparer)
		{
			_dictionary = new ConcurrentDictionary<int, T>();
			_comparer = comparer;
		}

		public int Count => _dictionary.Count;

		public bool IsReadOnly => false;

		public bool Add(T item)
		{
			return _dictionary.TryAdd(_comparer.GetHashCode(item), item);
		}

		public void Clear()
		{
			_dictionary.Clear();
		}

		public bool Contains(T item)
		{
			return _dictionary.ContainsKey(_comparer.GetHashCode(item));
		}

		public IEnumerator<T> GetEnumerator()
		{
			foreach (var item in _dictionary)
			{
				yield return item.Value;
			}
		}

		public bool Remove(T item)
		{
			return _dictionary.Remove(_comparer.GetHashCode(item), out T _);
		}

		void ICollection<T>.Add(T item)
		{
			_dictionary.TryAdd(_comparer.GetHashCode(item), item);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			foreach (var item in _dictionary)
			{
				yield return item.Value;
			}
		}

		public T GetOrAdd(T item)
		{
			return _dictionary.GetOrAdd(_comparer.GetHashCode(item), item);
		}

		public T GetOrAddByHash(int hashCode, Func<int, T> addFunc)
		{
			return _dictionary.GetOrAdd(hashCode, addFunc);
		}

		public T GetByHash(int hashCode)
		{
			if (_dictionary.TryGetValue(hashCode, out T val))
			{
				return val;
			}

			return default(T);
		}


		#region Unimplemented


		public void CopyTo(T[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public void ExceptWith(IEnumerable<T> other)
		{
			throw new NotImplementedException();
		}

		public bool SetEquals(IEnumerable<T> other)
		{
			throw new NotImplementedException();
		}

		public void SymmetricExceptWith(IEnumerable<T> other)
		{
			throw new NotImplementedException();
		}

		public void UnionWith(IEnumerable<T> other)
		{
			throw new NotImplementedException();
		}

		public void IntersectWith(IEnumerable<T> other)
		{
			throw new NotImplementedException();
		}

		public bool IsProperSubsetOf(IEnumerable<T> other)
		{
			throw new NotImplementedException();
		}

		public bool IsProperSupersetOf(IEnumerable<T> other)
		{
			throw new NotImplementedException();
		}

		public bool IsSubsetOf(IEnumerable<T> other)
		{
			throw new NotImplementedException();
		}

		public bool IsSupersetOf(IEnumerable<T> other)
		{
			throw new NotImplementedException();
		}

		public bool Overlaps(IEnumerable<T> other)
		{
			throw new NotImplementedException();
		}

		#endregion Unimplemented

	}
}
