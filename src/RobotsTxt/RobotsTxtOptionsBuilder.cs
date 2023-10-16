namespace RobotsTxt;

public class RobotsTxtOptionsBuilder {
    private readonly RobotsTxtOptions _options;

    public RobotsTxtOptionsBuilder(RobotsTxtOptions options = null) {
        _options = options ?? new RobotsTxtOptions();
    }

    public RobotsTxtOptionsBuilder ForEnvironment(string environment) {
        if(string.IsNullOrWhiteSpace(environment))
            throw new ArgumentException("Environment must not be null or whitespace.", nameof(environment));

        _options.Environment = environment;
        return this;
    }

    public RobotsTxtOptionsBuilder ForHostnames(params string[] hostnames) {
        _options.Hostnames = hostnames.ToList();
        return this;
    }

    public RobotsTxtOptionsBuilder WithMageAge(TimeSpan maxAge) {
        _options.MaxAge = maxAge;
        return this;
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
        // Note: on Linux/macOS, "/path" URLs are treated as valid absolute file URLs.
        // To ensure relative urls are correctly rejected on these platforms,
        // an additional check using IsWellFormedOriginalString() is made here.
        // See https://github.com/dotnet/corefx/issues/22098 for more information.
        if(!Uri.TryCreate(url, UriKind.Absolute, out var uri) || !uri.IsWellFormedOriginalString())
            throw new ArgumentException("Url must be an absolute url to the sitemap.", nameof(url));

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

        public SectionBuilder AddCustomDirective(string directive, string value) {
            Section.Rules.Add(new RobotsTxtCrawlCustomRule(directive, value));
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
