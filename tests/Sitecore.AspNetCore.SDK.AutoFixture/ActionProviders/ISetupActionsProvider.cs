using AutoFixture;

namespace Sitecore.AspNetCore.SDK.AutoFixture.ActionProviders;

public interface ISetupActionsProvider
{
    IEnumerable<Action<IFixture>> GetSetupActions(Type? type, string fixtureAction);
}