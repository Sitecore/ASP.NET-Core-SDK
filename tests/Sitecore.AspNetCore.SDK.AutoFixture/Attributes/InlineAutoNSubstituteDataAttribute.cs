using System.Diagnostics.CodeAnalysis;

namespace Sitecore.AspNetCore.SDK.AutoFixture.Attributes;

[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class InlineAutoNSubstituteDataAttribute : InlineAutoSetupDataAttribute
{
    public InlineAutoNSubstituteDataAttribute(Type externalClassSource, string[] fixtureSetups, params object[] values)
        : base(new AutoNSubstituteDataAttribute(externalClassSource, fixtureSetups), values)
    {
    }

    public InlineAutoNSubstituteDataAttribute(string[] fixtureSetups, params object[] values)
        : base(new AutoNSubstituteDataAttribute(fixtureSetups), values)
    {
    }

    public InlineAutoNSubstituteDataAttribute(params object[] values)
        : base(new AutoNSubstituteDataAttribute(), values)
    {
    }
}