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

		protected unsafe void Schedule<T>(in JobHandle<IntPtr> jobs, ref T job) where T : unmanaged
		{
			IntPtr ptr = new IntPtr();
			Marshal.StructureToPtr(job, ptr, true);
			jobs.Jobs.Add(ptr);
		}

		protected abstract void OnUpdate(in JobHandle<IntPtr> jobs, in UpdateStep step);

		internal override void Update(in UpdateStep step)
		{
			var jobs = new JobHandle<IntPtr>();

			OnUpdate(jobs, step);

		}

		internal override void Load(EntityManager manager)
		{
			base.Load(manager);
			var jobTypes = this.GetType()
				.GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Public)
				.Where(x => typeof(IJob).IsAssignableFrom(x))
				.ToArray();

			//var jobInterfaceTypes = new Type[]
			//{
			//	typeof(IJobForEach<>),
			//	typeof(IJobForEach<,>),
			//	typeof(IJobForEach<,,>)
			//};

			NestedJobs = jobTypes;
		}
	}
}
