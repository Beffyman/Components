using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Beffyman.Components.Internal;
using Beffyman.Components.Manager;

namespace Beffyman.Components.Systems
{
	/// <summary>
	/// Defines a system that will run once per entity that matches the job's generic arguments if it is scheduled this frame
	/// </summary>
	public abstract partial class JobComponentSystem : ComponentSystemBase
	{
		private Type[] NestedJobs;


		//#error Figure out a way to have reduced allocation job queueing?
		//#error Possiblty expose a Get method which pools the job?
		//#error I don't think it's possible to use the structs as

		protected unsafe void Schedule<T>(ref JobHandle<IntPtr> jobs, ref T job) where T : unmanaged, IJobForEach
		{
			IntPtr ptr = Marshal.AllocHGlobal(sizeof(T));
			Marshal.StructureToPtr(job, ptr, false);

			jobs.Jobs.Add(ptr);
		}

		protected unsafe void Schedule<T, K>(ref JobHandle<IntPtr> jobs, ref T job) where T : unmanaged, IJobForEach<K>
			where K : IComponent
		{
			IntPtr ptr = Marshal.AllocHGlobal(sizeof(T));
			Marshal.StructureToPtr(job, ptr, false);

			jobs.Jobs.Add(ptr);
		}

		protected unsafe void Schedule<T, TFirst, TSecond>(ref JobHandle<IntPtr> jobs, ref T job) where T : unmanaged, IJobForEach<TFirst, TSecond>
			where TFirst : IComponent
			where TSecond : IComponent
		{
			IntPtr ptr = Marshal.AllocHGlobal(sizeof(T));
			Marshal.StructureToPtr(job, ptr, false);

			jobs.Jobs.Add(ptr);
		}

		protected unsafe void Schedule<T, TFirst, TSecond, TThird>(ref JobHandle<IntPtr> jobs, ref T job) where T : unmanaged, IJobForEach<TFirst, TSecond, TThird>
			where TFirst : IComponent
			where TSecond : IComponent
			where TThird : IComponent
		{
			IntPtr ptr = Marshal.AllocHGlobal(sizeof(T));
			Marshal.StructureToPtr(job, ptr, false);

			jobs.Jobs.Add(ptr);
		}

		protected abstract ref JobHandle<IntPtr> OnUpdate(ref JobHandle<IntPtr> jobs, in UpdateStep step);

		internal override void Update(in UpdateStep step)
		{
			JobHandle<IntPtr> jobs = new JobHandle<IntPtr>();

			try
			{
				OnUpdate(ref jobs, step);

				//Do Work for the jobs here, queue up threads and then sync them up
			}
			finally
			{
				//Free all the pointers for the jobs used
				FreeAllJobPointers(ref jobs);
			}
		}

		private void FreeAllJobPointers(ref JobHandle<IntPtr> jobs)
		{
			for (int i = 0; i < jobs.Jobs.Length; i++)
			{
				Marshal.FreeHGlobal(jobs.Jobs[i]);
			}
		}

		internal override void Load(EntityManager manager)
		{
			var jobInterfaceTypes = new Type[]
			{
				typeof(IJobForEach),
				typeof(IJobForEach<>),
				typeof(IJobForEach<,>),
				typeof(IJobForEach<,,>)
			};


			base.Load(manager);

			var jobTypes = this.GetType()
				.GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Public)
				.Where(x => jobInterfaceTypes.Any(x => x.IsAssignableFrom(x)))
				.ToArray();

			NestedJobs = jobTypes;
		}
	}
}
