using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Sitecore.AspNetCore.SDK.Pages.Configuration;
using Sitecore.AspNetCore.SDK.Pages.Middleware;
using Sitecore.AspNetCore.SDK.Pages.Models;
using Sitecore.AspNetCore.SDK.Pages.Properties;
using Sitecore.AspNetCore.SDK.RenderingEngine.Configuration;

namespace Sitecore.AspNetCore.SDK.Pages.Controllers
{
    /// <summary>
    /// Pages Setup Controller, used to handle the configuration requests that MetaData editing mode uses to validate the editing host can connect successfully.
    /// </summary>
    /// <param name="options">The Sitecore Pages configuration options.</param>
    /// <param name="logger">The <see cref="ILogger"/> to use for logging.</param>
    /// <param name="renderingEngineOptions">The RenderingEngineOptions, used to retriece a list of all registered renderings for the applications.</param>
    public class PagesSetupController(IOptions<PagesOptions> options, ILogger<PagesSetupController> logger, IOptions<RenderingEngineOptions> renderingEngineOptions) : ControllerBase
    {
        private readonly PagesOptions options = options != null ? options.Value : throw new ArgumentNullException(nameof(options));
        private readonly ILogger<PagesSetupController> logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly RenderingEngineOptions renderingEngineOptions = renderingEngineOptions != null ? renderingEngineOptions.Value : throw new ArgumentNullException(nameof(renderingEngineOptions));

        /// <summary>
        /// The Config endpoint used to inform Pages how this editing host is configured and which components are implemented.
        /// </summary>
        /// <returns>PagesConfigResponse object.</returns>
        [HttpGet]
        public ActionResult<PagesConfigResponse> Config()
        {
            if (IsValidPagesConfigRequest(Request))
            {
                logger.LogDebug(Resources.Debug_ProcessingValidPagesConfigRequest);
                SetConfigResponseHeaders(Response);
                return Ok(BuildConfigResponseBody());
            }

            return BadRequest();
        }

        /// <summary>
        /// The render endpoint used to redirect the Pages application to the correct page.
        /// </summary>
        /// <returns>Redirect response.</returns>
        [HttpGet]
        public IActionResult Render()
        {
            if (IsValidPagesRenderRequest(Request))
            {
                logger.LogDebug(Resources.Debug_ProcessingValidPagesRenderRequest);
                PagesRenderArgs args = ParseQueryStringArgs(Request);
                return Redirect($"{args.Route}?mode={args.Mode}&sc_itemid={args.ItemId}&sc_version={args.Version}&sc_lang={args.Language}&sc_site={args.Site}&sc_layoutKind={args.LayoutKind}&secret={args.EditingSecret}&tenant_id={args.TenantId}&route={args.Route}");
            }

            return BadRequest();
        }

        private PagesRenderArgs ParseQueryStringArgs(HttpRequest request)
        {
            return new PagesRenderArgs
            {
                ItemId = Guid.TryParse(request.Query["sc_itemid"].FirstOrDefault(), out Guid itemId) ? itemId : Guid.Empty,
                EditingSecret = request.Query["secret"].FirstOrDefault() ?? string.Empty,
                Language = request.Query["sc_lang"].FirstOrDefault() ?? string.Empty,
                LayoutKind = request.Query["sc_layoutKind"].FirstOrDefault() ?? string.Empty,
                Mode = request.Query["mode"].FirstOrDefault() ?? string.Empty,
                Route = request.Query["route"].FirstOrDefault() ?? string.Empty,
                Site = request.Query["sc_site"].FirstOrDefault() ?? string.Empty,
                Version = int.TryParse(request.Query["sc_version"].FirstOrDefault(), out int version) ? version : 0,
                TenantId = request.Query["tenant_id"].FirstOrDefault() ?? string.Empty,
            };
        }

        private bool IsValidPagesRenderRequest(HttpRequest httpRequest)
        {
            ArgumentNullException.ThrowIfNull(httpRequest);

            if (!IsValidEditingSecret(httpRequest))
            {
                logger.LogError(Resources.Error_InvalidPagesEditingSecretValue);
                return false;
            }

            return true;
        }

        private PagesConfigResponse BuildConfigResponseBody()
        {
            return new PagesConfigResponse
            {
                EditMode = "metadata",
                Components = renderingEngineOptions.RendererRegistry.Where(x => x.Value.ComponentName != string.Empty).Select(x => x.Value.ComponentName).ToList()
            };
        }

        private void SetConfigResponseHeaders(HttpResponse httpResponse)
        {
            httpResponse.Headers.ContentSecurityPolicy = $"frame-ancestors 'self' {options.ValidOrigins} {options.ValidEditingOrigin}";
            httpResponse.Headers.AccessControlAllowOrigin = options.ValidEditingOrigin;
            httpResponse.Headers.AccessControlAllowMethods = "GET, POST, OPTIONS, PUT, PATCH, DELETE";
            httpResponse.StatusCode = StatusCodes.Status200OK;
            httpResponse.ContentType = "application/json";
        }

        private bool IsValidPagesConfigRequest(HttpRequest httpRequest)
        {
            ArgumentNullException.ThrowIfNull(httpRequest);

            if (!IsValidEditingSecret(httpRequest))
            {
                logger.LogError(Resources.Error_InvalidPagesEditingSecretValue);
                return false;
            }

            if (!RequestHasValidEditingOrigin(httpRequest))
            {
                logger.LogError(Resources.Error_InvalidPagesEditingOrigin);
                return false;
            }

            return true;
        }

        private bool IsValidEditingSecret(HttpRequest httpRequest)
        {
            if (httpRequest.Query.TryGetValue("secret", out StringValues editingSecretValues))
            {
                string editingSecret = editingSecretValues.FirstOrDefault() ?? string.Empty;
                if (editingSecret == options.EditingSecret)
                {
                    return true;
                }
            }

            return false;
        }

        private bool RequestHasValidEditingOrigin(HttpRequest httpRequest)
        {
            if (httpRequest.Headers.Origin == options.ValidEditingOrigin)
            {
                return true;
            }

            return false;
        }
    }
}
