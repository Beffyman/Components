using System;
using System.Collections.Generic;
using System.Text;
using Beffyman.Components.Manager;
using Beffyman.Components.Systems;

namespace Beffyman.Components.Benchmarks.Systems
{
	public class DoWorkJobSystem : JobComponentSystem
	{
		protected override ref JobHandle OnUpdate(ref JobHandle jobs, in UpdateStep step)
		{
			var job = new DoWorkJob();

			Schedule(job);

			return ref jobs;
		}

		protected readonly struct DoWorkJob : IJobForEach
		{
			public void Execute()
			{

			}
		}
	}
}
