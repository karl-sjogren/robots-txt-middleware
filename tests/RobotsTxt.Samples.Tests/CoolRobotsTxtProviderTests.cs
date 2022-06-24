using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using RobotsTxt.Samples;
using Xunit;

namespace RobotsTxt.Tests {
    public class MiddlewareTests {
        [Fact]
        public async Task CoolRobotsTxtProviderShouldReturnAnAllowAllToTheMiddlewareAsync() {
            var server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
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
    }
    public class Startup {
        public void ConfigureServices(IServiceCollection services) {
            services.AddScoped<CoolContext>();
            services.AddScoped<IRobotsTxtProvider, CoolRobotsTxtProvider>();
        }

        public void Configure(IApplicationBuilder app) {
            app.UseRobotsTxt();
        }
    }
}
