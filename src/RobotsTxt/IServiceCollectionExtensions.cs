using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace RobotsTxt;

public static class IServiceCollectionExtensions {
    [Obsolete("Use AddRobotsTxt instead.", false)]
    public static void AddStaticRobotsTxt(this IServiceCollection services, Func<RobotsTxtOptionsBuilder, RobotsTxtOptionsBuilder> builderFunc) {
        AddRobotsTxt(services, builderFunc);
    }

    public static void AddRobotsTxt(this IServiceCollection services, Func<RobotsTxtOptionsBuilder, RobotsTxtOptionsBuilder> builderFunc) {
        var builder = new RobotsTxtOptionsBuilder();
        var options = builderFunc(builder).Build();

        services.AddSingleton(options);

        var previousRegistration = services.FirstOrDefault(s => s.ServiceType == typeof(IRobotsTxtProvider));
        if(!options.Hostnames.Any()) {
            if(previousRegistration?.Lifetime != ServiceLifetime.Scoped) {
                services.TryAddSingleton<IRobotsTxtProvider, StaticRobotsTxtProvider>();
            }
        } else {
            if(previousRegistration?.Lifetime != ServiceLifetime.Scoped) {
                services.Remove(previousRegistration);
            }

            services.TryAddScoped(CreateRobotsTxtProviderForMultipleHosts);
        }
    }

    private static readonly RobotsTxtOptions _denyAllOptions = new RobotsTxtOptionsBuilder().DenyAll().Build();

    internal static IRobotsTxtProvider CreateRobotsTxtProviderForMultipleHosts(IServiceProvider services) {
        var httpContextAccessor = services.GetRequiredService<IHttpContextAccessor>();
        var httpContext = httpContextAccessor.HttpContext;

        if(httpContext == null) {
            throw new InvalidOperationException("Could not get HttpContext from IHttpContextAccessor.");
        }

        var host = httpContext.Request.Host.Value;

        var options = services.GetRequiredService<IEnumerable<RobotsTxtOptions>>().ToArray();
        var robotsOptions = options
            .Where(option => option.Hostnames.Any(hostname => hostname.Equals(host, StringComparison.OrdinalIgnoreCase)));

        if(!robotsOptions.Any()) {
            robotsOptions = options.Where(option => !option.Hostnames.Any());

            if(!robotsOptions.Any()) {
                robotsOptions = new[] { _denyAllOptions };
            }
        }

        //var hostEnvironemnt = services.GetRequiredService<IHostEnvironment>();
        return ActivatorUtilities.CreateInstance(services, typeof(StaticRobotsTxtProvider), robotsOptions) as IRobotsTxtProvider;
        //return new StaticRobotsTxtProvider(robotsOptions, hostEnvironemnt);
    }
}
