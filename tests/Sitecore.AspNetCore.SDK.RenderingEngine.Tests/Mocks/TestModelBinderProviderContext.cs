using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Mocks;

[ExcludeFromCodeCoverage]
public class TestModelBinderProviderContext(ModelMetadata modelMetadata)
    : ModelBinderProviderContext
{
    public TestModelBinderProviderContext(ModelMetadata modelMetadata, BindingInfo bindingInfo)
        : this(modelMetadata)
    {
        BindingInfo = bindingInfo;
    }

    public override BindingInfo BindingInfo { get; } = new();

    public override ModelMetadata Metadata { get; } = modelMetadata;

    public override IModelMetadataProvider MetadataProvider { get; } = new EmptyModelMetadataProvider();

    public override IModelBinder CreateBinder(ModelMetadata metadata)
    {
        throw new NotImplementedException();
    }
}