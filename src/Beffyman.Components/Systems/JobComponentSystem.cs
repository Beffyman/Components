using System;
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
		//#error Figure out a way to have reduced allocation job queueing?
		//#error Possiblty expose a Get method which pools the job?
		//#error I don't think it's possible to use the structs as

		protected void Execute<T>(in T job) where T : unmanaged, IJobForEach
		{
			if (Manager._entities.Count == 0)
			{
				return;
			}

			if (Manager.Options.Multithreading)
			{
				int entityCount = Manager._entities.Count;
				int batchSize = (int)Math.Ceiling((float)entityCount / Environment.ProcessorCount);
				int batches = (int)Math.Ceiling((float)entityCount / batchSize);

				using CountdownEvent waitLock = new CountdownEvent(batches);

				var executor = Executor<T>.Get();
				var array = ArrayPool<Entity>.Shared.Rent(batchSize);
				int index = 0;

				//Loop through entities
				foreach (var entity in Manager._entities)
				{
					//If we haven't reached the batch size yet, just increment, store the entity and keep looping
					if (index < batchSize)
					{
						array[index] = entity;
						index++;
					}
					else
					{
						//If we reached the batch size limit, fill the executor
						executor.Create(job, array, batchSize, waitLock);
						//Queue a new thread with the batch
						_ = ThreadPool.UnsafeQueueUserWorkItem(executor, true);

						//Reset the loop
						array = ArrayPool<Entity>.Shared.Rent(batchSize);
						index = 0;
						executor = Executor<T>.Get();
					}
				}

				//If we have an uneven batch, queue the last entities manually
				if (index != 0)
				{
					executor.Create(job, array, index, waitLock);
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

				while (waitLock.CurrentCount > 0)
				{
					Thread.Sleep(0);
				}
			}
			else
			{
				foreach (var entity in Manager._entities)
				{
					job.Execute();
				}
			}
		}

		protected void Execute<T, TFirst>(in T job) where T : unmanaged, IJobForEach<TFirst>
			where TFirst : class, IComponent, new()
		{
			if (Manager._entities.Count == 0)
			{
				return;
			}

			if (Manager.Options.Multithreading)
			{
				int entityCount = Manager._entities.Count;
				int batchSize = (int)Math.Ceiling((float)entityCount / Environment.ProcessorCount);
				int batches = (int)Math.Ceiling((float)entityCount / batchSize);

				using CountdownEvent waitLock = new CountdownEvent(batches);


				var executor = Executor<T, TFirst>.Get();
				var array = ArrayPool<Entity>.Shared.Rent(batchSize);
				int index = 0;

				//Loop through entities
#warning Need to implement ArcheType searches for entities
				foreach (var entity in Manager._entities)
				{
					//If we haven't reached the batch size yet, just increment, store the entity and keep looping
					if (index < batchSize)
					{
						array[index] = entity;
						index++;
					}
					else
					{
						//If we reached the batch size limit, fill the executor
						executor.Create(job, array, batchSize, waitLock);
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
					executor.Create(job, array, index, waitLock);
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

				while (waitLock.CurrentCount > 0)
				{
					Thread.Sleep(0);
				}
			}
			else
			{
#warning Need to implement ArcheType searches for entities
				foreach (var entity in Manager._entities)
				{
					job.Execute(entity.GetComponent<TFirst>());
				}
			}
		}

		protected void Execute<T, TFirst, TSecond>(in T job) where T : unmanaged, IJobForEach<TFirst, TSecond>
			where TFirst : class, IComponent, new()
			where TSecond : class, IComponent, new()
		{
			if(Manager._entities.Count== 0)
			{
				return;
			}

			if (Manager.Options.Multithreading)
			{
				int entityCount = Manager._entities.Count;
				int batchSize = (int)Math.Ceiling((float)entityCount / Environment.ProcessorCount);
				int batches = (int)Math.Ceiling((float)entityCount / batchSize);

				using CountdownEvent waitLock = new CountdownEvent(batches);

				var executor = Executor<T, TFirst, TSecond>.Get();
				var array = ArrayPool<Entity>.Shared.Rent(batchSize);
				int index = 0;

				//Loop through entities
#warning Need to implement ArcheType searches for entities
				foreach (var entity in Manager._entities)
				{
					//If we haven't reached the batch size yet, just increment, store the entity and keep looping
					if (index < batchSize)
					{
						array[index] = entity;
						index++;
					}
					else
					{
						//If we reached the batch size limit, fill the executor
						executor.Create(job, array, batchSize, waitLock);
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
					executor.Create(job, array, index, waitLock);
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

				while (waitLock.CurrentCount > 0)
				{
					Thread.Sleep(0);
				}
			}
			else
			{
#warning Need to implement ArcheType searches for entities
				foreach (var entity in Manager._entities)
				{
					job.Execute(entity.GetComponent<TFirst>(), entity.GetComponent<TSecond>());
				}
			}
		}


		protected abstract void OnUpdate(in UpdateStep step);

		internal override void Update(in UpdateStep step)
		{
			OnUpdate(step);

			//Do Work for the jobs here, queue up threads and then sync them up
		}
	}
}
