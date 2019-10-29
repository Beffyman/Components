using System;
using System.Reflection;
using Beffyman.Components.Manager;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.CsProj;
using BenchmarkDotNet.Toolchains.DotNetCli;

namespace Beffyman.Components.Benchmarks
{
	[Config(typeof(Config))]
	[MemoryDiagnoser]
	public class EntityManagerJobs
	{
		internal sealed class Config : ManualConfig
		{
			public Config()
			{
				var customToolchain = CsProjCoreToolchain.From(NetCoreAppSettings.NetCoreApp30.WithCustomDotNetCliPath(Environment.GetEnvironmentVariable("CUSTOM_SDK_PATH")));

				Add(Job.Default.With(CoreRuntime.Core30).With(customToolchain));
			}
		}

		private EntityManager Manager;

		[Params(false, true)]
		public bool MultiThreading { get; set; }

		[IterationSetup]
		public void Setup()
		{
			Manager = new EntityManager(new EntityManagerOptions
			{
				ComponentSystemAssemblies = new Assembly[] { typeof(EntityManagerJobs).Assembly },
				Multithreading = MultiThreading
			});
		}

		[IterationCleanup]
		public void Cleanup()
		{
			Manager.Destroy();
			Manager = null;
		}

		[Benchmark]
		public void SystemPerformance()
		{

		}
	}
}
