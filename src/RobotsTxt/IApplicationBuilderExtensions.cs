using RobotsTxt;

namespace Microsoft.AspNetCore.Builder {
    public static class IApplicationBuilderExtensions {
        public static void UseRobotsTxt(this IApplicationBuilder app) {
            app.UseMiddleware<RobotsTxtMiddleware>();
        }
    }
}
