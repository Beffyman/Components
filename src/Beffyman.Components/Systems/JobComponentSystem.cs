using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Beffyman.Components.Systems
{
	public abstract class JobComponentSystem : ComponentSystemBase
	{
		protected abstract JobHandle OnUpdate(JobHandle inputDeps);


		protected sealed class JobHandle
		{

		}

		protected interface IJob
		{
			void Execute();
		}

	}
}
