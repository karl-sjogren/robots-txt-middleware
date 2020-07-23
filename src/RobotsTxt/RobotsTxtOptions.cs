using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace RobotsTxt {
    public class RobotsTxtOptions {
        public RobotsTxtOptions() {
            Sections = new List<RobotsTxtSection>();
            SitemapUrls = new List<string>();
        }

        public List<RobotsTxtSection> Sections { get; }
        public List<string> SitemapUrls { get; }
        public TimeSpan MaxAge { get; } = TimeSpan.FromDays(1);

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
            builder.AppendLine("Crawl-delay: " + Value);
        }
    }
    
    public class RobotsTxtHostRule : RobotsTxtRule {
        public RobotsTxtHostRule(string host) : base(RobotsTxtRuleType.Host, host) { }

        public override void Build(StringBuilder builder) {
            builder.AppendLine("Host: " + Value);
        }
    }

    public enum RobotsTxtRuleType {
        Allow,
        Disallow,
        CrawlDelay,
        Host
    }
}