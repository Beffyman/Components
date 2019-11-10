using System;
using System.IO;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.AzurePipelines;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.ReportGenerator;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.ReportGenerator.ReportGeneratorTasks;

[CheckBuildProjectConfigurations]
[DotNetVerbosityMapping]
[UnsetVisualStudioEnvironmentVariables]
[AzurePipelines(
	AzurePipelinesImage.WindowsLatest,
	AzurePipelinesImage.UbuntuLatest,
	//AzurePipelinesImage.MacOsLatest,
	InvokedTargets = new[] { nameof(Pack), nameof(Report), nameof(Performance) },
	ExcludedTargets = new string[] { nameof(Clean) },
	NonEntryTargets = new string[] { nameof(Restore), nameof(Test), nameof(Label) })]
public partial class Build : NukeBuild
{
	public static int Main() => Execute<Build>(x => x.Compile);

	[Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
	readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

	[Solution] readonly Solution Solution;
	[GitRepository] readonly GitRepository GitRepository;
	[GitVersion] readonly GitVersion GitVersion;

	AbsolutePath SourceDirectory => RootDirectory / "src";
	AbsolutePath TestsDirectory => RootDirectory / "tests";
	AbsolutePath SamplesDirectory => RootDirectory / "samples";
	AbsolutePath BenchmarksDirectory => RootDirectory / "benchmarks";

	string BenchmarksProjectName => "Beffyman.Components.Benchmarks";
	AbsolutePath BenchmarksProjectDirectory => BenchmarksDirectory / BenchmarksProjectName;
	AbsolutePath BenchmarksProject => BenchmarksProjectDirectory / $"{BenchmarksProjectName}.csproj";
	AbsolutePath BenchmarksProjectArtifactsDirectory => BenchmarksProjectDirectory / "BenchmarkDotNet.Artifacts";

	AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
	//AbsolutePath NugetArtifactsDirectory => ArtifactsDirectory / "Nuget";
	//AbsolutePath TestArtifactsDirectory => ArtifactsDirectory / "Tests";
	AbsolutePath PerformanceArtifactsDirectory => ArtifactsDirectory / "Performance";


	string CodeCoverageFile => "coverage.cobertura.xml";
	int CodeCoverageRequirement => 90;
	string ReportsOutput => "Reports";


	Target Label => _ => _
		.Executes(() =>
		{
			AzurePipelines.Instance?.UpdateBuildNumber(GitVersion.NuGetVersionV2);
		});

	Target Clean => _ => _
		.Before(Restore)
		.Executes(() =>
		{
			SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
			SamplesDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
			BenchmarksDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
			TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
			EnsureCleanDirectory(ArtifactsDirectory);
		});

	Target Restore => _ => _
		.After(Label)
		.Executes(() =>
		{
			DotNetRestore(s => s
				.SetProjectFile(Solution));
		});

	Target Compile => _ => _
		.DependsOn(Label)
		.DependsOn(Restore)
		.Executes(() =>
		{
			DotNetBuild(s => s
				.SetProjectFile(Solution)
				.SetConfiguration(Configuration)
				.SetAssemblyVersion(GitVersion.AssemblySemVer)
				.SetFileVersion(GitVersion.AssemblySemFileVer)
				.SetInformationalVersion(GitVersion.InformationalVersion)
				.EnableNoRestore());
		});

	Target Pack => _ => _
		.DependsOn(Label)
		.DependsOn(Compile)
		.Produces(ArtifactsDirectory / "*nupkg")
		.Produces(ArtifactsDirectory / "*snupkg")
		.Executes(() =>
		{
			//EnsureExistingDirectory(NugetArtifactsDirectory);
			EnsureExistingDirectory(ArtifactsDirectory);

			DotNetPack(s => s.SetProject(Solution)
					.SetVersion(GitVersion.NuGetVersionV2)
					.SetNoBuild(AzurePipelines.Instance == null)
					.EnableIncludeSource()
					.EnableIncludeSymbols()
					.SetConfiguration(Configuration)
					.SetAssemblyVersion(GitVersion.AssemblySemVer)
					.SetFileVersion(GitVersion.AssemblySemFileVer)
					.SetInformationalVersion(GitVersion.InformationalVersion)
					.SetOutputDirectory(ArtifactsDirectory));
		});


	Target Test => _ => _
		.After(Label)
		.DependsOn(Compile)
		.Produces(ArtifactsDirectory / "*.trx")
		.Produces(ArtifactsDirectory / CodeCoverageFile)
		//.Produces(TestArtifactsDirectory / "*.trx")
		//.Produces(TestArtifactsDirectory / CodeCoverageFile)
		.Executes(() =>
		{
			//EnsureExistingDirectory(TestArtifactsDirectory);
			EnsureExistingDirectory(ArtifactsDirectory);

			TestsDirectory.GlobFiles("**/*.csproj").ForEach(csproj =>
			{
				var projectName = Path.GetFileNameWithoutExtension(csproj);
				AbsolutePath coverageOutput = ArtifactsDirectory / $"{projectName}";
				//AbsolutePath coverageOutput = TestArtifactsDirectory / $"{projectName}";

				DotNetTest(s => s.SetConfiguration(Configuration)
					.SetNoBuild(AzurePipelines.Instance == null)
					.SetLogger("trx")
					//.SetResultsDirectory(TestArtifactsDirectory)
					.SetResultsDirectory(ArtifactsDirectory)
					.SetArgumentConfigurator(arguments =>
						arguments.Add("/p:CollectCoverage={0}", "true")
								.Add("/p:CoverletOutput={0}/", coverageOutput)
								//.Add("/p:Threshold={0}", CodeCoverageRequirement)
								.Add("/p:Exclude=\"[xunit*]*%2c[*.Tests]*\"")
								.Add("/p:UseSourceLink={0}", "true")
								.Add("/p:CoverletOutputFormat={0}", "cobertura"))
					.SetProjectFile(csproj));

				FileExists(coverageOutput);
			});

			//var trxFiles = TestArtifactsDirectory.GlobFiles("*.trx").Select(x => new FileInfo(x));

			AzurePipelines.Instance?.PublishTestResults(AzurePipelines.Instance?.BuildNumber, "XUnit", ArtifactsDirectory.GlobFiles("*.trx").Select(x => x.ToString()), mergeResults: true);

		});

	Target Report => _ => _
		.DependsOn(Label)
		.DependsOn(Compile)
		.DependsOn(Test)
		.Executes(() =>
		{
			var coverageFiles = TestsDirectory.GlobFiles("**/*.csproj")
				//.Select(csproj => (string)(TestArtifactsDirectory / $"{Path.GetFileNameWithoutExtension(csproj)}"))
				.Select(csproj => (string)(ArtifactsDirectory / $"{Path.GetFileNameWithoutExtension(csproj)}"))
				.ToArray();

			foreach (var coverageFolder in coverageFiles)
			{
				var reportFolder = Path.Combine(coverageFolder, ReportsOutput);
				var coverage = Path.Combine(coverageFolder, CodeCoverageFile);

				ReportGenerator(s => s.SetReports(coverage)
							.SetTargetDirectory(reportFolder)
							.SetFramework("netcoreapp3.0")
							.SetTag(GitVersion.NuGetVersionV2)
							.SetReportTypes(ReportTypes.HtmlInline_AzurePipelines_Dark));
			}
		});

	Target Performance => _ => _
		.DependsOn(Label)
		.DependsOn(Clean)
		.DependsOn(Compile)
		.Produces(PerformanceArtifactsDirectory)
		.Executes(() =>
		{
			EnsureExistingDirectory(PerformanceArtifactsDirectory);

			DotNetRun(s => s.SetConfiguration(Configuration.Release)
				.SetWorkingDirectory(BenchmarksProjectDirectory)
				.AddEnvironmentVariable("CUSTOM_SDK_PATH", DotNetPath)
				.SetProjectFile(BenchmarksProject));

			CopyDirectoryRecursively(BenchmarksProjectArtifactsDirectory, PerformanceArtifactsDirectory, Nuke.Common.IO.DirectoryExistsPolicy.Merge, Nuke.Common.IO.FileExistsPolicy.Overwrite);
		});
}
