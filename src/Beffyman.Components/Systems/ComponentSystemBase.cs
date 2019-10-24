using System;
using System.Collections.Generic;
using System.Text;

namespace Beffyman.Components.Systems
{
	public abstract class ComponentSystemBase
	{
		/// <summary>
		/// Is this system currently running updates?
		/// </summary>
		public bool Enabled { get; set; }

		public bool ShouldRunSystem()
		{
#warning TODO
			return Enabled;
		}


		protected virtual void OnCreate() { }
		protected virtual void OnDestroy() { }
		protected virtual void OnStartRunning() { }
		protected virtual void OnStopRunning() { }
	}
}
