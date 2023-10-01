using System;
using Microsoft.Extensions.DependencyInjection;

namespace RobotsTxt;

public static class IServiceCollectionExtensions {
    public static void AddStaticRobotsTxt(this IServiceCollection services, Func<RobotsTxtOptionsBuilder, RobotsTxtOptionsBuilder> builderFunc) {
        var builder = new RobotsTxtOptionsBuilder();
        var options = builderFunc(builder).Build();

        services.AddSingleton<IRobotsTxtProvider>(new StaticRobotsTxtProvider(options));
    }
}
