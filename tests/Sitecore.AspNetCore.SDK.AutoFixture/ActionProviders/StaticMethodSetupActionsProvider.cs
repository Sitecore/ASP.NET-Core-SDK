using System.Reflection;

namespace Sitecore.AspNetCore.SDK.AutoFixture.ActionProviders;

public class StaticMethodSetupActionsProvider : SetupActionsProvider
{
    protected override object? ResolveFromType(Type? type, string fixtureAction)
    {
        MethodInfo? method = type?.GetMethod(
            fixtureAction,
            BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);

        return method?.Invoke(null, []);
    }
}