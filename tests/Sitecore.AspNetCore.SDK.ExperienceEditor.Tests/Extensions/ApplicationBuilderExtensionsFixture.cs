using System.Reflection;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using NSubstitute;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.ExperienceEditor.Configuration;
using Sitecore.AspNetCore.SDK.ExperienceEditor.Extensions;
using Sitecore.AspNetCore.SDK.ExperienceEditor.Middleware;
using Xunit;

namespace Sitecore.AspNetCore.SDK.ExperienceEditor.Tests.Extensions;

public class ApplicationBuilderExtensionsFixture
{
    [Fact]
    public void UseSitecoreRenderingEngine_NullApplicationBuilder_Throws()
    {
        Func<IApplicationBuilder> act =
            () => ExperienceEditorAppConfigurationExtensions.UseSitecoreExperienceEditor(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [AutoNSubstituteData]
    public void UseExperienceEditor_WithAppBuilderAndWithoutExperienceEditorServices_DoesNotCallMiddleware(IApplicationBuilder appBuilder)
    {
        // Arrange
        appBuilder.ApplicationServices.GetService(typeof(ExperienceEditorMarkerService)).Returns(null);

        // Act
        appBuilder.UseSitecoreExperienceEditor();

        // Assert
        bool received = appBuilder.ReceivedCalls().Any(c => c.GetArguments().OfType<Delegate>().Any(d =>
            d.Target?.GetType().GetField("_middleware", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(d.Target).As<Type>().FullName == typeof(ExperienceEditorMiddleware).FullName));
        received.Should().BeFalse();
    }
}