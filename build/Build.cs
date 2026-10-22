using System;
using System.Collections.Generic;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

class Build : NukeBuild
{
    [Solution(GenerateProjects = true)] readonly Solution Solution;

    [NuGetPackage("Meziantou.Framework.NuGetPackageValidation.Tool",
        "Meziantou.Framework.NuGetPackageValidation.Tool.dll", Framework = "net8.0")]
    Tool ValidationTool;

    [Parameter] string NugetApiUrl = "https://api.nuget.org/v3/index.json";
    [Parameter] [Secret] string NugetApiKey;

    static readonly string[] PackableProjects =
    [
        "McProtoNet.NBT",
        "McProtoNet.Primitives",
        "McProtoNet.Transport",
        "McProtoNet.Protocol",
        "McProtoNet"
    ];

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsProject => RootDirectory / "tests" / "McProtoNet.Tests" / "McProtoNet.Tests.csproj";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    AbsolutePath NugetDirectory => ArtifactsDirectory / "nuget";

    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    Target Clean => _ => _
        .Executes(() =>
        {
            ArtifactsDirectory.DeleteDirectory();
            DotNetClean(s => s
                .SetProject(Solution)
                .SetConfiguration(Configuration));
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetContinuousIntegrationBuild(true)
                .EnableNoRestore());
        });

    Target Tests => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetRun(s => s
                .SetProjectFile(TestsProject)
                .SetConfiguration(Configuration)
                .EnableNoRestore()
                .EnableNoBuild());
        });

    Target Pack => _ => _
        .DependsOn(Compile)
        .Requires(() => Configuration.Equals(Configuration.Release))
        .Executes(() =>
        {
            NugetDirectory.CreateOrCleanDirectory();
            PackableProjects.ForEach(name => DotNetPack(s => s
                .SetProject(SourceDirectory / name / $"{name}.csproj")
                .SetConfiguration(Configuration)
                .SetNoDependencies(true)
                .EnableNoRestore()
                .EnableNoBuild()
                .SetContinuousIntegrationBuild(true)
                .SetOutputDirectory(NugetDirectory)));
        });

    Target Validation => _ => _
        .DependsOn(Pack)
        .Executes(() =>
        {
            var packages = NugetDirectory.GlobFiles("*.nupkg");
            Assert.NotEmpty(packages, "No packages were produced in artifacts/nuget");

            var environment = new Dictionary<string, string>(EnvironmentInfo.Variables, StringComparer.OrdinalIgnoreCase)
            {
                ["DOTNET_ROLL_FORWARD"] = "LatestMajor"
            };

            packages.ForEach(x => ValidationTool.Invoke(x.ToString(), environmentVariables: environment));
        });

    Target Push => _ => _
        .DependsOn(Tests)
        .DependsOn(Pack)
        .DependsOn(Validation)
        .Requires(() => NugetApiUrl)
        .Requires(() => NugetApiKey)
        .Requires(() => Configuration.Equals(Configuration.Release))
        .Executes(() =>
        {
            NugetDirectory.GlobFiles("*.nupkg")
                .ForEach(x => DotNetNuGetPush(s => s
                    .SetTargetPath(x)
                    .EnableSkipDuplicate()
                    .SetSource(NugetApiUrl)
                    .SetApiKey(NugetApiKey)));
        });
}
