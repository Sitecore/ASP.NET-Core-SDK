using System.Text;
using AwesomeAssertions;
using Microsoft.AspNetCore.Html;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Rendering;

public class EditableChromeRendererFixture
{
    [Theory]
    [AutoNSubstituteData]
    public void Render_IsGuarded(
        EditableChromeRenderer sut,
        EditableChrome chrome)
    {
        // Arrange
        chrome.Name = string.Empty;
        Func<IHtmlContent> actNull =
            () => sut.Render(null!);
        Func<IHtmlContent> actChrome =
            () => sut.Render(chrome);

        // Act / Assert
        actNull.Should().Throw<ArgumentNullException>();
        actChrome.Should().Throw<ArgumentException>();
    }

    [Theory]
    [AutoNSubstituteData]
    public void Render_WithValidChromeObject_ReturnsExpectedChromeHtml(
        EditableChromeRenderer sut,
        EditableChrome chrome)
    {
        // Act
        IHtmlContent result = sut.Render(chrome);
        string expectedHtml = BuildChromeHtml(chrome);

        // Assert
        result.ToString().Should().Be(expectedHtml);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Render_WithChromeObjectWithoutAttributes_ReturnsExpectedChromeHtml(
        EditableChromeRenderer sut,
        EditableChrome chrome)
    {
        // Arrange
        chrome.Attributes.Clear();

        // Act
        IHtmlContent result = sut.Render(chrome);
        string expectedHtml = BuildChromeHtml(chrome);

        // Assert
        result.ToString().Should().Be(expectedHtml);
    }

    private static string BuildChromeHtml(EditableChrome chrome)
    {
        StringBuilder chromeTagString = new($"<{chrome.Name}");
        foreach (string attributeKey in chrome.Attributes.Keys)
        {
            chromeTagString.Append($" {attributeKey}='{chrome.Attributes[attributeKey]}'");
        }

        chromeTagString.Append(">");
        chromeTagString.Append($"{chrome.Content}");
        chromeTagString.Append($"</{chrome.Name}>");

        return chromeTagString.ToString();
    }
}