using System;
using System.Collections.Generic;
using System.Text;

namespace Beffyman.Components.Demo.Components
{
	public sealed class StatsComponent : IComponent
	{
		public int Health { get; set; }
		public int Mana { get; set; }
		public int Shield { get; set; }

	}
}
