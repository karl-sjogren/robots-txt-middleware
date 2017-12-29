using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using RobotsTxt;

namespace Microsoft.AspNetCore.Builder {
    public static class IApplicationBuilderExtensions {
        public static void UseRobotsTxtMiddleware(this IApplicationBuilder app, Func<RobotsTxtOptionsBuilder, RobotsTxtOptionsBuilder> builderFunc) {
            var builder = new RobotsTxtOptionsBuilder();
            var options = builderFunc(builder).Build();
            app.UseRobotsTxtMiddleware(options);
        }

        public static void UseRobotsTxtMiddleware(this IApplicationBuilder app, RobotsTxtOptions options) {
            app.UseMiddleware<RobotsTxtMiddleware>(options);
        }
    }
}