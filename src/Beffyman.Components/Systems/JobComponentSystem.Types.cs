using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Beffyman.Components.Internal;

namespace Beffyman.Components.Systems
{
	public abstract partial class JobComponentSystem
	{
		[StructLayout(LayoutKind.Sequential)]
		protected readonly ref struct JobHandle<T> where T : unmanaged
		{
			internal readonly SpanStack<T> Jobs;

			public JobHandle(in Span<T> jobs)
			{
				Jobs = new SpanStack<T>(jobs);
			}
		}

		protected interface IJob
		{

		}

		protected interface IJobForEach<T> : IJob
			where T : IComponent
		{
			void Execute(in T arg);
		}

		protected interface IJobForEach<TFirst, TSecond> : IJob
			where TFirst : IComponent
			where TSecond : IComponent
		{
			void Execute(in TFirst arg1, in TSecond arg2);
		}

		protected interface IJobForEach<TFirst, TSecond, TThird> : IJob
			where TFirst : IComponent
			where TSecond : IComponent
			where TThird : IComponent
		{
			void Execute(in TFirst arg1, in TSecond arg2, in TThird arg3);
		}
	}
}
