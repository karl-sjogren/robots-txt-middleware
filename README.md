# RobotsTxtMiddleware [![NuGet Badge](https://buildstats.info/nuget/RobotsTxtCore)](https://www.nuget.org/packages/RobotsTxtCore/)

> From version 2.0 I've dropped support for .NET Core versions prior to 3.1 and made a
huge breaking change by adding a `IRobotsTxtProvider` interface seperate from the
middleware to handle configuration.

A Robots.txt middleware for ASP.NET Core. Why is this needed you ask? Because if you
need to add dynamic values (such as a configured url from your CMS) you'll need some
sort of code to handle that, and this makes it easy.

## Installation

### NuGet

```powershell
PM> Install-Package RobotsTxtCore
```

### .Net CLI

```sh
> dotnet add package RobotsTxtCore
```

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

    public async Task<Memory<byte>> GetRobotsTxtAsync(CancellationToken cancellationToken) {
        var settings = await _context.Settings.FirstAsync();

        var builder = new RobotsTxtOptionsBuilder();

        string content;
        if(settings.RobotsTxt.AllowAll)
            content = builder.AllowAll().Build().ToString();
        else
            content = builder.DenyAll().Build().ToString();

        return Encoding.UTF8.GetBytes(content).AsMemory();
    }

    public async Task<TimeSpan> GetMaxAgeAsync(CancellationToken cancellationToken) {
        var settings = await _context.Settings.FirstAsync();
        return settings.RobotsTxt.MaxAge;
    }
}
```
