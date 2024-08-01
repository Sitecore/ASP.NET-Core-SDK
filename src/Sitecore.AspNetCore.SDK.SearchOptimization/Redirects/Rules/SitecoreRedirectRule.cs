using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Logging;

namespace Sitecore.AspNetCore.SDK.SearchOptimization.Redirects.Rules;

/// <summary>
/// Rule that redirects the request when the Url matches a regular expression.
/// </summary>
internal class SitecoreRedirectRule : IRule
{
    private readonly TimeSpan _regexTimeout = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Initializes a new instance of the <see cref="SitecoreRedirectRule"/> class.
    /// </summary>
    /// <param name="regex">Regular Expression to match.</param>
    /// <param name="replacement">Replacement value for matches.</param>
    /// <param name="statusCode">Status code to respond with.</param>
    /// <param name="isQueryStringPreserved">Where the query string is retained after processing.</param>
    public SitecoreRedirectRule(string regex, string replacement, int statusCode, bool isQueryStringPreserved)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(regex);
        ArgumentException.ThrowIfNullOrWhiteSpace(replacement);

        InitialMatch = new Regex(regex, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase, _regexTimeout);
        Replacement = replacement;
        StatusCode = statusCode;
        IsQueryStringPreserved = isQueryStringPreserved;
    }

    /// <summary>
    /// Gets Compiled, CultureInvariant, IgnoreCase Regular Expression used by the rule.
    /// </summary>
    public Regex InitialMatch { get; }

    /// <summary>
    /// Gets Replacement value for matches.
    /// </summary>
    public string Replacement { get; }

    /// <summary>
    /// Gets the status code to respond with.
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    /// Gets a value indicating whether the query string is retained after processing.
    /// </summary>
    public bool IsQueryStringPreserved { get; }

    /// <inheritdoc />
    public void ApplyRule(RewriteContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        HttpRequest request = context.HttpContext.Request;
        PathString path = request.Path;
        PathString pathBase = request.PathBase;

        Match initMatchResults = InitialMatch.Match(!path.HasValue ? string.Empty : path.ToString()[1..]);

        if (initMatchResults.Success)
        {
            string newPath = initMatchResults.Result(Replacement);
            HttpResponse response = context.HttpContext.Response;

            response.StatusCode = StatusCode;
            context.Result = RuleResult.EndResponse;

            string encodedPath;

            if (string.IsNullOrEmpty(newPath))
            {
                encodedPath = pathBase.HasValue ? pathBase.Value : "/";
            }
            else
            {
                HostString host = default;
                int schemeSplit = newPath.IndexOf(Uri.SchemeDelimiter, StringComparison.Ordinal);
                string scheme = request.Scheme;

                if (schemeSplit >= 0)
                {
                    scheme = newPath[..schemeSplit];
                    schemeSplit += Uri.SchemeDelimiter.Length;
                    int pathSplit = newPath.IndexOf('/', schemeSplit);

                    if (pathSplit == -1)
                    {
                        host = new HostString(newPath[schemeSplit..]);
                        newPath = "/";
                    }
                    else
                    {
                        host = new HostString(newPath[schemeSplit..pathSplit]);
                        newPath = newPath[pathSplit..];
                    }
                }

                if (newPath[0] != '/')
                {
                    newPath = '/' + newPath;
                }

                QueryString resolvedQuery = IsQueryStringPreserved ? request.QueryString : QueryString.Empty;
                string resolvedPath = newPath;

                int querySplit = newPath.IndexOf('?', StringComparison.CurrentCultureIgnoreCase);
                if (querySplit >= 0)
                {
                    resolvedQuery = IsQueryStringPreserved ? request.QueryString.Add(QueryString.FromUriComponent(newPath[querySplit..])) : QueryString.FromUriComponent(newPath[querySplit..]);
                    resolvedPath = newPath[..querySplit];
                }

                encodedPath = host.HasValue
                    ? UriHelper.BuildAbsolute(scheme, host, pathBase, resolvedPath, resolvedQuery)
                    : UriHelper.BuildRelative(pathBase, resolvedPath, resolvedQuery);
            }

            // Not using the HttpContext.Response.redirect here because status codes may be 301, 302, 307, 308
            response.Headers.Location = encodedPath;

            context.Logger.LogInformation("Request was redirected to {NewPath}", newPath);
        }
    }
}