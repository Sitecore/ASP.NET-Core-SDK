using AutoFixture.Xunit2;

namespace Sitecore.AspNetCore.SDK.AutoFixture.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class InlineAutoSetupDataAttribute(AutoDataAttribute baseAttribute, params object[] values)
    : InlineAutoDataAttribute(baseAttribute, values)
{
    public InlineAutoSetupDataAttribute(Type externalClassSource, string[] fixtureSetups, params object[] values)
        : this(new AutoSetupDataAttribute(externalClassSource, fixtureSetups), values)
    {
    }

    public InlineAutoSetupDataAttribute(Type externalClassSource, params object[] values)
        : this(new AutoSetupDataAttribute(externalClassSource), values)
    {
    }

    public InlineAutoSetupDataAttribute(string[] fixtureSetups, params object[] values)
        : this(new AutoSetupDataAttribute(fixtureSetups), values)
    {
    }

    public InlineAutoSetupDataAttribute(params object[] values)
        : this(new AutoSetupDataAttribute(), values)
    {
    }
}