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

## Changes from 3.0 to 4.0

`services.AddStaticRobotsTxt` is now marked as obsolete in favor of `services.AddRobotsTxt`.
After adding support for multiple hostnames and environments it didn't really feel like it
was "static" anymore.

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

Since version 3.0 you can call AddStaticRobotsTxt multiple times and specify different
settings for different environments. If there is a matching envrionment it will be used,
otherwise it will fall back to any configuration without an environment specified.

```csharp
public void ConfigureServices(IServiceCollection services) {
    services.AddStaticRobotsTxt(builder =>
        builder
            .ForEnvironment("Production")
            .AddSection(section =>
                section
                    .AddComment("Allow Googlebot")
                    .AddUserAgent("Googlebot")
                    .Allow("/")
                )
    );

    services.AddStaticRobotsTxt(builder =>
        builder
            .DenyAll()
    );
}

public void Configure(IApplicationBuilder app) {
    app.UseRobotsTxt();
}
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

    public async Task<RobotsTxtResult> GetResultAsync(CancellationToken cancellationToken) {
        var settings = await _context.Settings.FirstAsync();

        var builder = new RobotsTxtOptionsBuilder();

        RobotsTxtOptions options;
        if(settings.AllowAllRobots)
            options = builder.AllowAll().Build();
        else
            options = builder.DenyAll().Build();

        var content = options.ToString();
        var buffer = Encoding.UTF8.GetBytes(content).AsMemory();
        return new RobotsTxtResult(buffer, settings.RobotsTxtMaxAge);
    }
}
```
