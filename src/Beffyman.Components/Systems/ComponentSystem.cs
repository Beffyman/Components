using System;
using System.Collections.Generic;
using System.Text;
using Beffyman.Components.Manager;

namespace Beffyman.Components.Systems
{
	/// <summary>
	/// Defines a system that will run once per update
	/// </summary>
	public abstract class ComponentSystem : ComponentSystemBase
	{
		protected virtual void OnUpdate(in UpdateStep step) { }

		internal override void Update(in UpdateStep step)
		{
			OnUpdate(step);
		}


		protected virtual void OnFixedUpdate(in UpdateStep step) { }

		internal override void FixedUpdate(in UpdateStep step)
		{
			OnFixedUpdate(step);
		}
	}
}
