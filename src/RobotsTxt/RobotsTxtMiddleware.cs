using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RobotsTxt.Contracts;

namespace RobotsTxt {
    public class RobotsTxtMiddleware {
        private static readonly PathString _robotsTxtPath = new("/robots.txt");
        private readonly RequestDelegate _next;

        public RobotsTxtMiddleware(RequestDelegate next) {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IRobotsTxtProvider robotsTxtProvider) {
            if(context.Request.Path == _robotsTxtPath) {
                var maxAge = await robotsTxtProvider.GetMaxAgeAsync(context.RequestAborted);
                context.Response.ContentType = "text/plain";
                context.Response.Headers.Add("Cache-Control", $"max-age={maxAge.TotalSeconds}");

                var buffer = await robotsTxtProvider.GetRobotsTxtAsync(context.RequestAborted);
                await context.Response.Body.WriteAsync(buffer, context.RequestAborted);

                return;
            }

            await _next.Invoke(context);
        }
    }
}
