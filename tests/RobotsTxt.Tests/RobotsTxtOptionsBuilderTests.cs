using System;
using Shouldly;
using Xunit;

namespace RobotsTxt.Tests {
    public class RobotsTxtOptionsBuilderTests {
        [Theory]
        [InlineData("/sitemap.xml")]
        [InlineData("/")]
        [InlineData("")]
        [InlineData(null)]
        public void ShouldThrowOnInvalidSitemapUrl(string url) {
            var builder = new RobotsTxtOptionsBuilder();

            Should.Throw<ArgumentException>(() => builder.AddSitemap(url));
        }

        [Fact]
        public void ShouldClearPreviousSectionsWhenCallingAllowAll() {
            var builder = new RobotsTxtOptionsBuilder();

            builder
                .AddSection(section => section.AddUserAgent("Googlebot"))
                .AddSection(section => section.AddUserAgent("Bingbot"))
                .AllowAll();

            var options = builder.Build();

            options.Sections.Count.ShouldBe(1);
        }

        [Fact]
        public void ShouldClearPreviousSectionsWhenCallingDenyAll() {
            var builder = new RobotsTxtOptionsBuilder();

            builder
                .AddSection(section => section.AddUserAgent("Googlebot"))
                .AddSection(section => section.AddUserAgent("Bingbot"))
                .DenyAll();

            var options = builder.Build();

            options.Sections.Count.ShouldBe(1);
        }
    }
}
