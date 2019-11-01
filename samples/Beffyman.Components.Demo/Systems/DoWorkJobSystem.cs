using System;
using System.Collections.Generic;
using System.Text;
using Beffyman.Components.Manager;
using Beffyman.Components.Systems;

namespace Beffyman.Components.Demo.Systems
{
	public class DoWorkJobSystem : JobComponentSystem
	{
		protected override JobHandle<IntPtr> OnUpdate(ref JobHandle<IntPtr> jobs, in UpdateStep step)
		{
			var job = new DoWorkJob();

			Schedule(ref jobs, ref job);
		}

		protected readonly struct DoWorkJob : IJobForEach
		{
			public void Execute()
			{

			}
		}
	}
}
