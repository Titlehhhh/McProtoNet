using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.MinVer;
using Nuke.Common.Utilities.Collections;
using Serilog;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

class Build : NukeBuild
{
    [Solution(GenerateProjects = true)] readonly Solution Solution;

    [NuGetPackage("Meziantou.Framework.NuGetPackageValidation.Tool",
        "Meziantou.Framework.NuGetPackageValidation.Tool.dll", Framework = "net8.0")]
    Tool ValidationTool;

    [Parameter] string NugetApiUrl = "https://api.nuget.org/v3/index.json";
    [Parameter] string NugetApiKey;


    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    AbsolutePath NugetDirectory => ArtifactsDirectory / "nuget";
    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            ArtifactsDirectory.DeleteDirectory();
            DotNetClean(s => s
                .SetProject(Solution)
                .SetConfiguration(Configuration));
        });

    Target Tests => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            DotNetTest(x =>
                x.SetProjectFile(Solution.tests.McProtoNet_Tests)
                    .SetNoRestore(true)
                    .SetConfiguration(Configuration));
        });

    Target Validation => _ => _
        .DependsOn(Pack)
        .Executes(() =>
        {
            NugetDirectory.GlobFiles("*.nupkg")
                .ForEach(x =>
                {
                    ValidationTool.Invoke(x.ToString());
                });
        });


    Target Pack => _ => _
        .DependsOn(Clean)
        .DependsOn(Compile)
        .Requires(() => Configuration.Equals(Configuration.Release))
        .Executes(() =>
        {
            PackCore(Solution.src.McProtoNet_NBT);
            PackCore(Solution.src.McProtoNet_Utils);
            PackCore(Solution.src.McProtoNet_Serialization);
            PackCore(Solution.src.McProtoNet_Abstractions);
            PackCore(Solution.src.McProtoNet);
            PackCore(Solution.src.McProtoNet_Protocol);
            return;

            void PackCore(Project project)
            {
                DotNetPack(s => s
                    .SetProject(project)
                    .SetConfiguration(Configuration)
                    .SetNoDependencies(true)
                    .SetContinuousIntegrationBuild(true)
                    .SetOutputDirectory(NugetDirectory));
            }
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(_ => _
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(x => x
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration));
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
                .ForEach(x =>
                {
                    DotNetNuGetPush(s => s
                        .SetTargetPath(x)
                        .EnableSkipDuplicate()
                        .SetSource(NugetApiUrl)
                        .SetApiKey(NugetApiKey)
                    );
                });
        });
}