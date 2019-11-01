using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Beffyman.Components.Internal
{
	/// <summary>
	/// Defines an array of <see cref="T"/> that will dynamically expand while reusing the backing array from a pool
	/// </summary>
	/// <typeparam name="T"></typeparam>
	internal ref struct SpanStack<T> where T : unmanaged
	{
		private Span<T> Pointers;
		private T[] _backingArray;
		public int NextPosition;
		public int Length;

		public SpanStack(in Span<T> objects)
		{
			NextPosition = 0;
			Pointers = objects;
			Length = objects.Length;
			_backingArray = null;
		}

		/// <summary>
		/// Is the array passed in one from the <see cref="ArrayPool{T}"/> Shared Instance?  If it is, it will be returned when this array resizes
		/// </summary>
		/// <param name="objects"></param>
		/// <param name="pooled"></param>
		public SpanStack(in T[] objects, bool pooled = false)
		{
			NextPosition = 0;
			Pointers = objects;
			Length = objects.Length;

			if (pooled)
			{
				_backingArray = objects;
			}
			else
			{
				_backingArray = null;
			}
		}

		public void Add(in T obj)
		{
			if (Pointers.Length == Length)
			{
				Resize();
			}

			Pointers[NextPosition++] = obj;
			if (NextPosition > Length)
			{
				Length++;
			}
		}

		private void Resize()
		{
			int newSize;
			if (Pointers.IsEmpty)
			{
				newSize = 8;
			}
			else
			{
				newSize = Pointers.Length * 2;
			}

			var oldBackingArray = _backingArray;
			_backingArray = ArrayPool<T>.Shared.Rent(newSize);
			var newSpan = _backingArray.AsSpan();
			Pointers.CopyTo(newSpan);
			if (oldBackingArray != null)
			{
				ArrayPool<T>.Shared.Return(oldBackingArray);
			}
			Pointers = newSpan;
		}

		public bool MoveNext()
		{
			if (NextPosition < Length)
			{
				NextPosition++;
				return true;
			}

			return false;
		}

		public T Get()
		{
			return Pointers[NextPosition];
		}

		public void ResetPositon()
		{
			NextPosition = 0;
		}

		public T this[int index]
		{
			get => Pointers[index];
		}
	}
}
