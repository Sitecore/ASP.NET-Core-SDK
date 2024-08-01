namespace Sitecore.AspNetCore.SDK.AutoFixture.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class MemberAutoNSubstituteDataAttribute : MemberAutoSetupDataAttribute
{
    public MemberAutoNSubstituteDataAttribute(string memberName, params object[] parameters)
        : base(memberName, parameters)
    {
        AttrFactory = values => new InlineAutoNSubstituteDataAttribute(values);
    }

    public MemberAutoNSubstituteDataAttribute(string[] fixtureSetups, string memberName, params object[] parameters)
        : base(memberName, parameters)
    {
        AttrFactory = values => new InlineAutoNSubstituteDataAttribute(fixtureSetups, values);
    }

    public MemberAutoNSubstituteDataAttribute(Type externalClassSource, string memberName, params object[] parameters)
        : base(memberName, parameters)
    {
        AttrFactory = values => new InlineAutoNSubstituteDataAttribute(externalClassSource, values);
    }

    public MemberAutoNSubstituteDataAttribute(Type externalClassSource, string[] fixtureSetups, string memberName, params object[] parameters)
        : base(memberName, parameters)
    {
        AttrFactory = values => new InlineAutoNSubstituteDataAttribute(externalClassSource, fixtureSetups, values);
    }
}