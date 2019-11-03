using System;
using System.Buffers;
using System.Collections.Concurrent;
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

		protected void Schedule<T>(in T job) where T : unmanaged, IJobForEach
		{
			void Handle(in T j)
			{
				j.Execute();
			}

			int entityCount = Manager.Entities.Count;
			int batchSize = (int)Math.Ceiling((float)entityCount / Environment.ProcessorCount);
			int batches = (int)Math.Ceiling((float)entityCount / batchSize);

			//!? Wrapping the method group inside a lambda causes it to avoid an action allocation, DO NOT CHANGE THIS
			for (int i = 0; i < batches; i++)
			{
				int start = i * batchSize;
				int end = Math.Min(start + batchSize, entityCount);


				_ = ThreadPool.UnsafeQueueUserWorkItem<T>((T j) =>
				{
					Handle(j);
				}, job, true);
			}
		}

		//protected void Schedule<T, TFirst>(in T job) where T : unmanaged, IJobForEach<TFirst>
		//	where TFirst: IComponent
		//{
		//	void Handle<T,TFirst>(in T j) where T : unmanaged, IJobForEach<TFirst>
		//	where TFirst : IComponent
		//	{
		//		Manager.GetComponent<TFirst>()
		//		j.Execute();
		//	}

		//	//!? Wrapping the method group inside a lambda causes it to avoid an action allocation, DO NOT CHANGE THIS
		//	_ = ThreadPool.UnsafeQueueUserWorkItem<T>((T j) =>
		//	{
		//		Handle<T,TFirst>(j);
		//	}, job, true);
		//}


		protected abstract ref JobHandle OnUpdate(ref JobHandle jobs, in UpdateStep step);

		internal override void Update(in UpdateStep step)
		{
			JobHandle jobs = new JobHandle();

			OnUpdate(ref jobs, step);

			//Do Work for the jobs here, queue up threads and then sync them up
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
