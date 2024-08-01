using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Logging;

namespace Sitecore.AspNetCore.SDK.SearchOptimization.Redirects.Rules;

/// <summary>
/// Rule that rewrites the Url according to a regular expression.
/// </summary>
internal class SitecoreRewriteRule : IRule
{
    private readonly TimeSpan _regexTimeout = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Initializes a new instance of the <see cref="SitecoreRewriteRule"/> class.
    /// </summary>
    /// <param name="regex">Regular Expression to match.</param>
    /// <param name="replacement">Replacement value for matches.</param>
    /// <param name="stopProcessing">Whether to stop processing any additional rules or continue.</param>
    /// <param name="isQueryStringPreserved">Where the query string is retained after processing.</param>
    public SitecoreRewriteRule(string regex, string replacement, bool stopProcessing, bool isQueryStringPreserved)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(regex);
        ArgumentException.ThrowIfNullOrWhiteSpace(replacement);

        InitialMatch = new Regex(regex, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase, _regexTimeout);
        Replacement = replacement;
        StopProcessing = stopProcessing;
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
    /// Gets a value indicating whether to stop processing additional rules after this one.
    /// </summary>
    public bool StopProcessing { get; }

    /// <summary>
    /// Gets a value indicating whether the query string is retained after processing.
    /// </summary>
    public bool IsQueryStringPreserved { get; }

    /// <inheritdoc />
    public void ApplyRule(RewriteContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        PathString path = context.HttpContext.Request.Path;
        Match initMatchResults = InitialMatch.Match(path == PathString.Empty ? path.ToString() : path.ToString()[1..]);

        if (initMatchResults.Success)
        {
            string result = initMatchResults.Result(Replacement);
            HttpRequest request = context.HttpContext.Request;

            if (StopProcessing)
            {
                context.Result = RuleResult.SkipRemainingRules;
            }

            if (string.IsNullOrEmpty(result))
            {
                result = "/";
            }

            if (!IsQueryStringPreserved)
            {
                request.QueryString = QueryString.Empty;
            }

            if (result.Contains(Uri.SchemeDelimiter, StringComparison.Ordinal))
            {
                UriHelper.FromAbsolute(result, out string? scheme, out HostString host, out PathString pathString, out QueryString query, out _);

                request.Scheme = scheme;
                request.Host = host;
                request.Path = pathString;
                request.QueryString = query.Add(request.QueryString);
            }
            else
            {
                int split = result.IndexOf('?', StringComparison.InvariantCultureIgnoreCase);
                if (split >= 0)
                {
                    string newPath = result[..split];
                    request.Path = newPath[0] == '/' ? PathString.FromUriComponent(newPath) : PathString.FromUriComponent('/' + newPath);

                    request.QueryString = request.QueryString.Add(
                        QueryString.FromUriComponent(
                            result[split..]));
                }
                else
                {
                    request.Path = result[0] == '/' ? PathString.FromUriComponent(result) : PathString.FromUriComponent('/' + result);
                }
            }

            context.Logger.LogInformation("Request was rewritten to {Result}", result);
        }
    }
}