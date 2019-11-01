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
		/// <summary>
		/// Keeps track of jobs scheduled within this frame in this component
		/// </summary>
		/// <typeparam name="T"></typeparam>
		[StructLayout(LayoutKind.Sequential)]
		protected ref struct JobHandle<T> where T : unmanaged
		{
			internal SpanStack<T> Jobs;

			public JobHandle(in Span<T> jobs)
			{
				Jobs = new SpanStack<T>(jobs);
			}
		}

		protected interface IJobForEach
		{
			void Execute();
		}

		protected interface IJobForEach<T>
			where T : IComponent
		{
			void Execute(in T arg);
		}

		protected interface IJobForEach<TFirst, TSecond>
			where TFirst : IComponent
			where TSecond : IComponent
		{
			void Execute(in TFirst arg1, in TSecond arg2);
		}

		protected interface IJobForEach<TFirst, TSecond, TThird>
			where TFirst : IComponent
			where TSecond : IComponent
			where TThird : IComponent
		{
			void Execute(in TFirst arg1, in TSecond arg2, in TThird arg3);
		}
	}
}