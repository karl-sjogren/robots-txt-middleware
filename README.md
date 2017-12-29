# RobotsTxtMiddleware [![Build Status](https://travis-ci.org/karl-sjogren/robots-txt-middleware.svg?branch=master)](https://travis-ci.org/karl-sjogren/robots-txt-middleware)

A Robots.txt middleware for ASP.NET Core.

## Usage
```csharp
    app.UseRobotsTxtMiddleware(builder =>
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
                    .Disallow("/")
                )
            .AddSitemap("sitemap.xml")
    );
```
