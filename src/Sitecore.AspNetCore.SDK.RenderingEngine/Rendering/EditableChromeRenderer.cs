using System.Text;
using Microsoft.AspNetCore.Html;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;

/// <inheritdoc />
public class EditableChromeRenderer : IEditableChromeRenderer
{
    /// <inheritdoc />
    public IHtmlContent Render(EditableChrome chrome)
    {
        ArgumentNullException.ThrowIfNull(chrome);
        ArgumentException.ThrowIfNullOrWhiteSpace(chrome.Name, nameof(chrome) + nameof(chrome.Name));

        StringBuilder chromeTagString = new($"<{chrome.Name}");

        foreach (KeyValuePair<string, string> entry in chrome.Attributes)
        {
            chromeTagString.Append($" {entry.Key}='{entry.Value}'");
        }

        chromeTagString.Append('>');
        chromeTagString.Append($"{chrome.Content}");
        chromeTagString.Append($"</{chrome.Name}>");

        return new HtmlString(chromeTagString.ToString());
    }
}