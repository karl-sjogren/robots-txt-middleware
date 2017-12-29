
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace RobotsTxt {
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
}