# RobotsTxtMiddleware [![Build Status](https://travis-ci.org/karl-sjogren/robots-txt-middleware.svg?branch=master)](https://travis-ci.org/karl-sjogren/robots-txt-middleware)

A Robots.txt middleware for ASP.NET Core. Why is this needed you ask? Because if you need to add dynamic values (such as a configured url from your CMS) you'll need some sort of code to handle that, and this makes it easy.

## Installation
```Install-Package RobotsTxtCore```

https://www.nuget.org/packages/RobotsTxtCore/

## Usage
To specify multiple rules with the fluent interface makes it really easy.

```csharp
    app.UseRobotsTxt(builder =>
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
```

Output

```
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
    app.UseRobotsTxt(builder =>
        builder
            .DenyAll()
    );
```

Output
```
User-agent: *
Disallow: /
```
