using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.CsProj;
using BenchmarkDotNet.Toolchains.DotNetCli;

namespace Beffyman.Components.Benchmarks
{
	internal sealed class DefaultConfig : ManualConfig
	{

		internal const float DELTATIME = (1f / 60f);
		internal const int PREPARE_ENTITIES = 100_000;
		internal const int PREPARE_LOOPS = 100;
		internal const int TEST_LOOPS = 100_000;

		public DefaultConfig()
		{
			var customToolchain = CsProjCoreToolchain.From(NetCoreAppSettings.NetCoreApp30.WithCustomDotNetCliPath(Environment.GetEnvironmentVariable("CUSTOM_SDK_PATH")));

			Add(Job.Default.With(CoreRuntime.Core30).With(customToolchain));
		}
	}
}
