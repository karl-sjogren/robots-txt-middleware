using System;

namespace RobotsTxt
{
    public class RobotsTxtOptionsBuilder {
        private readonly RobotsTxtOptions _options;

        internal RobotsTxtOptionsBuilder() {
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
            
            public SectionBuilder AddHost(string host) {
                if(!Uri.TryCreate(host, UriKind.Absolute, out _))
                    throw new ArgumentException("Host must be an absolute url.", nameof(host));
                
                _section.Rules.Add(new RobotsTxtHostRule(host));
                return this;
            }
        }
    }
}