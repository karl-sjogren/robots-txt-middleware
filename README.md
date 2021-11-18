# RobotsTxtCore [![NuGet Badge](https://buildstats.info/nuget/RobotsTxtCore)](https://www.nuget.org/packages/RobotsTxtCore/)

A Robots.txt middleware for ASP.NET Core. Why is this needed you ask? Because if you
need to add dynamic values (such as a configured url from your CMS) you'll need some
sort of code to handle that, and this makes it easy.

## Installation

### .Net CLI

```sh
> dotnet add package RobotsTxtCore
```

### NuGet

```powershell
PM> Install-Package RobotsTxtCore
```

## Changes from 1.0 to 2.0

The first version of this package was only a middleware and had to be configured in
the `Configure` method in the `Startup` class. This felt fine at the time but as more
and more things moved to having configuration as a service and then letting the
middleware consume the service I felt like this package got outdated.

So I've made a breaking change and made the middleware consume a `IRobotsTxtProvider`
which in turn takes care of configuration. There is a default provider for static uses
(i.e. exactly what the old one did) but doing it this way also let me optimize it quite
a lot. A quick benchmark shows that running a thousand requests against `/robots.txt`
is now done in 25% of the time while also lowering allocations about the same.

| NuGetReferences              |       Mean |    Error |   StdDev |    Gen 0 |  Gen 1 | Allocated |
|----------------------------- |-----------:|---------:|---------:|---------:|-------:|----------:|
| RobotsTxtCore 1.1.0          | 1,169.2 μs | 22.62 μs | 27.77 μs | 691.4063 | 1.9531 |  4,242 KB |
| RobotsTxtCore 2.0.0-preview1 |   419.8 μs |  3.88 μs |  3.24 μs | 167.9688 |      - |  1,031 KB |
| RobotsTxtCore 2.0.0-preview2 |   431.5 μs |  2.90 μs |  2.57 μs | 150.3906 |      - |    922 KB |
| RobotsTxtCore 2.0.0-preview3 |   307.4 μs |  2.00 μs |  1.87 μs | 155.2734 |      - |    953 KB |

Sure, it was really fast to start with and there are very few sites where `/robots.txt`
gets a ton of traffic but that doesn't mean it's not worth it 😉.

Introducing the `IRobotsTxtProvider` also allows for easier dynamic usage, like
reading settings from a database or switching depending on which environment the code
is executing in.

## Usage

To specify multiple rules with the fluent interface makes it really easy.

```csharp
public void ConfigureServices(IServiceCollection services) {
    services.AddStaticRobotsTxt(builder =>
        builder
            .AddSection(section =>
                section
                    .AddComment("Allow Googlebot")
                    .AddUserAgent("Googlebot")
                    .Allow("/")
                )
            .AddSection(section =>
                section
                    .AddComment("Disallow the rest")
                    .AddUserAgent("*")
                    .AddCrawlDelay(TimeSpan.FromSeconds(10))
                    .Disallow("/")
                )
            .AddSitemap("https://example.com/sitemap.xml")
    );
}

public void Configure(IApplicationBuilder app) {
    app.UseRobotsTxt();
}
```

Output

```robots
# Allow Googlebot
User-agent: Googlebot
Allow: /

# Disallow the rest
User-agent: *
Disallow: /
Crawl-delay: 10

Sitemap: https://example.com/sitemap.xml
```

Or if you just want to deny everyone.

```csharp
public void ConfigureServices(IServiceCollection services) {
    services.AddStaticRobotsTxt(builder =>
        builder
            .DenyAll()
    );
}

public void Configure(IApplicationBuilder app) {
    app.UseRobotsTxt();
}
```

Output

```robots
User-agent: *
Disallow: /
```

## IRobotsTxtProvider

`IRobotsTxtProvider` allows for dynamicly configuring the Robots.txt output depending
on your case. It could be used to read from config, to check a database setting or
perhaps which environment your application is currently running in.

```csharp
public class CoolRobotsTxtProvider : IRobotsTxtProvider {
    private readonly CoolContext _context;

    public CoolRobotsTxtProvider(CoolContext context) {
        _context = context;
    }

    public Task<RobotsTxtResult> GetResultAsync(CancellationToken cancellationToken) {
        var settings = await _context.Settings.FirstAsync();

        var builder = new RobotsTxtOptionsBuilder();
        string content;

        if(settings.RobotsTxt.AllowAll)
            content = builder.AllowAll().Build().ToString();
        else
            content = builder.DenyAll().Build().ToString();

        var buffer = Encoding.UTF8.GetBytes(content).AsMemory();
        return new RobotsTxtResult(buffer, settings.RobotsTxt.MaxAge);
    }
}
```
