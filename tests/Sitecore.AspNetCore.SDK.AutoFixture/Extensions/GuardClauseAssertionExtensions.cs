using System.Reflection;
using AutoFixture.Idioms;

namespace Sitecore.AspNetCore.SDK.AutoFixture.Extensions;

public static class GuardClauseAssertionExtensions
{
    public static void Verify<T>(this GuardClauseAssertion assertion)
    {
        ArgumentNullException.ThrowIfNull(assertion);
        assertion.Verify(typeof(T));
    }

    public static void VerifyConstructors<T>(this GuardClauseAssertion assertion)
    {
        ArgumentNullException.ThrowIfNull(assertion);
        assertion.Verify(typeof(T).GetTypeInfo().DeclaredConstructors);
    }

    public static void VerifyMethod<T>(this GuardClauseAssertion assertion, string methodName)
    {
        ArgumentNullException.ThrowIfNull(assertion);
        if (string.IsNullOrWhiteSpace(methodName))
        {
            throw new ArgumentNullException(nameof(methodName));
        }

        assertion.Verify(typeof(T).GetTypeInfo().GetDeclaredMethods(methodName));
    }
}