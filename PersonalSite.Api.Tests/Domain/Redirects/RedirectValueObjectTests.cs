using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Domain.Redirects;

namespace PersonalSite.Api.Tests.Domain.Redirects;

public sealed class RedirectValueObjectTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("CV")]
    [InlineData("cv_en")]
    [InlineData("-cv")]
    [InlineData("cv-")]
    [InlineData("cv--en")]
    [InlineData("cví")]
    [InlineData("cv\n")]
    public void Slug_constructor_enforces_exact_format(string? value) =>
        Assert.Throws<DomainException>(() => new RedirectSlug(value));

    [Fact]
    public void Slug_has_value_equality_and_bounded_storage_value()
    {
        var slug = new RedirectSlug("application-2026-01");
        Assert.Equal(slug, new RedirectSlug("application-2026-01"));
        Assert.NotEqual(slug, new RedirectSlug("cv"));
        string value = slug;
        Assert.Equal("application-2026-01", value);
        Assert.Equal(value, slug.ToString());
        Assert.Equal(new string('a', 80), new RedirectSlug(new string('a', 80)).Value);
        Assert.Throws<DomainException>(() => new RedirectSlug(new string('a', 81)));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("/relative")]
    [InlineData("//example.test")]
    [InlineData("javascript:alert(1)")]
    [InlineData("ftp://example.test/file")]
    [InlineData("https://user:password@example.test")]
    [InlineData("https://example.test/with space")]
    [InlineData("https://example.test/\r\nLocation:bad")]
    [InlineData("https://example.test\\bad")]
    public void Target_constructor_rejects_invalid_or_unsafe_urls(string? value) =>
        Assert.Throws<DomainException>(() => new RedirectTargetUrl(value));

    [Theory]
    [InlineData("http://example.test/path?source=cv#section")]
    [InlineData("https://example.test/path%2Fkept?q=a%20b#section")]
    public void Target_preserves_valid_url_exactly_and_has_value_equality(string value)
    {
        var target = new RedirectTargetUrl(value);
        Assert.Equal(target, new RedirectTargetUrl(value));
        Assert.Equal(value, target.Value);
        string text = target;
        Assert.Equal(value, text);
        Assert.Equal(value, target.ToString());
    }

    [Fact]
    public void Target_enforces_existing_length_limit()
    {
        const string prefix = "https://example.test/";
        var value = prefix + new string('a', 2048 - prefix.Length);
        Assert.Equal(value, new RedirectTargetUrl(value).Value);
        Assert.Throws<DomainException>(() => new RedirectTargetUrl(value + "a"));
    }

    [Fact]
    public void Campaign_is_a_trimmed_nonblank_label_with_value_equality()
    {
        var campaign = new RedirectCampaign("  english-cv  ");
        Assert.Equal("english-cv", campaign.Value);
        Assert.Equal(new RedirectCampaign("english-cv"), campaign);
        Assert.NotEqual(new RedirectCampaign("other"), campaign);
        string value = campaign;
        Assert.Equal("english-cv", value);
        Assert.Equal(value, campaign.ToString());
        Assert.Null(RedirectCampaign.CreateOptional(null));
        Assert.Null(RedirectCampaign.CreateOptional(""));
        Assert.Null(RedirectCampaign.CreateOptional("   "));
        Assert.Equal(campaign, RedirectCampaign.CreateOptional("  english-cv  "));
        Assert.Equal(200, new RedirectCampaign(new string('a', 200)).Value.Length);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("two\nlines")]
    [InlineData("label\tvalue")]
    public void Campaign_constructor_rejects_invalid_required_values(string? value) =>
        Assert.Throws<DomainException>(() => new RedirectCampaign(value));

    [Theory]
    [InlineData("\n")]
    [InlineData("\t")]
    [InlineData("two\r\nlines")]
    public void Optional_campaign_does_not_silently_clear_control_characters(string value) =>
        Assert.Throws<DomainException>(() => RedirectCampaign.CreateOptional(value));

    [Fact]
    public void Campaign_validates_raw_length_before_trimming_or_clearing()
    {
        Assert.Throws<DomainException>(() => new RedirectCampaign(new string('a', 201)));
        Assert.Throws<DomainException>(() => RedirectCampaign.CreateOptional(new string(' ', 201)));
        Assert.Throws<DomainException>(() => RedirectCampaign.CreateOptional(" " + new string('a', 200)));
    }
}
