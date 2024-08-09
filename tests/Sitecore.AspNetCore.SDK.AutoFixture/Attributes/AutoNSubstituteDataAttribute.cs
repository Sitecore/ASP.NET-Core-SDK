using System.Diagnostics.CodeAnalysis;
using AutoFixture;
using AutoFixture.AutoNSubstitute;

namespace Sitecore.AspNetCore.SDK.AutoFixture.Attributes;

[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Method)]
public class AutoNSubstituteDataAttribute : AutoSetupDataAttribute
{
    public AutoNSubstituteDataAttribute(params string[] fixtureSetups)
        : base(fixtureSetups)
    {
    }

    public AutoNSubstituteDataAttribute(Type externalClassSource, params string[] fixtureSetups)
        : base(externalClassSource, fixtureSetups)
    {
    }

    protected override IEnumerable<Action<IFixture>> GetSetups(Type? functionSourceType)
    {
        return
            new Action<IFixture>[] { x => x.Customize(new AutoNSubstituteCustomization { ConfigureMembers = true }) }.Concat(
                base.GetSetups(functionSourceType));
    }
}