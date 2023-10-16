using System.Globalization;
using System.Text;

namespace RobotsTxt;

public class RobotsTxtOptions {
    public RobotsTxtOptions() {
        Sections = new List<RobotsTxtSection>();
        SitemapUrls = new List<string>();
        Hostnames = new List<string>();
    }

    public string Environment { get; internal set; }
    public List<string> Hostnames { get; internal set; }
    public List<RobotsTxtSection> Sections { get; }
    public List<string> SitemapUrls { get; }
    public TimeSpan MaxAge { get; internal set; } = TimeSpan.FromDays(1);

    internal StringBuilder GenerateFileContents() {
        var builder = new StringBuilder();

        foreach(var section in Sections) {
            section.Build(builder);
            builder.AppendLine();
        }

        foreach(var url in SitemapUrls) {
            builder
                .Append("Sitemap: ")
                .AppendLine(url);
        }

        return builder;
    }

    public override string ToString() {
        return GenerateFileContents().ToString()?.TrimEnd();
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
        if(UserAgents.Count == 0)
            return;

        foreach(var comment in Comments) {
            builder
                .Append("# ")
                .AppendLine(comment);
        }

        foreach(var userAgent in UserAgents) {
            builder
                .Append("User-agent: ")
                .AppendLine(userAgent);
        }

        foreach(var rule in Rules) {
            rule.Build(builder);
        }
    }
}

public abstract class RobotsTxtRule {
    public string Value { get; }

    protected RobotsTxtRule(string value) {
        Value = value;
    }

    public abstract void Build(StringBuilder builder);
}

public class RobotsTxtAllowRule : RobotsTxtRule {
    public RobotsTxtAllowRule(string path) : base(path) { }

    public override void Build(StringBuilder builder) {
        builder
            .Append("Allow: ")
            .AppendLine(Value);
    }
}

public class RobotsTxtDisallowRule : RobotsTxtRule {
    public RobotsTxtDisallowRule(string path) : base(path) { }

    public override void Build(StringBuilder builder) {
        builder
            .Append("Disallow: ")
            .AppendLine(Value);
    }
}

public class RobotsTxtCrawlDelayRule : RobotsTxtRule {
    public RobotsTxtCrawlDelayRule(TimeSpan delay)
        : base(delay.TotalSeconds.ToString(CultureInfo.InvariantCulture)) { }

    public override void Build(StringBuilder builder) {
        builder
            .Append("Crawl-delay: ")
            .AppendLine(Value);
    }
}

public class RobotsTxtCrawlCustomRule : RobotsTxtRule {
    private readonly string _directive;

    public RobotsTxtCrawlCustomRule(string directive, string value)
        : base(value) {
        _directive = directive;
    }

    public override void Build(StringBuilder builder) {
        builder
            .Append(_directive)
            .Append(": ")
            .AppendLine(Value);
    }
}
