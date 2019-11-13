using System;
using System.Collections.Generic;
using System.Text;
using Beffyman.Components.Manager;
using Beffyman.Components.Systems;

namespace Beffyman.Components.Benchmarks.Systems
{
	public class WorkComponent : IComponent
	{

	}

	public class DoWorkJobSystem : JobComponentSystem
	{
		protected override void OnUpdate(in UpdateStep step)
		{
			var job = new DoWorkJob();

			Execute<DoWorkJob, WorkComponent>(job);
		}

		protected readonly struct DoWorkJob : IJobForEach<WorkComponent>
		{
			public void Execute(WorkComponent work)
			{

			}
		}
	}
}
