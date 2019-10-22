using System.Collections.Generic;
using Nuke.Common;
using Nuke.Common.CI.AzurePipelines;
using Nuke.Common.CI.AzurePipelines.Configuration;
using Nuke.Common.Execution;
using Nuke.Common.Tooling;

partial class Build
{
	public class AzurePipelinesAttribute : Nuke.Common.CI.AzurePipelines.AzurePipelinesAttribute
	{
		public AzurePipelinesAttribute(AzurePipelinesImage image, params AzurePipelinesImage[] images)
			: base(image, images)
		{
		}

		protected override AzurePipelinesJob GetJob(ExecutableTarget executableTarget, LookupTable<ExecutableTarget, AzurePipelinesJob> jobs)
		{
			var job = base.GetJob(executableTarget, jobs);

			var dictionary = new Dictionary<string, string>
							 {
								 { nameof(CI), "☁️" },
								 { nameof(Clean), "🗑️" },
								 { nameof(Restore), "⬇️" },
								 { nameof(Compile), "⚙️" },
								 { nameof(Pack), "📦" },
								 { nameof(Test), "🚦" },
								 { nameof(Report), "📄" },
								 { nameof(Performance), "⏲️" },
							 };

			var symbol = dictionary.GetValueOrDefault(job.Name).NotNull("symbol != null");
			job.DisplayName = job.PartitionName == null
				? $"{symbol} {job.DisplayName}"
				: $"{symbol} {job.DisplayName} 🧩";
			return job;
		}
	}
}
