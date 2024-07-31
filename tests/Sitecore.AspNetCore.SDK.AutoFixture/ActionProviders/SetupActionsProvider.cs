using AutoFixture;

namespace Sitecore.AspNetCore.SDK.AutoFixture.ActionProviders;

public abstract class SetupActionsProvider : ISetupActionsProvider
{
    public virtual IEnumerable<Action<IFixture>> GetSetupActions(Type? type, string fixtureAction)
    {
        object? setup = ResolveFromType(type, fixtureAction);

        return ResolveActions(setup);
    }

    protected static IEnumerable<Action<IFixture>> ResolveActions(object? actions)
    {
        if (actions == null)
        {
            return [];
        }

        if (actions is IEnumerable<Action<IFixture>> enumerable)
        {
            return enumerable;
        }

        if (actions is not Action<IFixture> single)
        {
            throw new ArgumentNullException(nameof(actions), "No setup action provided");
        }

        enumerable = [single];

        return enumerable;
    }

    protected abstract object? ResolveFromType(Type? type, string fixtureAction);
}