using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace RobotsTxtMiddleware {
    public static class RobotsMiddlewareExtensions {
        public static void UseRobotsTxtMiddleware(this IApplicationBuilder app, Func<RobotsTxtOptionsBuilder, RobotsTxtOptionsBuilder> builderFunc) {
            var builder = new RobotsTxtOptionsBuilder();
            var options = builderFunc(builder).Build();
            app.UseRobotsTxtMiddleware(options);
        }

        public static void UseRobotsTxtMiddleware(this IApplicationBuilder app, RobotsTxtOptions options) {
            app.UseMiddleware<RobotsTxtMiddleware>(options);
        }
    }

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
                await BuildRobotsTxt(context);
                return;
            }

            await _next.Invoke(context);
        }

        private async Task BuildRobotsTxt(HttpContext context) {
            var sb = _options.Build();

            using(var sw = new StreamWriter(context.Response.Body))
                await sw.WriteAsync(sb.ToString());
        }
    }

    public class RobotsTxtOptionsBuilder {
        private readonly RobotsTxtOptions _options;

        internal RobotsTxtOptionsBuilder() {
            _options = new RobotsTxtOptions();
        }

        public RobotsTxtOptionsBuilder AddSection(Func<SectionBuilder, SectionBuilder> builder) {
            var sectionBuilder = new SectionBuilder();
            sectionBuilder = builder(sectionBuilder);
            _options.Sections.Add(sectionBuilder.Section);
            return this;
        }

        public RobotsTxtOptionsBuilder AddSitemap(string url) {
            _options.SitemapUrls.Add(url);
            return this;
        }

        public RobotsTxtOptions Build() {
            return _options;
        }

        public class SectionBuilder {
            private readonly RobotsTxtSection _section;

            internal RobotsTxtSection Section => _section;

            internal SectionBuilder() {
                _section = new RobotsTxtSection();
            }

            public SectionBuilder AddUserAgent(string userAgent) {
                _section.UserAgents.Add(userAgent);
                return this;
            }

            public SectionBuilder AddComment(string comment) {
                _section.Comments.Add(comment);
                return this;
            }

            public SectionBuilder AddCrawlDelay(TimeSpan delay) {
                _section.Rules.Add(new RobotsTxtCrawlDelayRule(delay));
                return this;
            }

            public SectionBuilder Allow(string path) {
                _section.Rules.Add(new RobotsTxtAllowRule(path));
                return this;
            }

            public SectionBuilder Disallow(string path) {
                _section.Rules.Add(new RobotsTxtDisallowRule(path));
                return this;
            }
        }
    }

    public class RobotsTxtOptions {
        public RobotsTxtOptions() {
            Sections = new List<RobotsTxtSection>();
            SitemapUrls = new List<string>();
        }

        public List<RobotsTxtSection> Sections { get; }
        public List<string> SitemapUrls { get; }

        internal StringBuilder Build() {
            var builder = new StringBuilder();

            foreach(var section in Sections) {
                section.Build(builder);
                builder.AppendLine();
            }

            foreach(var url in SitemapUrls) {
                builder.AppendLine("Sitemap: " + url);
            }

            return builder;
        }
    }

    public class RobotsTxtSection {
        public RobotsTxtSection() {
            Comments = new List<string>();
            UserAgents = new List<string>();
            Rules = new List<RobotsTxtRule>();
        }

        public List<string> Comments;
        public List<string> UserAgents;
        public List<RobotsTxtRule> Rules;

        public void Build(StringBuilder builder) {
            if(!UserAgents.Any())
                return;

            foreach(var comment in Comments) {
                builder.AppendLine("# " + comment);
            }

            foreach(var userAgent in UserAgents) {
                builder.AppendLine("User-agent: " + userAgent);
            }

            foreach(var rule in Rules) {
                rule.Build(builder);
            }
        }
    }

    public abstract class RobotsTxtRule {
        public RobotsTxtRuleType Type { get; }
        public string Value { get; }

        protected RobotsTxtRule(RobotsTxtRuleType type, string value) {
            Type = type;
            Value = value;
        }

        public abstract void Build(StringBuilder builder);
    }

    public class RobotsTxtAllowRule : RobotsTxtRule {
        public RobotsTxtAllowRule(string path) : base(RobotsTxtRuleType.Allow, path) { }

        public override void Build(StringBuilder builder) {
            builder.AppendLine("Allow: " + Value);
        }
    }

    public class RobotsTxtDisallowRule : RobotsTxtRule {
        public RobotsTxtDisallowRule(string path) : base(RobotsTxtRuleType.Disallow, path) { }

        public override void Build(StringBuilder builder) {
            builder.AppendLine("Disallow: " + Value);
        }
    }

    public class RobotsTxtCrawlDelayRule : RobotsTxtRule {
        public RobotsTxtCrawlDelayRule(TimeSpan delay)
            : base(RobotsTxtRuleType.CrawlDelay, delay.TotalSeconds.ToString(CultureInfo.InvariantCulture)) { }

        public override void Build(StringBuilder builder) {
            builder.AppendLine("crawl-delay: " + Value);
        }
    }

    public enum RobotsTxtRuleType {
        Allow,
        Disallow,
        CrawlDelay
    }
}