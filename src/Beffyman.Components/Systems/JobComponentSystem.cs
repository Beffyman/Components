using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Beffyman.Components.Manager;

namespace Beffyman.Components.Systems
{
	/// <summary>
	/// Defines a system that will run once per entity that matches the job's generic arguments if it is scheduled this frame
	/// </summary>
	public abstract class JobComponentSystem : ComponentSystemBase
	{
		protected void Schedule<T>(in T job) where T : struct, IJob
		{

		}

		internal override void Update(in UpdateStep step)
		{
			OnUpdate(step);


		}


		protected interface IJob
		{

		}

		protected interface IJob<T> : IJob
			where T : IComponent
		{
			void Execute(in T arg);
		}

		protected interface IJob<TFirst, TSecond> : IJob
			where TFirst : IComponent
			where TSecond : IComponent
		{
			void Execute(in TFirst arg1, in TSecond arg2);
		}

		protected interface IJob<TFirst, TSecond, TThird> : IJob
			where TFirst : IComponent
			where TSecond : IComponent
			where TThird : IComponent
		{
			void Execute(in TFirst arg1, in TSecond arg2, in TThird arg3);
		}

		internal override void Load(EntityManager manager)
		{
			base.Load(manager);
			var jobTypes = this.GetType()
				.GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Public)
				.Where(x => typeof(IJob).IsAssignableFrom(x))
				.ToArray();

			var jobInterfaceTypes = new Type[]
			{
				typeof(IJob<>),
				typeof(IJob<,>),
				typeof(IJob<,,>)
			};


		}
	}
}
