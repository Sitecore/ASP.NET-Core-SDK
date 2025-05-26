using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Sitecore.AspNetCore.SDK.Pages.Configuration;
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
    /// <param name="renderingEngineOptions">The RenderingEngineOptions, used to retrieve a list of all registered renderings for the applications.</param>
    public class PagesSetupController(IOptions<PagesOptions> options, ILogger<PagesSetupController> logger, IOptions<RenderingEngineOptions> renderingEngineOptions) : ControllerBase
    {
        private readonly PagesOptions _options = options != null ? options.Value : throw new ArgumentNullException(nameof(options));
        private readonly ILogger<PagesSetupController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly RenderingEngineOptions _renderingEngineOptions = renderingEngineOptions != null ? renderingEngineOptions.Value : throw new ArgumentNullException(nameof(renderingEngineOptions));

        /// <summary>
        /// The Config endpoint used to inform Pages how this editing host is configured and which components are implemented.
        /// </summary>
        /// <returns>PagesConfigResponse object.</returns>
        [HttpGet]
        [HttpOptions]
        public ActionResult<PagesConfigResponse> Config()
        {
            if (IsValidPagesConfigRequest(Request))
            {
                _logger.LogDebug("{Message}", Resources.Debug_ProcessingValidPagesConfigRequest);
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
            IActionResult result;
            if (IsValidPagesRenderRequest(Request))
            {
                _logger.LogDebug("{Message}", Resources.Debug_ProcessingValidPagesRenderRequest);
                PagesRenderArgs args = ParseQueryStringArgs(Request);
                result = Redirect($"{args.Route}?{Constants.QueryStringKeys.Mode}={args.Mode}&{Constants.QueryStringKeys.ItemId}={args.ItemId}&{Constants.QueryStringKeys.Version}={args.Version}&{Constants.QueryStringKeys.Language}={args.Language}&{Constants.QueryStringKeys.Site}={args.Site}&{Constants.QueryStringKeys.LayoutKind}={args.LayoutKind}&{Constants.QueryStringKeys.Secret}={args.EditingSecret}&{Constants.QueryStringKeys.TenantId}={args.TenantId}&{Constants.QueryStringKeys.Route}={args.Route}");
            }
            else
            {
                result = BadRequest();
            }

            return result;
        }

        private static PagesRenderArgs ParseQueryStringArgs(HttpRequest request)
        {
            return new PagesRenderArgs
            {
                ItemId = Guid.TryParse(request.Query[Constants.QueryStringKeys.ItemId].FirstOrDefault(), out Guid itemId) ? itemId : Guid.Empty,
                EditingSecret = request.Query[Constants.QueryStringKeys.Secret].FirstOrDefault() ?? string.Empty,
                Language = request.Query[Constants.QueryStringKeys.Language].FirstOrDefault() ?? string.Empty,
                LayoutKind = request.Query[Constants.QueryStringKeys.LayoutKind].FirstOrDefault() ?? string.Empty,
                Mode = request.Query[Constants.QueryStringKeys.Mode].FirstOrDefault() ?? string.Empty,
                Route = request.Query[Constants.QueryStringKeys.Route].FirstOrDefault() ?? string.Empty,
                Site = request.Query[Constants.QueryStringKeys.Site].FirstOrDefault() ?? string.Empty,
                Version = int.TryParse(request.Query[Constants.QueryStringKeys.Version].FirstOrDefault(), out int version) ? version : 0,
                TenantId = request.Query[Constants.QueryStringKeys.TenantId].FirstOrDefault() ?? string.Empty,
            };
        }

        private bool IsValidPagesRenderRequest(HttpRequest httpRequest)
        {
            bool result = false;
            if (IsValidEditingSecret(httpRequest))
            {
                result = true;
            }
            else
            {
                _logger.LogError("{Message}", Resources.Error_InvalidPagesEditingSecretValue);
            }

            return result;
        }

        private PagesConfigResponse BuildConfigResponseBody()
        {
            return new PagesConfigResponse
            {
                EditMode = "metadata",
                Components = _renderingEngineOptions.RendererRegistry.Where(x => x.Value.ComponentName != string.Empty).Select(x => x.Value.ComponentName).ToList()
            };
        }

        private void SetConfigResponseHeaders(HttpResponse httpResponse)
        {
            httpResponse.Headers.ContentSecurityPolicy = $"frame-ancestors 'self' {_options.ValidOrigins} {_options.ValidEditingOrigin}";
            httpResponse.Headers.AccessControlAllowOrigin = _options.ValidEditingOrigin;
            httpResponse.Headers.AccessControlAllowMethods = _options.ValidMethods;
            httpResponse.Headers.AccessControlAllowHeaders = _options.ValidHeaders;
            httpResponse.StatusCode = StatusCodes.Status200OK;
            httpResponse.ContentType = "application/json";
        }

        private bool IsValidPagesConfigRequest(HttpRequest httpRequest)
        {
            return IsValidEditingSecret(httpRequest) && RequestHasValidEditingOrigin(httpRequest);
        }

        private bool IsValidEditingSecret(HttpRequest httpRequest)
        {
            bool result = false;
            if (httpRequest.Query.TryGetValue(Constants.QueryStringKeys.Secret, out StringValues editingSecretValues))
            {
                string editingSecret = editingSecretValues.FirstOrDefault() ?? string.Empty;
                if (editingSecret == _options.EditingSecret)
                {
                    result = true;
                }
            }

            if (!result)
            {
                _logger.LogError("{Message}", Resources.Error_InvalidPagesEditingSecretValue);
            }

            return result;
        }

        private bool RequestHasValidEditingOrigin(HttpRequest httpRequest)
        {
            bool result = false;
            if (httpRequest.Headers.Origin == _options.ValidEditingOrigin)
            {
                result = true;
            }
            else
            {
                _logger.LogError("{Message}", Resources.Error_InvalidPagesEditingOrigin);
            }

            return result;
        }
    }
}