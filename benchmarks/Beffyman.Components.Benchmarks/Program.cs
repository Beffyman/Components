using System;
using BenchmarkDotNet.Running;

namespace Beffyman.Components.Benchmarks
{
	public static class Program
	{
		public static void Main()
		{
			try
			{
				var summary1 = BenchmarkRunner.Run<ComponentSystemBenchmarks>();
				var summary2 = BenchmarkRunner.Run<JobComponentSystemBenchmarks>();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}
	}
}
