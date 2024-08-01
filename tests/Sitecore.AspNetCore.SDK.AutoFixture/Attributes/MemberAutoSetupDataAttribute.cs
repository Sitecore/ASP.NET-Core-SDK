using System.Globalization;
using System.Reflection;
using Xunit;

namespace Sitecore.AspNetCore.SDK.AutoFixture.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class MemberAutoSetupDataAttribute : MemberDataAttributeBase
{
    public MemberAutoSetupDataAttribute(string memberName, params object[] parameters)
        : base(memberName, parameters)
    {
        AttrFactory = values => new InlineAutoSetupDataAttribute(values);
    }

    public MemberAutoSetupDataAttribute(string[] fixtureSetups, string memberName, params object[] parameters)
        : base(memberName, parameters)
    {
        AttrFactory = values => new InlineAutoSetupDataAttribute(fixtureSetups, values);
    }

    public MemberAutoSetupDataAttribute(Type externalClassSource, string memberName, params object[] parameters)
        : base(memberName, parameters)
    {
        AttrFactory = values => new InlineAutoSetupDataAttribute(externalClassSource, values);
    }

    public MemberAutoSetupDataAttribute(Type externalClassSource, string[] fixtureSetups, string memberName, params object[] parameters)
        : base(memberName, parameters)
    {
        AttrFactory = values => new InlineAutoSetupDataAttribute(externalClassSource, fixtureSetups, values);
    }

    protected Func<object[], InlineAutoSetupDataAttribute> AttrFactory { get; set; }

    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        foreach (InlineAutoSetupDataAttribute attr in base.GetData(testMethod).Select(AttrFactory))
        {
            foreach (object[]? parameters in attr.GetData(testMethod))
            {
                yield return parameters;
            }
        }
    }

    protected override object[]? ConvertDataItem(MethodInfo testMethod, object? item)
    {
        if (item == null)
        {
            return null;
        }

        if (item is not object[] array)
        {
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Property {0} on {1} yielded an item that is not an object[]", MemberName, MemberType ?? testMethod.DeclaringType));
        }

        return array;
    }
}