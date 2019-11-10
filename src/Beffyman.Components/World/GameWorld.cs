using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Beffyman.Components.Manager;

namespace Beffyman.Components.World
{
	/// <summary>
	/// A helper class that will handle fixed/free timestepping for the Manager
	/// </summary>
	public sealed class GameWorld
	{
		#region Stepping Variables

		private Stopwatch timer = new Stopwatch();
		private long _nanosecPerTick = (1000L * 1000L * 1000L) / Stopwatch.Frequency;
		private double _deltaTime;
		private double _accumulatedDeltaTime;

		private const double _fpsTimer = 2.0f;
		private double _fpsTimerAcc = 0;

		#endregion Stepping Variables

		public Guid Id { get; } = Guid.NewGuid();
		public bool Running { get; set; }
		public GameWorldOptions Options { get; }
		public EntityManager Manager { get; }

		public GameWorld(GameWorldOptions options = null)
		{
			//Assign options if left null
			Options = options ?? new GameWorldOptions();
			Options.EntityManagerOptions = Options.EntityManagerOptions ?? new EntityManagerOptions();

			Manager = new EntityManager(Options.EntityManagerOptions);
		}

		/// <summary>
		/// Blocks the current thread until running is complete
		/// </summary>
		public void Run()
		{
			Running = true;

			while (Running)
			{
				_deltaTime = timer.ElapsedTicks * _nanosecPerTick * 1e-9;
				timer.Restart();
				_accumulatedDeltaTime += _deltaTime;

				ConsoleFps();

				FixedTimestep();

				Manager.Update(new UpdateStep(_deltaTime));
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ConsoleFps()
		{
			_fpsTimerAcc += _deltaTime;
			if (_fpsTimerAcc > _fpsTimer)
			{
				_fpsTimerAcc = 0;

				if (Options.OutputFpsToConsole)
				{
					Console.WriteLine($"FPS: {(1d / _deltaTime).ToString(CultureInfo.CurrentCulture)}");
				}
				else
				{
					Debug.WriteLine($"FPS: {(1d / _deltaTime).ToString(CultureInfo.CurrentCulture)}");
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void FixedTimestep()
		{
			while (_accumulatedDeltaTime >= Manager.Options.FixedTimeStep)
			{
				_accumulatedDeltaTime -= Manager.Options.FixedTimeStep;

				Manager.FixedUpdate(new UpdateStep(Manager.Options.FixedTimeStep));
			}
		}

		/// <summary>
		/// Runs the world in a separate thread
		/// </summary>
		public Thread RunInThread(ThreadPriority priority = ThreadPriority.Highest, bool startThread = true)
		{
			Thread thread = new Thread(Run);
			thread.Priority = priority;
			thread.Name = $"GameWorld_{Id.ToString()}";
			if (startThread)
			{
				thread.Start();
			}

			return thread;
		}

	}
}
