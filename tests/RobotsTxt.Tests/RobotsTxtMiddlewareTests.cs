using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace RobotsTxt.Tests;

public class RobotsTxtMiddlewareTests {
    [Fact]
    public async Task RobotsTxtShouldRenderOnCorrectPathAsync() {
        var server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
        var client = server.CreateClient();

        var response = await client.GetAsync("/robots.txt");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();

        var expectedLines = new[] {
            "# Allow Googlebot",
            "User-agent: Googlebot",
            "Allow: /",
            "Request-rate: 1/10",
            "",
            "# Disallow the rest",
            "User-agent: *",
            "Crawl-delay: 10",
            "Disallow: /",
            "",
            "Sitemap: https://example.com/sitemap.xml"
        };

        var expected = string.Join(Environment.NewLine, expectedLines);

        result.ShouldBe(expected);
    }

    [Fact]
    public async Task RobotsTxtShouldRenderAllowAllCorrectlyAsync() {
        var server = new TestServer(new WebHostBuilder().UseStartup<StartupAllowAll>());
        var client = server.CreateClient();

        var response = await client.GetAsync("/robots.txt");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();

        var expectedLines = new[] {
            "User-agent: *",
            "Disallow:"
        };

        var expected = string.Join(Environment.NewLine, expectedLines);

        result.ShouldBe(expected);
    }

    [Fact]
    public async Task RobotsTxtShouldRenderDenyAllCorrectlyAsync() {
        var server = new TestServer(new WebHostBuilder().UseStartup<StartupDenyAll>());
        var client = server.CreateClient();

        var response = await client.GetAsync("/robots.txt");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();

        var expectedLines = new[] {
            "User-agent: *",
            "Disallow: /"
        };

        var expected = string.Join(Environment.NewLine, expectedLines);

        result.ShouldBe(expected);
    }

    [Fact]
    public async Task RobotsTxtShouldRenderACommentIfEmptyAsync() {
        var server = new TestServer(new WebHostBuilder().UseStartup<StartupNoConfig>());
        var client = server.CreateClient();

        var response = await client.GetAsync("/robots.txt");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();
        var expected = "# This file didn't get any instructions so everyone is allowed";

        result.ShouldBe(expected);
    }

    [Fact]
    public async Task RobotsTxtShouldFallThroughOnInvalidPathAsync() {
        var server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
        var client = server.CreateClient();

        var response = await client.GetAsync("/not-robots.txt");

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RobotsTxtShouldRenderForTheCorrextEnvironmentAsync() {
        var server = new TestServer(new WebHostBuilder().UseStartup<StartupMultipleEnvironments>());
        var client = server.CreateClient();

        var response = await client.GetAsync("/robots.txt");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();
        var expected = @"# All robots allowed for Production
User-agent: *
Allow: /";

        result.ShouldBe(expected);
    }
}

file class TestHostEnvironment : IHostEnvironment {
    public string ApplicationName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public IFileProvider ContentRootFileProvider { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string ContentRootPath { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string EnvironmentName { get => "Production"; set => throw new NotImplementedException(); }
}

file class Startup {
    public void ConfigureServices(IServiceCollection services) {
        services.Replace(new ServiceDescriptor(typeof(IHostEnvironment), _ => new TestHostEnvironment(), ServiceLifetime.Singleton));
        services.AddRobotsTxt(builder =>
            builder
                .AddSection(section =>
                    section
                        .AddComment("Allow Googlebot")
                        .AddUserAgent("Googlebot")
                        .Allow("/")
                        .AddCustomDirective("Request-rate", "1/10")
                    )
                .AddSection(section =>
                    section
                        .AddComment("Disallow the rest")
                        .AddUserAgent("*")
                        .AddCrawlDelay(TimeSpan.FromSeconds(10))
                        .Disallow("/")
                    )
                .AddSitemap("https://example.com/sitemap.xml"));
    }

    public void Configure(IApplicationBuilder app) {
        app.UseRobotsTxt();
    }
}

file class StartupMultipleEnvironments {
    public void ConfigureServices(IServiceCollection services) {
        services.Replace(new ServiceDescriptor(typeof(IHostEnvironment), _ => new TestHostEnvironment(), ServiceLifetime.Singleton));
        services.AddRobotsTxt(builder =>
            builder
                .ForEnvironment("Development")
                .AddSection(section =>
                    section
                        .AddUserAgent("*")
                        .AddComment("No robots allowed for Development")
                        .Disallow("/")
                    )
                );

        services.AddRobotsTxt(builder =>
            builder
                .ForEnvironment("Production")
                .AddSection(section =>
                    section
                        .AddUserAgent("*")
                        .AddComment("All robots allowed for Production")
                        .Allow("/")
                    )
                );
    }

    public void Configure(IApplicationBuilder app) {
        app.UseRobotsTxt();
    }
}

file class StartupAllowAll {
    public void ConfigureServices(IServiceCollection services) {
        services.AddSingleton<IHostEnvironment, TestHostEnvironment>();
        services.AddRobotsTxt(builder => builder.AllowAll());
    }

    public void Configure(IApplicationBuilder app) {
        app.UseRobotsTxt();
    }
}

file class StartupDenyAll {
    public void ConfigureServices(IServiceCollection services) {
        services.AddSingleton<IHostEnvironment, TestHostEnvironment>();
        services.AddRobotsTxt(builder => builder.DenyAll());
    }

    public void Configure(IApplicationBuilder app) {
        app.UseRobotsTxt();
    }
}

file class StartupNoConfig {
    public void ConfigureServices(IServiceCollection services) {
        services.AddSingleton<IHostEnvironment, TestHostEnvironment>();
        services.AddRobotsTxt(builder => builder);
    }

    public void Configure(IApplicationBuilder app) {
        app.UseRobotsTxt();
    }
}
