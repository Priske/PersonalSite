using PersonalSite.Api.Domain.Common;
using PersonalSite.Api.Domain.Exceptions;

namespace PersonalSite.Api.Tests.Domain.Common;

public sealed class UrlTests
{
    [Fact]
    public void Constructor_WithValidHttpsUrl_CreatesUrl()
    {
        var url = new Url("https://github.com");

        Assert.Equal("https://github.com/", url.Value);
    }

    [Fact]
    public void Constructor_WithValidHttpUrl_CreatesUrl()
    {
        var url = new Url("http://localhost:3000");

        Assert.Equal("http://localhost:3000/", url.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void Constructor_WithEmptyValue_ThrowsDomainException(string value)
    {
        Assert.Throws<DomainException>(() => new Url(value));
    }

    [Theory]
    [InlineData("not a url")]
    [InlineData("github.com")]
    [InlineData("http:/github.com")]
    public void Constructor_WithInvalidUrl_ThrowsDomainException(string value)
    {
        Assert.Throws<DomainException>(() => new Url(value));
    }

    [Theory]
    [InlineData("ftp://example.com")]
    [InlineData("mailto:test@example.com")]
    public void Constructor_WithUnsupportedScheme_ThrowsDomainException(string value)
    {
        Assert.Throws<DomainException>(() => new Url(value));
    }

    [Fact]
    public void ImplicitOperator_ReturnsValue()
    {
        var url = new Url("https://github.com");

        string value = url;

        Assert.Equal("https://github.com/", value);
    }

    [Fact]
    public void ToString_ReturnsValue()
    {
        var url = new Url("https://github.com");

        Assert.Equal("https://github.com/", url.ToString());
    }
}