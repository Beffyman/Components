using Beffyman.Components.Demo.Components;
using Beffyman.Components.World;

namespace Beffyman.Components.Demo
{
	public static class Program
	{
		public static void Main()
		{
			GameWorld world = new GameWorld(new GameWorldOptions
			{
				OutputFpsToConsole = true
			});

			for (int i = 0; i < 5000; i++)
			{
				var entity = world.Manager.CreateEntity();
				entity.AddComponent<TrackerComponent>();
			}

			world.Run();
		}
	}
}
