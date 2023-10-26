using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

class Build : NukeBuild
{
	[Solution] readonly Solution Solution;
	[GitRepository] readonly GitRepository GitRepository;
	[GitVersion] readonly GitVersion GitVersion;

	[Parameter] string NugetApiUrl = "https://api.nuget.org/v3/index.json";
	[Parameter] string NugetApiKey;

	public static int Main() => Execute<Build>(x => x.Pack);

	[Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
	readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

	AbsolutePath SourceDirectory => RootDirectory / "src";
	AbsolutePath TestsDirectory => RootDirectory / "tests";
	AbsolutePath AnalyzerDirectory => RootDirectory / "analyzers";
	AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
	AbsolutePath NugetDirectory => ArtifactsDirectory / "nuget";
	Target Clean => _ => _
		.Before(Restore)
		.Executes(() =>
		{
		});


	Target Restore => _ => _
		.Executes(() =>
		{
			DotNetRestore(_ => _
			   .SetProjectFile(Solution));
		});

	Target Compile => _ => _
		.DependsOn(Restore)
		.Executes(() =>
		{
			DotNetBuild(_ => _
			   .SetProjectFile(Solution)
			   .SetConfiguration(Configuration)
			   .SetAssemblyVersion(GitVersion.AssemblySemVer)
			   .SetFileVersion(GitVersion.AssemblySemFileVer)
			   .SetInformationalVersion(GitVersion.InformationalVersion)
			   .EnableNoRestore());
		});



	Target Pack => _ => _
	  .DependsOn(Compile)
	  .Executes(() =>
	  {

		  NugetDirectory.DeleteDirectory();

		  //string json = System.Text.Json.JsonSerializer.Serialize(this.GitVersion, new JsonSerializerOptions
		  //{
		  //  WriteIndented = true
		  //});

		  //using var sw = new StreamWriter("version.json");
		  //sw.WriteLine(json);


		  //if (Int32.TryParse(GitVersion.CommitsSinceVersionSource, out commitNum))
		  // NuGetVersionCustom = commitNum > 0 ? NuGetVersionCustom + $"{commitNum}" : NuGetVersionCustom;


		  string NuGetVersionCustom = "1.8.0";
		  int commitNum = 0;
		  if (Int32.TryParse(GitVersion.CommitsSinceVersionSource, out commitNum))
		  {

		  }

		  NuGetVersionCustom = NuGetVersionCustom + "-" + "experimental." + commitNum;




		  DotNetPack(s => s
			.SetProject(Solution.GetProject("McProtoNet"))
			.SetConfiguration(Configuration)
			.EnableNoBuild()
			.EnableNoRestore()
			.SetVersion(NuGetVersionCustom)
			.SetAuthors("Titlehhhh")
			.SetNoDependencies(true)
			.SetOutputDirectory(ArtifactsDirectory / "nuget"));

		  DotNetPack(s => s
				.SetProject(Solution.GetProject("McProtoNet.Core"))
				.SetConfiguration(Configuration)
				.EnableNoBuild()
				.SetAuthors("Titlehhhh")
				.EnableNoRestore()
				.SetVersion(NuGetVersionCustom)
				.SetNoDependencies(true)
				.SetOutputDirectory(ArtifactsDirectory / "nuget"));


		  DotNetPack(s => s
				.SetProject(Solution.GetProject("McProtoNet.NBT"))
				.SetConfiguration(Configuration)
				.EnableNoBuild()
				.SetAuthors("Titlehhhh")
				.EnableNoRestore()
				.SetVersion(NuGetVersionCustom)
				.SetNoDependencies(true)
				.SetOutputDirectory(ArtifactsDirectory / "nuget"));


		  DotNetPack(s => s
				.SetProject(Solution.GetProject("McProtoNet.Utils"))
				.SetConfiguration(Configuration)
				.EnableNoBuild()
				.SetAuthors("Titlehhhh")
				.EnableNoRestore()
				.SetVersion(NuGetVersionCustom)
				.SetNoDependencies(true)
				.SetOutputDirectory(ArtifactsDirectory / "nuget"));

	  });


	Target LocalNuget => _ => _
	  .DependsOn(Pack)
	  .Requires(() => Configuration.Equals(Configuration.Debug))
	  .Executes(() =>
	  {
		  NugetDirectory.GlobFiles("*.nupkg")
				  .NotEmpty()
				  // .Where(x => !x.EndsWith("symbols.nupkg"))
				  .ForEach(x =>
				  {
					  DotNetNuGetPush(s => s
						  .SetTargetPath(x)
						  .SetSource("I:\\LocalNuget")
					  );
				  });
	  });


	Target Push => _ => _
	  .DependsOn(Pack)
	  .Requires(() => NugetApiUrl)
	  .Requires(() => NugetApiKey)
	  .Requires(() => Configuration.Equals(Configuration.Release))
	  .Executes(() =>
	  {
		  NugetDirectory.GlobFiles("*.nupkg")
				  .NotEmpty()
				  .ForEach(x =>
				  {
					  DotNetNuGetPush(s => s
						  .SetTargetPath(x)
						  .SetSource(NugetApiUrl)
						  .SetApiKey(NugetApiKey)
					  );
				  });
	  });


}
