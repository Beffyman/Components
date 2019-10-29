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
				var summary = BenchmarkRunner.Run<EntityManagerJobs>();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}
	}
}
