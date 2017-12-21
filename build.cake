var target = Argument("target", "build");
var output = Argument("output", "./artifacts");
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
        DotNetCoreRestore("./src/RobotsTxtMiddleware/RobotsTxtMiddleware.csproj");
        
        var buildSettings = new DotNetCoreBuildSettings {
            Configuration = "Release",
            VersionSuffix = versionSuffix
        };

        DotNetCoreBuild("./src/RobotsTxtMiddleware/RobotsTxtMiddleware.csproj", buildSettings);
    });

Task("pack")
    .Does(() => {
        var packSettings = new DotNetCorePackSettings{
            Configuration = "Release",
            OutputDirectory = output,
            VersionSuffix = versionSuffix
        };

        DotNetCorePack("./src/RobotsTxtMiddleware/RobotsTxtMiddleware.csproj", packSettings);
    });

Task("test")
    .Does(() => {
        var settings = new DotNetCoreTestSettings { };

        DotNetCoreRestore("./tests/RobotsTxtMiddleware.Tests/RobotsTxtMiddleware.Tests.csproj");                
        DotNetCoreTest("./tests/RobotsTxtMiddleware.Tests/RobotsTxtMiddleware.Tests.csproj", settings);
    });

RunTarget(target);