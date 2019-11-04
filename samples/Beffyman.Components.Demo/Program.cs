using System;
using System.Diagnostics;
using System.Globalization;
using Beffyman.Components.Demo.Components;
using Beffyman.Components.Manager;

namespace Beffyman.Components.Demo
{
	public static class Program
	{

		private static Stopwatch timer = new Stopwatch();
		private static long _nanosecPerTick = (1000L * 1000L * 1000L) / Stopwatch.Frequency;
		private static double deltaTime;
		private const double fpsTimer = 2.0f;
		private static double fpsTimerAcc = 0;

		public static void Main()
		{
			var manager = new EntityManager(new EntityManagerOptions
			{
				Multithreading = true,
				InitialPoolSize = 500
			});

			for (int i = 0; i < 5000; i++)
			{
				var entity = manager.CreateEntity();
				entity.AddComponent<TrackerComponent>();
			}

			while (true)
			{
				deltaTime = timer.ElapsedTicks * _nanosecPerTick * 1e-9;
				timer.Restart();
				fpsTimerAcc += deltaTime;

				if (fpsTimerAcc > fpsTimer)
				{
					fpsTimerAcc = 0;
					Console.WriteLine($"FPS: {(1d / deltaTime).ToString(CultureInfo.CurrentCulture)}");
				}

				manager.Update(new UpdateStep(deltaTime));
			}
		}
	}
}
