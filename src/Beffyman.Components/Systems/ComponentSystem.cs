using System;
using System.Collections.Generic;
using System.Text;

namespace Beffyman.Components.Systems
{
	public abstract class ComponentSystem : ComponentSystemBase
	{
		protected abstract void OnUpdate();
	}
}
