using Microsoft.AspNetCore.Http;
using Sitecore.AspNetCore.SDK.ExperienceEditor.Configuration;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;

namespace Sitecore.AspNetCore.SDK.ExperienceEditor.Extensions;

/// <summary>
/// Extensions to help configure <see cref="ExperienceEditorOptions"/>.
/// </summary>
public static class ExperienceEditorOptionsExtensions
{
    /// <summary>
    ///  Adds a custom mapping action.
    /// </summary>
    /// <param name="options">The <see cref="ExperienceEditorOptions"/> to configure.</param>
    /// <param name="mapAction">The mapping action to configure <see cref="HttpRequest"/>.</param>
    /// <returns>The <see cref="ExperienceEditorOptions"/> so that additional calls can be chained.</returns>
    public static ExperienceEditorOptions MapToRequest(this ExperienceEditorOptions options, Action<SitecoreLayoutResponseContent, string, HttpRequest> mapAction)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(mapAction);

        options.ItemMappings.Add(mapAction);

        return options;
    }
}