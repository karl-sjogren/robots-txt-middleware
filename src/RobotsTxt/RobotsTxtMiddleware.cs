using Microsoft.AspNetCore.Http;

namespace RobotsTxt;

public class RobotsTxtMiddleware {
    private static readonly PathString _robotsTxtPath = new("/robots.txt");
    private readonly RequestDelegate _next;

    public RobotsTxtMiddleware(RequestDelegate next) {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IRobotsTxtProvider robotsTxtProvider) {
        if(context.Request.Path == _robotsTxtPath) {
            var result = await robotsTxtProvider.GetResultAsync(context.RequestAborted);

            context.Response.ContentType = "text/plain";
            context.Response.Headers["Cache-Control"] = $"max-age={result.MaxAge}";

            await context.Response.Body.WriteAsync(result.Content, context.RequestAborted);

            return;
        }

        await _next.Invoke(context);
    }
}
