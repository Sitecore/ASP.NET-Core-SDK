using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Sources;
using Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Mocks;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Binding;

public class SitecoreLayoutModelBinderFixture
{
    // ReSharper disable once UnusedMember.Global - Used by testing framework
    public static Action<IFixture> AutoSetup => f =>
    {
        ActionContext actionContext = new(
            new DefaultHttpContext(),
            new Microsoft.AspNetCore.Routing.RouteData(),
            new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());

        f.Inject(actionContext);
        IServiceProvider? sp = f.Freeze<IServiceProvider>();
        IServiceScopeFactory? scopeFactory = f.Freeze<IServiceScopeFactory>();
        sp.GetService(typeof(IServiceScopeFactory)).Returns(scopeFactory);
    };

    [Theory]
    [AutoNSubstituteData]
    public async Task BindModelAsync_WithNullModelBindingContext_Throws(SitecoreLayoutModelBinder<SitecoreLayoutBindingSource> sut)
    {
        Func<Task> act =
            () => sut.BindModelAsync(null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Theory]
    [AutoNSubstituteData]
    public void BindModelAsync_WithNullModelReturnedByBindingSource_ReturnsNoModel(
        SitecoreLayoutModelBinder<TestSitecoreLayoutBindingSource> sut,
        DefaultModelBindingContext defaultModelBindingContext)
    {
        // Act
        sut.BindModelAsync(defaultModelBindingContext);

        // Assert
        defaultModelBindingContext.Result.IsModelSet.Should().BeFalse();
    }

    [Theory]
    [AutoNSubstituteData]
    public void BindModelAsync_WithNullModelReturnedByBindingSource_WritesDebugLogs(
        DefaultModelBindingContext defaultModelBindingContext,
        IServiceProvider sp)
    {
        // Arrange
        ILogger<SitecoreLayoutModelBinder<TestSitecoreLayoutBindingSource>>? substituteLogger = Substitute.For<ILogger<SitecoreLayoutModelBinder<TestSitecoreLayoutBindingSource>>>();
        SitecoreLayoutModelBinder<TestSitecoreLayoutBindingSource> modelBinder = new(sp, substituteLogger);

        // Act
        modelBinder.BindModelAsync(defaultModelBindingContext);

        // Assert
        substituteLogger.Received(1).Log<IReadOnlyList<KeyValuePair<string, object?>>>(LogLevel.Debug, 0, Arg.Any<IReadOnlyList<KeyValuePair<string, object?>>>(), null, Arg.Any<Func<object, Exception?, string>>());
    }
}