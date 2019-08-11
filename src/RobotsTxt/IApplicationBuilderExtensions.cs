using System;
using RobotsTxt;

namespace Microsoft.AspNetCore.Builder
{
    public static class IApplicationBuilderExtensions {
        public static void UseRobotsTxt(this IApplicationBuilder app, Func<RobotsTxtOptionsBuilder, RobotsTxtOptionsBuilder> builderFunc) {
            var builder = new RobotsTxtOptionsBuilder();
            var options = builderFunc(builder).Build();
            app.UseRobotsTxt(options);
        }

        public static void UseRobotsTxt(this IApplicationBuilder app, RobotsTxtOptions options) {
            app.UseMiddleware<RobotsTxtMiddleware>(options);
        }
    }
}