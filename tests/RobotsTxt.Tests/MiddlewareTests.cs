using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace RobotsTxt.Tests {
    public class MiddlewareTests {
        [Fact]
        public async Task RobotsTxtShouldRenderOnCorrectPath() {
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
        public async Task RobotsTxtShouldRenderAllowAllCorrectly() {
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
        public async Task RobotsTxtShouldRenderDenyAllCorrectly() {
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
        public async Task RobotsTxtShouldRenderACommentIfEmpty() {
            var server = new TestServer(new WebHostBuilder().UseStartup<StartupNoConfig>());
            var client = server.CreateClient();

            var response = await client.GetAsync("/robots.txt");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            var expected = "# This file didn't get any instructions so everyone is allowed";

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task RobotsTxtShouldFallThroughOnInvalidPath() {
            var server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            var client = server.CreateClient();

            var response = await client.GetAsync("/not-robots.txt");
            
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }

    public class Startup {
        public void Configure(IApplicationBuilder app) {
            app.UseRobotsTxt(builder => 
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
                    .AddSitemap("https://example.com/sitemap.xml")
            );
        }
    }

    public class StartupAllowAll {
        public void Configure(IApplicationBuilder app) {
            app.UseRobotsTxt(builder => builder.AllowAll());
        }
    }

    public class StartupDenyAll {
        public void Configure(IApplicationBuilder app) {
            app.UseRobotsTxt(builder => builder.DenyAll());
        }
    }

    public class StartupNoConfig {
        public void Configure(IApplicationBuilder app) {
            app.UseRobotsTxt(builder => builder);
        }
    }
}
