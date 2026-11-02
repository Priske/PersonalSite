using PersonalSite.Api.Application.Redirects;

namespace PersonalSite.Api.Tests.IntegrationTests.Application.Analytics;

public sealed class RedirectValidationTests
{
    [Theory]
    [InlineData("")]
    [InlineData("cv\n")]
    [InlineData("cv\r\n")]
    [InlineData("-cv")]
    [InlineData("cv-")]
    [InlineData("cv--en")]
    [InlineData("cv_en")]
    [InlineData("CV")]
    [InlineData("cví")]
    public void Slug_requires_exact_lowercase_ascii_name(string slug) => Assert.False(RedirectLinkHandler.IsSlug(slug));

    [Fact]
    public void Bounds_and_blank_optional_campaign_are_validated_without_fetching_a_target()
    {
        Assert.True(RedirectLinkHandler.IsSlug(new string('a', 80)));
        Assert.False(RedirectLinkHandler.IsSlug(new string('a', 81)));
        var request = new SaveRedirectLinkRequest("https://example.test/" + new string('a', 2048), null, true);
        Assert.Contains("targetUrl", RedirectLinkHandler.Validate("cv", request).Keys);
        Assert.Contains("campaign", RedirectLinkHandler.Validate("cv", request with
        {
            TargetUrl = "https://example.test", Campaign = new string('a', 201)
        }).Keys);
        Assert.Empty(RedirectLinkHandler.Validate("cv-en", new("https://example.test/path?source=cv#section", "  ", true)));
    }
}
