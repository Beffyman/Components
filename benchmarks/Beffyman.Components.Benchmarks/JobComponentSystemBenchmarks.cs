using System;
using System.Reflection;
using Beffyman.Components.Benchmarks.Systems;
using Beffyman.Components.Manager;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.CsProj;
using BenchmarkDotNet.Toolchains.DotNetCli;

namespace Beffyman.Components.Benchmarks
{
	[Config(typeof(DefaultConfig))]
	[MemoryDiagnoser]
	public class JobComponentSystemBenchmarks
	{

		private EntityManager Manager;

		[Params(false, true)]
		public bool MultiThreading { get; set; }

		[IterationSetup]
		public void Setup()
		{
			Manager = new EntityManager(new EntityManagerOptions
			{
				ComponentSystemTypes = new Type[] { typeof(DoWorkJobSystem) },
				Multithreading = MultiThreading,
				InitialPoolSize = 500
			});

			//Do a spin up to allocate shared resources
			for (int i = 0; i < DefaultConfig.PREPARE_ENTITIES; i++)
			{
				Manager.CreateEntity();
			}

			for (int i = 0; i < DefaultConfig.PREPARE_LOOPS; i++)
			{
				Manager.Update(new UpdateStep(DefaultConfig.DELTATIME));
			}
		}

		[IterationCleanup]
		public void Cleanup()
		{
			Manager.Destroy();
			Manager = null;
		}

		[Benchmark]
		public void JobComponentSystemAllocations()
		{
			for (int i = 0; i < DefaultConfig.TEST_LOOPS; i++)
			{
				Manager.Update(new UpdateStep(DefaultConfig.DELTATIME));
			}
		}
	}
}
