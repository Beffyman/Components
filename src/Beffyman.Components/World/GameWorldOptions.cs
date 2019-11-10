using System;
using System.Collections.Generic;
using System.Text;
using Beffyman.Components.Manager;

namespace Beffyman.Components.World
{
	public sealed class GameWorldOptions
	{
		public bool OutputFpsToConsole { get; set; } = false;
		public EntityManagerOptions EntityManagerOptions { get; set; } = new EntityManagerOptions();
	}
}
