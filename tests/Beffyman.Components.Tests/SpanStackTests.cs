using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using Beffyman.Components.Internal;
using Xunit;

namespace Beffyman.Components.Tests
{
	public class SpanStackTests
	{
		private struct TestData
		{
			public int X;
			public int Y;
			public int Z;
		}

		[Fact]
		public void Test()
		{
			var td1 = new TestData
			{
				X = 1,
				Y = 2,
				Z = 3
			};


			var rentedArray = ArrayPool<TestData>.Shared.Rent(8);
			SpanStack<TestData> spanStack = new SpanStack<TestData>(rentedArray);

			Assert.Equal(0, spanStack.NextPosition);
			Assert.Equal(8, spanStack.Length);

			spanStack.Add(td1);

			Assert.Equal(1, spanStack.NextPosition);
			Assert.Equal(8, spanStack.Length);

			var e_td1 = spanStack.Get();
			Assert.Equal(td1, e_td1);

			var ei_td1 = spanStack[0];
			Assert.Equal(td1, ei_td1);
		}
	}
}
