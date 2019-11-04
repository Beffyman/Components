using System;
using System.Collections.Generic;
using System.Text;
using Beffyman.Components.Manager;
using Beffyman.Components.Systems;

namespace Beffyman.Components.Demo.Systems
{
	public class DoWorkJobSystem : JobComponentSystem
	{
		protected override void OnUpdate(in UpdateStep step)
		{
			var job = new DoWorkJob();

			Execute(job);
		}

		protected readonly struct DoWorkJob : IJobForEach
		{
			public void Execute()
			{

			}
		}
	}
}
