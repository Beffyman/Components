using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.ObjectPool;

namespace Beffyman.Components.Internal
{
	internal class InfiniteScaleObjectPool<T> : ObjectPool<T> where T : class, new()
	{
		private protected readonly ConcurrentQueue<T> _items;
		private protected readonly DefaultPooledObjectPolicy<T> _policy;

		/// <summary>
		/// Creates an instance of <see cref="InfiniteScaleObjectPool{T}"/>.
		/// </summary>
		public InfiniteScaleObjectPool()
		{
			_policy = new DefaultPooledObjectPolicy<T>();
			_items = new ConcurrentQueue<T>();
		}

		public override T Get()
		{
			if (_items.TryDequeue(out T @object))
			{
				return @object;
			}
			else
			{
				return Create();
			}
		}

		// Non-inline to improve its code quality as uncommon path
		[MethodImpl(MethodImplOptions.NoInlining)]
		private T Create() => _policy.Create();

		public override void Return(T obj)
		{
			if (_policy.Return(obj))
			{
				_items.Enqueue(obj);
			}
		}

		public static InfiniteScaleObjectPool<T> Create(uint initialSize)
		{
			var pool = new InfiniteScaleObjectPool<T>();

			for (uint i = 0; i < initialSize; i++)
			{
				pool.Return(pool.Create());
			}

			return pool;
		}
	}
}
