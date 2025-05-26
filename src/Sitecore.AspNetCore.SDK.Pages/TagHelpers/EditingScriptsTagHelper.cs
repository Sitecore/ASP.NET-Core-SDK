using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Sitecore.AspNetCore.SDK.Pages.Properties;
using Sitecore.AspNetCore.SDK.Pages.Response;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;

namespace Sitecore.AspNetCore.SDK.Pages.TagHelpers
{
    /// <summary>
    /// EditingScriptsTagHelper, used to output the script tags into the page header when running in Chromes editing mode.
    /// </summary>
    [HtmlTargetElement(Constants.SitecoreTagHelpers.EditScriptsHtmlTag)]
    public class EditingScriptsTagHelper : TagHelper
    {
        /// <summary>
        /// Gets or sets the current view context for the tag helper.
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext? ViewContext { get; set; }

        /// <inheritdoc />
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            ISitecoreRenderingContext renderingContext = ViewContext?.HttpContext.GetSitecoreRenderingContext() ??
                                             throw new NullReferenceException(Resources.Exception_EditingScriptsTagHelperSitecoreRenderingContextNull);

            output.TagName = string.Empty;
            string html = string.Empty;

            if (renderingContext.Response?.Content?.Sitecore?.Context?.IsEditing ?? false)
            {
                EditingContext? editingContext = JsonSerializer.Deserialize<EditingContext>(renderingContext.Response?.Content.ContextRawData ?? string.Empty);
                if (editingContext == null)
                {
                    throw new NullReferenceException(Resources.Exception_EditingScriptsTagHelperUnableToProcessContextRawData);
                }

                foreach (string script in editingContext.ClientScripts ?? [])
                {
                    html += $"<script type=\"text/javascript\" src=\"{script}\"></script>";
                }

                html += $@"
                            <script id=""hrz-canvas-state"" type=""application/json"">
                                {{
                                    ""itemId"":""{editingContext.ClientData?.CanvasState?.ItemId}"",
                                    ""itemVersion"":{editingContext.ClientData?.CanvasState?.ItemVersion},
                                    ""siteName"":""{editingContext.ClientData?.CanvasState?.SiteName}"",
                                    ""language"":""{editingContext.ClientData?.CanvasState?.Language}"",
                                    ""deviceId"":""{editingContext.ClientData?.CanvasState?.DeviceId}"",
                                    ""pageMode"":""{editingContext.ClientData?.CanvasState?.PageMode}"",
                                    ""variant"":""{editingContext.ClientData?.CanvasState?.Variant}""
                                }}
                            </script>
                        ";

                html += $"<script id=\"hrz-canvas-verification-token\" type=\"application/json\">{editingContext.ClientData?.CanvasVerificationToken}</script>";
            }

            output.Content.SetHtmlContent(html);
        }
    }
}
