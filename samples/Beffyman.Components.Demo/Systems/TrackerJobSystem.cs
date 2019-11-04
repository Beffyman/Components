using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Beffyman.Components.Demo.Components;
using Beffyman.Components.Manager;
using Beffyman.Components.Systems;

namespace Beffyman.Components.Demo.Systems
{
	public class TrackerJobSystem : JobComponentSystem
	{
		protected override void OnUpdate(in UpdateStep step)
		{
			var job = new TrackerJob();

			Execute<TrackerJob, TrackerComponent>(job);

		}

		protected readonly struct TrackerJob : IJobForEach<TrackerComponent>
		{
			public void Execute(TrackerComponent tracker)
			{
				Interlocked.Increment(ref tracker.Ticks);
			}
		}
	}
}
