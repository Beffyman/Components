using System;
using System.Collections.Generic;
using System.Text;

namespace Beffyman.Components.Manager
{
	public readonly ref struct UpdateStep
	{
		public readonly double DeltaTime;


		public UpdateStep(double deltaTime)
		{
			DeltaTime = deltaTime;
		}
	}
}
