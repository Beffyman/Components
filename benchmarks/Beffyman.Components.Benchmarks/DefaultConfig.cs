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
		public DefaultConfig()
		{
			var customToolchain = CsProjCoreToolchain.From(NetCoreAppSettings.NetCoreApp30.WithCustomDotNetCliPath(Environment.GetEnvironmentVariable("CUSTOM_SDK_PATH")));

			Add(Job.Default.With(CoreRuntime.Core30).With(customToolchain));
		}
	}
}
