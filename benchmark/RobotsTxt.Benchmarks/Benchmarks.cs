using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using Microsoft.AspNetCore.Http;
#if V2 && !V2P3
using RobotsTxt.Contracts;
using RobotsTxt.Services;
#endif

namespace RobotsTxt.Benchmarks;

[Config(typeof(Config))]
public class Benchmarks {
    private readonly RobotsTxtOptionsBuilder _optionsBuilder;
    private readonly RobotsTxtOptions _options;

#if V1
    private readonly RobotsTxtMiddleware _middleware;
#endif

#if V2
    private readonly RobotsTxtMiddleware _middleware;
    private readonly IRobotsTxtProvider _provider;
#endif

    public Benchmarks() {
        _optionsBuilder = GetNewOptionsBuilder()
            .AddSection(section =>
                section
                    .AddComment("Allow Googlebot")
                    .AddUserAgent("Googlebot")
                    .Allow("/")
                )
            .AddSection(section =>
                section
                    .AddComment("Allow Bing for most stuff")
                    .AddUserAgent("Bing")
                    .Disallow("/bing-should-not-see-this")
                    .Allow("/")
                )
            .AddSection(section =>
                section
                    .AddComment("Disallow the rest")
                    .AddUserAgent("*")
                    .AddCrawlDelay(TimeSpan.FromSeconds(10))
                    .Disallow("/")
                )
            .AddSitemap("https://example.com/sitemap.xml");

        _options = _optionsBuilder.Build();
#if V1
        _middleware = new RobotsTxtMiddleware(RequestDelegateAsync, _options);
#endif

#if V2
        _middleware = new RobotsTxtMiddleware(RequestDelegateAsync);
        _provider = new StaticRobotsTxtProvider(_options);
#endif
    }

    private static RobotsTxtOptionsBuilder GetNewOptionsBuilder() {
#if V2
        return new RobotsTxtOptionsBuilder();
#else
        return (RobotsTxtOptionsBuilder)typeof(RobotsTxtOptionsBuilder).GetConstructor(
              BindingFlags.NonPublic | BindingFlags.Instance,
              null, Type.EmptyTypes, null).Invoke(null);
#endif
    }

    [Benchmark]
    public async Task StaticRobotsTxtProviderAsync() {
        for(var i = 0; i < 1000; i++) {
            var httpContext = new SimpleHttpContext();
#if V1
            await _middleware.Invoke(httpContext);
#endif

#if V2
            await _middleware.InvokeAsync(httpContext, _provider);
#endif
        }
    }

    public static Task RequestDelegateAsync(HttpContext context) {
        return Task.CompletedTask;
    }

    private class Config : ManualConfig {
        public Config() {
            var baseJob = Job.Default;

            AddJob(baseJob.WithNuGet("RobotsTxtCore", "2.1.0").WithId("2.1.0").WithCustomBuildConfiguration("V21"));
            AddJob(baseJob.WithNuGet("RobotsTxtCore", "2.0.0-preview3").WithId("2.0.0-preview3").WithCustomBuildConfiguration("V2P3"));
            AddJob(baseJob.WithNuGet("RobotsTxtCore", "2.0.0-preview2").WithId("2.0.0-preview2").WithCustomBuildConfiguration("V2"));
            AddJob(baseJob.WithNuGet("RobotsTxtCore", "2.0.0-preview1").WithId("2.0.0-preview1").WithCustomBuildConfiguration("V2"));
            AddJob(baseJob.WithNuGet("RobotsTxtCore", "1.1.0").WithId("1.1.0").WithCustomBuildConfiguration("V1"));

            AddDiagnoser(MemoryDiagnoser.Default);
            AddExporter(MarkdownExporter.GitHub);
        }
    }
}
