using System;

namespace RobotsTxt {
    public class RobotsTxtOptionsBuilder {
        private readonly RobotsTxtOptions _options;

        public RobotsTxtOptionsBuilder() {
            _options = new RobotsTxtOptions();
        }

        public RobotsTxtOptionsBuilder DenyAll() {
            _options.Sections.Clear();
            return AddSection(section =>
                section
                    .AddUserAgent("*")
                    .Disallow("/")
            );
        }

        public RobotsTxtOptionsBuilder AllowAll() {
            _options.Sections.Clear();
            return AddSection(section =>
                section
                    .AddUserAgent("*")
                    .Disallow(string.Empty)
            );
        }

        public RobotsTxtOptionsBuilder AddSection(Func<SectionBuilder, SectionBuilder> builder) {
            var sectionBuilder = new SectionBuilder();
            sectionBuilder = builder(sectionBuilder);
            _options.Sections.Add(sectionBuilder.Section);
            return this;
        }

        public RobotsTxtOptionsBuilder AddSitemap(string url) {
            if(!Uri.TryCreate(url, UriKind.Absolute, out _))
                throw new ArgumentException("Url must be an absolute url for sitemaps.", nameof(url));

            _options.SitemapUrls.Add(url);
            return this;
        }

        public RobotsTxtOptions Build() {
            return _options;
        }

        public class SectionBuilder {
            internal RobotsTxtSection Section { get; }

            internal SectionBuilder() {
                Section = new RobotsTxtSection();
            }

            public SectionBuilder AddUserAgent(string userAgent) {
                Section.UserAgents.Add(userAgent);
                return this;
            }

            public SectionBuilder AddComment(string comment) {
                Section.Comments.Add(comment);
                return this;
            }

            public SectionBuilder AddCrawlDelay(TimeSpan delay) {
                Section.Rules.Add(new RobotsTxtCrawlDelayRule(delay));
                return this;
            }

            public SectionBuilder Allow(string path) {
                Section.Rules.Add(new RobotsTxtAllowRule(path));
                return this;
            }

            public SectionBuilder Disallow(string path) {
                Section.Rules.Add(new RobotsTxtDisallowRule(path));
                return this;
            }
        }
    }
}
