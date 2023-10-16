using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace RobotsTxt.Tests;

public class ServiceCollectionExtensionTests {
    [Fact]
    public void AddStaticRobotsTxt_WhenRegisteringWithoutHostname_RegistersProviderAsSingleton() {
        var services = new ServiceCollection();

        services.AddStaticRobotsTxt(builder =>
            builder
                .DenyAll()
        );

        var serviceRegistration = services.FirstOrDefault(s => s.ServiceType == typeof(IRobotsTxtProvider));

        serviceRegistration.ShouldNotBeNull();
        serviceRegistration.Lifetime.ShouldBe(ServiceLifetime.Singleton);
    }

    [Fact]
    public void AddStaticRobotsTxt_WhenRegisteringMultipleOptionsWithDifferentHostNameOptions_RegistersProviderAsScoped() {
        var services = new ServiceCollection();

        services.AddStaticRobotsTxt(builder =>
            builder
                .DenyAll()
        );

        services.AddStaticRobotsTxt(builder =>
            builder
                .ForHostnames("example.com")
                .DenyAll()
        );

        services.AddStaticRobotsTxt(builder =>
            builder
                .DenyAll()
        );

        var serviceRegistration = services.FirstOrDefault(s => s.ServiceType == typeof(IRobotsTxtProvider));

        serviceRegistration.ShouldNotBeNull();
        serviceRegistration.Lifetime.ShouldBe(ServiceLifetime.Scoped);
    }

    [Theory]
    [InlineData("example.com", "https://example.com/sitemap.xml")]
    [InlineData("www.example.com", "https://example.com/sitemap.xml")]
    [InlineData("example-com-test-site.com", "https://example.com/sitemap.xml")]
    [InlineData("cool-horses-with-glasses.com", "https://cool-horses-with-glasses.com/sitemap.xml")]
    [InlineData("squirrels-with-hats.com", "https://sample-website.com/sitemap.xml")]

    public async Task CreateRobotsTxtProviderForMultipleHosts_WhenCalledWithDifferentHostnames_ReturnsExpectedResultAsync(string hostname, string sitemapUrl) {
        var services = new ServiceCollection();

        services.AddStaticRobotsTxt(builder =>
            builder
                .ForHostnames("example.com", "www.example.com", "example-com-test-site.com")
                .AddSitemap("https://example.com/sitemap.xml")
                .DenyAll()
        );

        services.AddStaticRobotsTxt(builder =>
            builder
                .ForHostnames("cool-horses-with-glasses.com")
                .AddSitemap("https://cool-horses-with-glasses.com/sitemap.xml")
                .DenyAll()
        );

        services.AddStaticRobotsTxt(builder =>
            builder
                .AddSitemap("https://sample-website.com/sitemap.xml")
                .DenyAll()
        );

        var hostEnvironemnt = A.Dummy<IHostEnvironment>();
        services.AddSingleton(hostEnvironemnt);

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Host = new HostString(hostname);

        services.AddSingleton<IHttpContextAccessor>(new HttpContextAccessor { HttpContext = httpContext });

        var serviceProvider = services.BuildServiceProvider();

        var provider = IServiceCollectionExtensions.CreateRobotsTxtProviderForMultipleHosts(serviceProvider);

        var sitemap = await provider.GetResultAsync(CancellationToken.None);

        sitemap.ShouldNotBeNull();

        var sitemapContent = Encoding.UTF8.GetString(sitemap.Content.Span);

        sitemapContent.ShouldContain(sitemapUrl);
    }
}
