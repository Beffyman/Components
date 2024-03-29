﻿using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Beffyman.Components.Internal;
using Beffyman.Components.Manager;

namespace Beffyman.Components.Systems
{
	/// <summary>
	/// Defines a system that will run once per entity that matches the job's generic arguments if it is scheduled this frame
	/// </summary>
	public abstract partial class JobComponentSystem : ComponentSystemBase
	{
		//protected void Execute<T>(in T job) where T : unmanaged, IJobForEach
		//{
		//	if (Manager._entities.Count == 0)
		//	{
		//		return;
		//	}

		//	var archeType = ArcheType.Empty;
		//	var entityCount = Manager.GetArcheTypeEntityCount(archeType);

		//	if (entityCount == 0)
		//	{
		//		return;
		//	}

		//	var entityArray = ArrayPool<Entity>.Shared.Rent(entityCount);

		//	if (Manager.Options.Multithreading)
		//	{
		//		int entityCount = Manager._entities.Count;
		//		int batchSize = (int)Math.Ceiling((float)entityCount / Environment.ProcessorCount);
		//		int batches = (int)Math.Ceiling((float)entityCount / batchSize);

		//		PooledCountdownEvent waitLock = PooledCountdownEvent.Get(batches);

		//		var executor = Executor<T>.Get();
		//		var array = ArrayPool<Entity>.Shared.Rent(batchSize);
		//		int index = 0;

		//		//Loop through entities
		//		foreach (var entity in Manager._entities)
		//		{
		//			//If we haven't reached the batch size yet, just increment, store the entity and keep looping
		//			if (index < batchSize)
		//			{
		//				array[index] = entity;
		//				index++;
		//			}
		//			else
		//			{
		//				//If we reached the batch size limit, fill the executor
		//				executor.Create(job, array, batchSize, waitLock);
		//				//Queue a new thread with the batch
		//				_ = ThreadPool.UnsafeQueueUserWorkItem(executor, true);

		//				//Reset the loop
		//				array = ArrayPool<Entity>.Shared.Rent(batchSize);
		//				index = 0;
		//				executor = Executor<T>.Get();
		//			}
		//		}

		//		//If we have an uneven batch, queue the last entities manually
		//		if (index != 0)
		//		{
		//			executor.Create(job, array, index, waitLock);
		//			//Queue a new thread with the batch
		//			_ = ThreadPool.UnsafeQueueUserWorkItem(executor, true);

		//		}
		//		else
		//		{
		//			//Cleanup
		//			ArrayPool<Entity>.Shared.Return(array);
		//			executor.Execute();
		//			executor = null;
		//		}

		//		waitLock.SleepSpinWaitReturn();
		//	}
		//	else
		//	{
		//		foreach (var entity in Manager._entities)
		//		{
		//			job.Execute();
		//		}
		//	}

		//	ArrayPool<Entity>.Shared.Return(entityArray, true);
		//}

		protected void Execute<T, TFirst>(in T job) where T : unmanaged, IJobForEach<TFirst>
			where TFirst : class, IComponent, new()
		{
			if (Manager._entities.Count == 0)
			{
				return;
			}

			var componentTypes = ArrayPool<Type>.Shared.Rent(1);
			componentTypes[0] = typeof(TFirst);

			var archeType = Manager.GetArcheType(componentTypes);
			var entityCount = Manager.GetArcheTypeEntityCount(archeType);

			if (entityCount == 0)
			{
				return;
			}

			var entityArray = ArrayPool<Entity>.Shared.Rent(entityCount);

			Manager.GetEntities(archeType, entityArray);

			if (Manager.Options.Multithreading)
			{
				int batchSize = (int)Math.Ceiling((float)entityCount / Environment.ProcessorCount);
				int batches = (int)Math.Ceiling((float)entityCount / batchSize);

				PooledCountdownEvent waitLock = PooledCountdownEvent.Get(batches);


				var executor = Executor<T, TFirst>.Get();
				var array = ArrayPool<Entity>.Shared.Rent(batchSize);
				int index = 0;

				//Loop through entities

				for (int i = 0; i < entityCount; i++)
				{
					//If we haven't reached the batch size yet, just increment, store the entity and keep looping
					if (index < batchSize)
					{
						array[index] = entityArray[i];
						index++;
					}
					else
					{
						//If we reached the batch size limit, fill the executor
						executor.Create(job, archeType, array, batchSize, waitLock);
						//Queue a new thread with the batch
						_ = ThreadPool.UnsafeQueueUserWorkItem(executor, true);

						//Reset the loop
						array = ArrayPool<Entity>.Shared.Rent(batchSize);
						index = 0;
						executor = Executor<T, TFirst>.Get();
					}
				}

				//If we have an uneven batch, queue the last entities manually
				if (index != 0)
				{
					executor.Create(job, archeType, array, index, waitLock);
					//Queue a new thread with the batch
					_ = ThreadPool.UnsafeQueueUserWorkItem(executor, true);

				}
				else
				{
					//Cleanup
					ArrayPool<Entity>.Shared.Return(array);
					executor.Execute();
					executor = null;
				}

				waitLock.SleepSpinWaitReturn();
			}
			else
			{
				for (int i = 0; i < entityCount; i++)
				{
					job.Execute(entityArray[i].GetComponent<TFirst>());
				}
			}

			ArrayPool<Entity>.Shared.Return(entityArray, true);
			ArrayPool<Type>.Shared.Return(componentTypes, true);
		}

