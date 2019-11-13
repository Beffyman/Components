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
		private abstract class Executor
		{
			protected ArcheType ArcheType;
			protected CountdownEvent Countdown;
			protected int BatchSize;
			protected Entity[] Entities;

			protected void SetToDefault()
			{
				ArcheType = default;
				Entities = default;
				BatchSize = default;
				Countdown = default;
			}

		}


		//private class Executor<T> : Executor, IThreadPoolWorkItem
		//	where T : unmanaged, IJobForEach
		//{
		//	private static InfiniteScaleObjectPool<Executor<T>> Pooled = new InfiniteScaleObjectPool<Executor<T>>();

		//	public static Executor<T> Get()
		//	{
		//		return Pooled.Get();
		//	}

		//	private static void Return(Executor<T> executor)
		//	{
		//		ArrayPool<Entity>.Shared.Return(executor.Entities, true);
		//		executor.SetToDefault();
		//		executor.Job = default;
		//		Pooled.Return(executor);
		//	}

		//	private T Job;

		//	public void Create(in T job, ArcheType archetype, Entity[] entities, int batchSize, CountdownEvent countdown)
		//	{
		//		ArcheType = archetype;
		//		Job = job;
		//		Entities = entities;
		//		BatchSize = batchSize;
		//		Countdown = countdown;
		//	}

		//	public void Execute()
		//	{
		//		for (int i = 0; i < BatchSize; i++)
		//		{
		//			//var entity = Entities[i];
		//			Job.Execute();
		//		}

		//		Countdown.Signal();
		//		Return(this);
		//	}
		//}

		private class Executor<T, TFirst> : Executor, IThreadPoolWorkItem
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
				executor.SetToDefault();
				executor.Job = default;
				Pooled.Return(executor);
			}

			private T Job;

			public void Create(in T job, ArcheType archetype, Entity[] entities, int batchSize, CountdownEvent countdown)
			{
				ArcheType = archetype;
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

					var components = entity.GetEntityComponentsByArcheType(ArcheType);

					Job.Execute(components[typeof(TFirst)] as TFirst);
				}

				Countdown.Signal();
				Return(this);
			}
		}

		private class Executor<T, TFirst, TSecond> : Executor, IThreadPoolWorkItem
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
				executor.SetToDefault();
				executor.Job = default;
				Pooled.Return(executor);
			}

			private T Job;

			public void Create(in T job, ArcheType archetype, Entity[] entities, int batchSize, CountdownEvent countdown)
			{
				ArcheType = archetype;
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

					var components = entity.GetEntityComponentsByArcheType(ArcheType);

					Job.Execute(components[typeof(TFirst)] as TFirst, components[typeof(TSecond)] as TSecond);
				}

				Countdown.Signal();
				Return(this);
			}
		}

	}
}
