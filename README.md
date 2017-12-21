# RobotsTxtMiddleware
A Robots.txt middleware for ASP.NET Core

## Usage
```
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