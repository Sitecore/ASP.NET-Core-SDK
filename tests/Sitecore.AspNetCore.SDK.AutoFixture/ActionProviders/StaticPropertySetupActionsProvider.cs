using System.Reflection;

namespace Sitecore.AspNetCore.SDK.AutoFixture.ActionProviders;

public class StaticPropertySetupActionsProvider : SetupActionsProvider
{
    protected override object? ResolveFromType(Type? type, string fixtureAction)
    {
        PropertyInfo? property = type?.GetProperty(
            fixtureAction,
            BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);

        return property?.GetValue(null);
    }
}