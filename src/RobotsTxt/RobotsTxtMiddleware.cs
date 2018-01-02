using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace RobotsTxt {
    public class RobotsTxtMiddleware {
        private static readonly PathString _robotsTxtPath = new PathString("/robots.txt");
        private readonly RobotsTxtOptions _options;
        private readonly RequestDelegate _next;

        public RobotsTxtMiddleware(RequestDelegate next, RobotsTxtOptions options) {
            _next = next;
            _options = options;
        }

        public async Task Invoke(HttpContext context) {
            if(context.Request.Path == _robotsTxtPath) {
                context.Response.ContentType = "text/plain";
                context.Response.Headers.Add("Cache-Control", $"max-age={_options.MaxAge.TotalSeconds}");

                await BuildRobotsTxt(context);
                return;
            }

            await _next.Invoke(context);
        }

        private async Task BuildRobotsTxt(HttpContext context) {
            var sb = _options.Build();

            var output = sb.ToString();

            if(string.IsNullOrWhiteSpace(output))
                output = "# This file didn't get any instructions so everyone is allowed";

            using(var sw = new StreamWriter(context.Response.Body))
                await sw.WriteAsync(output);
        }
    }
}