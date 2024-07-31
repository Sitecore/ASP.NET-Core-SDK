using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sitecore.AspNetCore.SDK.ExperienceEditor.Configuration;
using Sitecore.AspNetCore.SDK.ExperienceEditor.Mappers;
using Sitecore.AspNetCore.SDK.ExperienceEditor.Models;
using Sitecore.AspNetCore.SDK.LayoutService.Client;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;

namespace Sitecore.AspNetCore.SDK.ExperienceEditor.Middleware;

/// <summary>
/// The Experience Editor middleware implementation that handles POST requests from the Sitecore Experience Editor
/// and wraps the response HTML in a JSON format.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ExperienceEditorMiddleware"/> class.
/// </remarks>
/// <param name="next">The next middleware to call.</param>
/// <param name="options">The experience editor configuration options.</param>
/// <param name="serializer">A configured instance of <see cref="ISitecoreLayoutSerializer"/>.</param>
/// <param name="logger">The <see cref="ILogger"/> to use for logging.</param>
public class ExperienceEditorMiddleware(RequestDelegate next, IOptions<ExperienceEditorOptions> options, ISitecoreLayoutSerializer serializer, ILogger<ExperienceEditorMiddleware> logger)
{
    private readonly ISitecoreLayoutSerializer _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
    private readonly RequestDelegate _next = next ?? throw new ArgumentNullException(nameof(next));
    private readonly ExperienceEditorOptions _options = options != null ? options.Value : throw new ArgumentNullException(nameof(options));
    private readonly ILogger<ExperienceEditorMiddleware> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly SitecoreLayoutResponseMapper _responseMapper = new(options);

    /// <summary>
    /// The middleware Invoke method.
    /// </summary>
    /// <param name="httpContext">The current <see cref="HttpContext"/>.</param>
    /// <returns>A Task to support async calls.</returns>
    public async Task Invoke(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        if (IsExperienceEditorRequest(httpContext.Request))
        {
            ExperienceEditorPostModel? postModel = await TryParseContentFromRequestBodyAsync(httpContext).ConfigureAwait(false);

            if (postModel == null || !CheckJssEditingSecret(postModel, httpContext) || !TrySetHttpContextFeaturesForNextHandler(httpContext, postModel))
            {
                return;
            }

            Stream realResponseStream = httpContext.Response.Body;
            try
            {
                MemoryStream tmpResponseBuffer = new();

                httpContext.Response.Body = tmpResponseBuffer;

                await _next(httpContext).ConfigureAwait(false);

                tmpResponseBuffer.Position = 0;
                string responseBody = await new StreamReader(tmpResponseBuffer).ReadToEndAsync().ConfigureAwait(false);

                await using StreamWriter realResponseWriter = new(realResponseStream);
                await realResponseWriter.WriteAsync("{\"html\":").ConfigureAwait(false);

                string html = JsonSerializer.Serialize(responseBody);
                await realResponseWriter.WriteAsync(html).ConfigureAwait(false);
                await realResponseWriter.WriteAsync("}").ConfigureAwait(false);
            }
            finally
            {
                httpContext.Response.Body = realResponseStream;
            }
        }
        else
        {
            await _next(httpContext).ConfigureAwait(false);
        }
    }

    private static async Task<ExperienceEditorPostModel> ParseContentFromRequestBodyAsync(HttpContext context)
    {
        using StreamReader reader = new(context.Request.Body);
        string body = await reader.ReadToEndAsync().ConfigureAwait(false);
        if (string.IsNullOrEmpty(body))
        {
            throw new FormatException("Empty request body");
        }

        return JsonSerializer.Deserialize<ExperienceEditorPostModel>(body, JsonLayoutServiceSerializer.GetDefaultSerializerOptions()) ?? new ExperienceEditorPostModel();
    }

