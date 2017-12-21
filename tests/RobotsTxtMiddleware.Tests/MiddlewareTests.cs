using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace RobotsTxtMiddleware.Tests {
    public class MiddlewareTests {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public MiddlewareTests() {
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task RobotsTxtShouldRenderOnCorrectPath() {
            var response = await _client.GetAsync("/robots.txt");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();

            Assert.Equal("# Allow Googlebot\nUser-agent: Googlebot\nAllow: /\n\n# Disallow the rest\nUser-agent: *\nDisallow: /\n\nSitemap: sitemap.xml\n", result);
        }

        [Fact]
        public async Task RobotsTxtShouldFallThroughOnInvalidPath() {
            var response = await _client.GetAsync("/not-robots.txt");
            
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }

    public class Startup {
        public void Configure(IApplicationBuilder app) {
            app.UseRobotsTxtMiddleware(builder => 
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
                            .Disallow("/")
                        )
                    .AddSitemap("sitemap.xml")
            );
        }
    }
}
