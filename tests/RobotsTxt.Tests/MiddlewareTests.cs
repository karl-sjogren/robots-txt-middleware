using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace RobotsTxt.Tests {
    public class MiddlewareTests {
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
                "",
                "# Disallow the rest",
                "User-agent: *",
                "Crawl-delay: 10",
                "Disallow: /",
                "",
                "Sitemap: https://example.com/sitemap.xml"
            };

            var expected = string.Join(Environment.NewLine, expectedLines);

            Assert.Equal(expected, result);
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

            Assert.Equal(expected, result);
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

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task RobotsTxtShouldRenderACommentIfEmptyAsync() {
            var server = new TestServer(new WebHostBuilder().UseStartup<StartupNoConfig>());
            var client = server.CreateClient();

            var response = await client.GetAsync("/robots.txt");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            var expected = "# This file didn't get any instructions so everyone is allowed";

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task RobotsTxtShouldFallThroughOnInvalidPathAsync() {
            var server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            var client = server.CreateClient();

            var response = await client.GetAsync("/not-robots.txt");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }

    public class Startup {
        public void ConfigureServices(IServiceCollection services) {
            services.AddStaticRobotsTxt(builder =>
                builder
                    .AddSection(section =>
                        section
                            .AddComment("Allow Googlebot")
                            .AddUserAgent("Googlebot")
                            .Allow("/")
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

    public class StartupAllowAll {
        public void ConfigureServices(IServiceCollection services) {
            services.AddStaticRobotsTxt(builder => builder.AllowAll());
        }

        public void Configure(IApplicationBuilder app) {
            app.UseRobotsTxt();
        }
    }

    public class StartupDenyAll {
        public void ConfigureServices(IServiceCollection services) {
            services.AddStaticRobotsTxt(builder => builder.DenyAll());
        }

        public void Configure(IApplicationBuilder app) {
            app.UseRobotsTxt();
        }
    }

    public class StartupNoConfig {
        public void ConfigureServices(IServiceCollection services) {
            services.AddStaticRobotsTxt(builder => builder);
        }

        public void Configure(IApplicationBuilder app) {
            app.UseRobotsTxt();
        }
    }
}