    private static string GetSitecoreItemPathFromRequestBody(ExperienceEditorPostModel postModel)
    {
        string? result = string.Empty;
        if (JsonDocument.Parse(postModel.Args[1]).RootElement.TryGetProperty(LayoutServiceClientConstants.Serialization.SitecoreDataPropertyName, out JsonElement sitecore)
            && sitecore.TryGetProperty(LayoutServiceClientConstants.Serialization.ContextPropertyName, out JsonElement context)
            && context.TryGetProperty("itemPath", out JsonElement path))
        {
            result = path.GetString();
        }

        if (string.IsNullOrWhiteSpace(result)
            && JsonDocument.Parse(postModel.Args[2]).RootElement.TryGetProperty("httpContext", out JsonElement httpContext)
            && httpContext.TryGetProperty("request", out JsonElement request)
            && request.TryGetProperty("path", out path))
        {
            // keep backwards compatibility in case people use an older JSS version that doesn't send the path in the context.
            result = path.GetString();
        }

        return result ?? string.Empty;
    }

    private bool IsExperienceEditorRequest(HttpRequest httpRequest)
    {
        ArgumentNullException.ThrowIfNull(httpRequest);
        return httpRequest.Method == HttpMethods.Post && httpRequest.Path.Value!.Equals(_options.Endpoint, StringComparison.InvariantCultureIgnoreCase);
    }

    private SitecoreLayoutResponseContent GetSitecoreLayoutContentFromRequestBody(ExperienceEditorPostModel postModel)
    {
        return _serializer.Deserialize(postModel.Args[1]) ?? new SitecoreLayoutResponseContent();
    }

    private string GetMappedPath(string scPath, SitecoreLayoutResponseContent response, HttpRequest request)
    {
        string? customMappedPath = _responseMapper.MapRoute(response, scPath, request);
        return string.IsNullOrWhiteSpace(customMappedPath) ? scPath : customMappedPath;
    }

    private bool TrySetHttpContextFeaturesForNextHandler(HttpContext httpContext, ExperienceEditorPostModel postModel)
    {
        try
        {
            // parse POST body and set rendering context
            SitecoreLayoutResponseContent content = GetSitecoreLayoutContentFromRequestBody(postModel);

            SitecoreRenderingContext scContext = new()
            {
                Response = new SitecoreLayoutResponse([])
                {
                    Content = content
                }
            };

            httpContext.SetSitecoreRenderingContext(scContext);

            // Changing POST request to GET stream
            httpContext.Request.Method = HttpMethods.Get;

            // Replace the request path with the item path from request metadata.
            // This is needed to support different routing or other kinds of path dependent logic.
            string scPath = GetSitecoreItemPathFromRequestBody(postModel);
            if (!string.IsNullOrWhiteSpace(scPath))
            {
                httpContext.Request.Path = GetMappedPath(scPath, content, httpContext.Request);
            }
            else
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                _logger.LogError("Error parsing Layout content from POST request: Empty Item path.");
                return false;
            }
        }

        // Disabled catching general exceptions because all exceptions in request parsing shall be treated as bad requests.
        catch (Exception exception)
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            _logger.LogError(exception, "Error parsing Layout content from POST request");
            return false;
        }

        return true;
    }

    private bool CheckJssEditingSecret(ExperienceEditorPostModel postModel, HttpContext httpContext)
    {
        string localSecret = _options.JssEditingSecret;
        if (string.IsNullOrEmpty(localSecret))
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            _logger.LogError("The JSS_EDITING_SECRET environment variable is missing or invalid.");
            return false;
        }

        string? secretFromRequest = postModel.JssEditingSecret;

        bool result = localSecret == secretFromRequest;
        if (!result)
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            _logger.LogError("Missing or invalid secret");
        }

        return result;
    }

    private async Task<ExperienceEditorPostModel?> TryParseContentFromRequestBodyAsync(HttpContext httpContext)
    {
        ExperienceEditorPostModel postModel;
        try
        {
            postModel = await ParseContentFromRequestBodyAsync(httpContext).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            _logger.LogError(exception, "Error parsing POST request");
            return null;
        }

        return postModel;
    }
}