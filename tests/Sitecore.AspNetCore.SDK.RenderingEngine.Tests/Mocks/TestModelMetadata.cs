using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Mocks;

[ExcludeFromCodeCoverage]
public class TestModelMetadata(ModelMetadataIdentity identity, Type type)
    : ModelMetadata(identity)
{
    public TestModelMetadata(Type type)
        : this(ModelMetadataIdentity.ForType(type), type)
    {
    }

    public new Type UnderlyingOrModelType { get; } = type;

    public override IReadOnlyDictionary<object, object> AdditionalValues => null!;

    public override string BinderModelName => string.Empty;

    public override Type BinderType => null!;

    public override BindingSource BindingSource => null!;

    public override bool ConvertEmptyStringToNull => false;

    public override string DataTypeName => string.Empty;

    public override string Description => string.Empty;

    public override string DisplayFormatString => string.Empty;

    public override string DisplayName => string.Empty;

    public override string EditFormatString => string.Empty;

    public override ModelMetadata ElementMetadata => null!;

    public override IEnumerable<KeyValuePair<EnumGroupAndName, string>> EnumGroupedDisplayNamesAndValues => null!;

    public override IReadOnlyDictionary<string, string> EnumNamesAndValues => null!;

    public override bool HasNonDefaultEditFormat => false;

    public override bool HideSurroundingHtml => false;

    public override bool HtmlEncode => false;

    public override bool IsBindingAllowed => false;

    public override bool IsBindingRequired => false;

    public override bool IsEnum => false;

    public override bool IsFlagsEnum => false;

    public override bool IsReadOnly => false;

    public override bool IsRequired => false;

    public override ModelBindingMessageProvider ModelBindingMessageProvider => null!;

    public override string NullDisplayText => string.Empty;

    public override int Order => 0;

    public override string Placeholder => string.Empty;

    public override ModelPropertyCollection Properties => null!;

    public override IPropertyFilterProvider PropertyFilterProvider => null!;

    public override Func<object, object> PropertyGetter => null!;

    public override Action<object, object?>? PropertySetter => null;

    public override bool ShowForDisplay => false;

    public override bool ShowForEdit => false;

    public override string SimpleDisplayProperty => string.Empty;

    public override string TemplateHint => string.Empty;

    public override bool ValidateChildren => false;

    public override IReadOnlyList<object> ValidatorMetadata => [];
}