using System;
using System.Diagnostics;
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

			while (true)
			{
				deltaTime = timer.ElapsedTicks * _nanosecPerTick * 1e-9;
				timer.Restart();
				fpsTimerAcc += deltaTime;

				if (fpsTimerAcc > fpsTimer)
				{
					fpsTimerAcc = 0;
					Console.WriteLine($"FPS: {(1d / deltaTime).ToString()}");
				}

				manager.Update(new UpdateStep(deltaTime));
			}
		}
	}
}
