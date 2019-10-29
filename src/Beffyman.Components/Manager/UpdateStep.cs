using System;
using System.Collections.Generic;
using System.Text;

namespace Beffyman.Components.Manager
{
	public readonly ref struct UpdateStep
	{
		public readonly float DeltaTime;


		public UpdateStep(float deltaTime)
		{
			DeltaTime = deltaTime;
		}
	}
}