		protected void Execute<T, TFirst, TSecond>(in T job) where T : unmanaged, IJobForEach<TFirst, TSecond>
			where TFirst : class, IComponent, new()
			where TSecond : class, IComponent, new()
		{
			if (Manager._entities.Count == 0)
			{
				return;
			}

			var componentTypes = ArrayPool<Type>.Shared.Rent(2);
			componentTypes[0] = typeof(TFirst);
			componentTypes[1] = typeof(TSecond);

			var archeType = Manager.GetArcheType(componentTypes);
			var entityCount = Manager.GetArcheTypeEntityCount(archeType);

			if (entityCount == 0)
			{
				return;
			}

			var entityArray = ArrayPool<Entity>.Shared.Rent(entityCount);

			if (Manager.Options.Multithreading)
			{
				int batchSize = (int)Math.Ceiling((float)entityCount / Environment.ProcessorCount);
				int batches = (int)Math.Ceiling((float)entityCount / batchSize);

				PooledCountdownEvent waitLock = PooledCountdownEvent.Get(batches);

				var executor = Executor<T, TFirst, TSecond>.Get();
				var array = ArrayPool<Entity>.Shared.Rent(batchSize);
				int index = 0;

				//Loop through entities
				for (int i = 0; i < entityCount; i++)
				{
					//If we haven't reached the batch size yet, just increment, store the entity and keep looping
					if (index < batchSize)
					{
						array[index] = entityArray[i];
						index++;
					}
					else
					{
						//If we reached the batch size limit, fill the executor
						executor.Create(job, archeType, array, batchSize, waitLock);
						//Queue a new thread with the batch
						_ = ThreadPool.UnsafeQueueUserWorkItem(executor, true);

						//Reset the loop
						array = ArrayPool<Entity>.Shared.Rent(batchSize);
						index = 0;
						executor = Executor<T, TFirst, TSecond>.Get();
					}
				}

				//If we have an uneven batch, queue the last entities manually
				if (index != 0)
				{
					executor.Create(job, archeType, array, index, waitLock);
					//Queue a new thread with the batch
					_ = ThreadPool.UnsafeQueueUserWorkItem(executor, true);
				}
				else
				{
					//Cleanup
					ArrayPool<Entity>.Shared.Return(array);
					executor.Execute();
					executor = null;
				}

				waitLock.SleepSpinWaitReturn();
			}
			else
			{
				for (int i = 0; i < entityCount; i++)
				{
					job.Execute(entityArray[i].GetComponent<TFirst>(), entityArray[i].GetComponent<TSecond>());
				}
			}

			ArrayPool<Entity>.Shared.Return(entityArray, true);
			ArrayPool<Type>.Shared.Return(componentTypes, true);
		}


		protected virtual void OnUpdate(in UpdateStep step) { }

		internal override void Update(in UpdateStep step)
		{
			OnUpdate(step);

			//Do Work for the jobs here, queue up threads and then sync them up
		}

		protected virtual void OnFixedUpdate(in UpdateStep step) { }

		internal override void FixedUpdate(in UpdateStep step)
		{
			OnFixedUpdate(step);

			//Do Work for the jobs here, queue up threads and then sync them up
		}
	}
}
