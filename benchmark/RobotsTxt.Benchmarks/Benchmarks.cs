using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using RobotsTxt;
#if V2
using RobotsTxt.Contracts;
using RobotsTxt.Services;
#endif

namespace RobotsTxt.Benchmarks {
    [MemoryDiagnoser]
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
#if V1
            return (RobotsTxtOptionsBuilder)typeof(RobotsTxtOptionsBuilder).GetConstructor(
                  BindingFlags.NonPublic | BindingFlags.Instance,
                  null, Type.EmptyTypes, null).Invoke(null);
#endif

#if V2
            return new RobotsTxtOptionsBuilder();
#else
            return null;
#endif
        }

        [Benchmark]
        public async Task StaticRobotsTxtProviderAsync() {
            var httpContext = new DefaultHttpContext();

#if V1
            await _middleware.Invoke(httpContext);
#endif

#if V2
            await _middleware.InvokeAsync(httpContext, _provider);
#endif
        }

        public static Task RequestDelegateAsync(HttpContext context) {
            return Task.CompletedTask;
        }

        private class Config : ManualConfig {
            public Config() {
                var baseJob = Job.MediumRun.WithStrategy(RunStrategy.Throughput);

                AddJob(baseJob.WithNuGet("RobotsTxtCore", "2.0.0-preview1").WithId("2.0.0-preview1").WithCustomBuildConfiguration("V2"));
                AddJob(baseJob.WithNuGet("RobotsTxtCore", "1.1.0").WithId("1.1.0").WithCustomBuildConfiguration("V1"));
            }
        }
    }
}
