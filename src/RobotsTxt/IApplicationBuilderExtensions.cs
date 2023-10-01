using Microsoft.AspNetCore.Builder;

namespace RobotsTxt;

public static class IApplicationBuilderExtensions {
    public static void UseRobotsTxt(this IApplicationBuilder app) {
        app.UseMiddleware<RobotsTxtMiddleware>();
    }
}
