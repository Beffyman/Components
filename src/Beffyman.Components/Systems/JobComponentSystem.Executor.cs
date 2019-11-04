using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Beffyman.Components.Internal;

namespace Beffyman.Components.Systems
{
	public partial class JobComponentSystem
	{
		private class Executor<T> : IThreadPoolWorkItem
			where T : unmanaged, IJobForEach
		{
			private static InfiniteScaleObjectPool<Executor<T>> Pooled = new InfiniteScaleObjectPool<Executor<T>>();

			public static Executor<T> Get()
			{
				return Pooled.Get();
			}

			private static void Return(Executor<T> executor)
			{
				ArrayPool<Entity>.Shared.Return(executor.Entities, true);
				executor.Entities = default;
				executor.Job = default;
				executor.BatchSize = default;
				executor.Countdown = default;
				Pooled.Return(executor);
			}

			private CountdownEvent Countdown;
			private int BatchSize;
			private Entity[] Entities;
			private T Job;

			public void Create(in T job, Entity[] entities, int batchSize, CountdownEvent countdown)
			{
				Job = job;
				Entities = entities;
				BatchSize = batchSize;
				Countdown = countdown;
			}

			public void Execute()
			{
				for (int i = 0; i < BatchSize; i++)
				{
					//var entity = Entities[i];
					Job.Execute();
				}

				Countdown.Signal();
				Return(this);
			}
		}

		private class Executor<T, TFirst> : IThreadPoolWorkItem
			where T : unmanaged, IJobForEach<TFirst>
			where TFirst : class, IComponent, new()
		{
			private static InfiniteScaleObjectPool<Executor<T, TFirst>> Pooled = new InfiniteScaleObjectPool<Executor<T, TFirst>>();

			public static Executor<T, TFirst> Get()
			{
				return Pooled.Get();
			}

			private static void Return(Executor<T, TFirst> executor)
			{
				ArrayPool<Entity>.Shared.Return(executor.Entities, true);
				executor.Entities = default;
				executor.Job = default;
				executor.BatchSize = default;
				executor.Countdown = default;
				Pooled.Return(executor);
			}

			private CountdownEvent Countdown;
			private int BatchSize;
			private Entity[] Entities;
			private T Job;

			public void Create(in T job, Entity[] entities, int batchSize, CountdownEvent countdown)
			{
				Entities = entities;
				Job = job;
				BatchSize = batchSize;
				Countdown = countdown;
			}

			public void Execute()
			{
				for (int i = 0; i < BatchSize; i++)
				{
					var entity = Entities[i];

					Job.Execute(entity.GetComponent<TFirst>());
				}

				Countdown.Signal();
				Return(this);
			}
		}

		private class Executor<T, TFirst, TSecond> : IThreadPoolWorkItem
			where T : unmanaged, IJobForEach<TFirst, TSecond>
			where TFirst : class, IComponent, new()
			where TSecond : class, IComponent, new()
		{
			private static InfiniteScaleObjectPool<Executor<T, TFirst, TSecond>> Pooled = new InfiniteScaleObjectPool<Executor<T, TFirst, TSecond>>();

			public static Executor<T, TFirst, TSecond> Get()
			{
				return Pooled.Get();
			}

			private static void Return(Executor<T, TFirst, TSecond> executor)
			{
				ArrayPool<Entity>.Shared.Return(executor.Entities, true);
				executor.Entities = default;
				executor.Job = default;
				executor.BatchSize = default;
				executor.Countdown = default;
				Pooled.Return(executor);
			}

			private CountdownEvent Countdown;
			private int BatchSize;
			private Entity[] Entities;
			private T Job;

			public void Create(in T job, Entity[] entities, int batchSize, CountdownEvent countdown)
			{
				Entities = entities;
				Job = job;
				BatchSize = batchSize;
				Countdown = countdown;
			}

			public void Execute()
			{
				for (int i = 0; i < BatchSize; i++)
				{
					var entity = Entities[i];

					Job.Execute(entity.GetComponent<TFirst>(), entity.GetComponent<TSecond>());
				}

				Countdown.Signal();
				Return(this);
			}
		}

	}
}
