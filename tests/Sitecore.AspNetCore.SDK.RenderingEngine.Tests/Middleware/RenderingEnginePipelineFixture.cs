using System.Reflection;
using AwesomeAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using NSubstitute;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.RenderingEngine.Localization;
using Sitecore.AspNetCore.SDK.RenderingEngine.Middleware;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Middleware;

public class RenderingEnginePipelineFixture
{
    [Theory]
    [AutoNSubstituteData]
    public void Configure_NullApp_Throws(RenderingEnginePipeline sut)
    {
        // Arrange
        Action action = () => sut.Configure(null!);

        // Act / Assert
        action.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("app");
    }

    [Theory]
    [AutoNSubstituteData]
    public void Configure_WithApp_RegistersMiddleware(RenderingEnginePipeline sut, IApplicationBuilder app)
    {
        // Arrange
        app.ApplicationServices.GetService(typeof(IOptions<RequestLocalizationOptions>))
            .Returns(Substitute.For<IOptions<RequestLocalizationOptions>>());
        app.ApplicationServices.GetService(typeof(SitecoreQueryStringCultureProvider))
            .Returns(new SitecoreQueryStringCultureProvider());
        IOptions<RequestLocalizationOptions>? options = Substitute.For<IOptions<RequestLocalizationOptions>>();
        app.ApplicationServices.GetService(typeof(IOptions<RequestLocalizationOptions>)).Returns(options);
        options.Value.Returns(new RequestLocalizationOptions());

        // Act
        sut.Configure(app);

        // Assert
        bool received = app.ReceivedCalls().Any(c => c.GetArguments().OfType<Delegate>().Any(d =>
            d.Target?.GetType().GetField("_middleware", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(d.Target).As<Type>().FullName == typeof(RenderingEngineMiddleware).FullName));
        received.Should().BeTrue();
    }
}