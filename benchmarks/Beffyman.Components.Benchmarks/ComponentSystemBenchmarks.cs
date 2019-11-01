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
	public class ComponentSystemBenchmarks
	{
		private const float DeltaTime = (1f / 30f);

		private EntityManager Manager;

		[Params(false, true)]
		public bool MultiThreading { get; set; }

		[IterationSetup]
		public void Setup()
		{
			Manager = new EntityManager(new EntityManagerOptions
			{
				ComponentSystemTypes = new Type[] { typeof(DoWorkSystem) },
				Multithreading = MultiThreading,
				InitialPoolSize = 500
			});

			//Do a spin up to allocate shared resources
			for (int i = 0; i < 10; i++)
			{
				Manager.Update(new UpdateStep(DeltaTime));
			}
		}

		[IterationCleanup]
		public void Cleanup()
		{
			Manager.Destroy();
			Manager = null;
		}

		[Benchmark]
		public void ComponentSystemAllocations()
		{
			for (int i = 0; i < 10000; i++)
			{
				Manager.Update(new UpdateStep(DeltaTime));
			}
		}
	}
}
