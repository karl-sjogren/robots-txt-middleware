var target = Argument("target", "build");
var output = Argument("output", "./artifacts");
var framework = Argument("framework", "");
var versionSuffix = Argument<string>("versionSuffix", null);

Task("clean")
    .Does(() => {
        CleanDirectories("./src/**/bin/");
        CleanDirectories("./src/**/obj/");
        CleanDirectories("./test/**/bin/");
        CleanDirectories("./test/**/obj/");
        CleanDirectory("./artifacts");
    });   

Task("build")
    .IsDependentOn("clean")
    .Does(() => {
        DotNetCoreRestore("./src/RobotsTxt/RobotsTxt.csproj");
        
        var buildSettings = new DotNetCoreBuildSettings {
            Configuration = "Release",
            VersionSuffix = versionSuffix,
			Framework = framework
        };

        DotNetCoreBuild("./src/RobotsTxt/RobotsTxt.csproj", buildSettings);
    });

Task("pack")
    .Does(() => {
        var packSettings = new DotNetCorePackSettings{
            Configuration = "Release",
            OutputDirectory = output,
            VersionSuffix = versionSuffix
        };

        DotNetCorePack("./src/RobotsTxt/RobotsTxt.csproj", packSettings);
    });

Task("test")
    .Does(() => {
        var settings = new DotNetCoreTestSettings {
			Framework = framework
		};

        DotNetCoreRestore("./tests/RobotsTxt.Tests/RobotsTxt.Tests.csproj");                
        DotNetCoreTest("./tests/RobotsTxt.Tests/RobotsTxt.Tests.csproj", settings);
    });

RunTarget(target);