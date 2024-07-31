using Sitecore.AspNetCore.SDK.RenderingEngine.Binding;
using Sitecore.AspNetCore.SDK.RenderingEngine.ViewComponents;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Mocks;

public class BindingViewComponentMock(IViewModelBinder binder)
    : BindingViewComponent(binder)
{
    public IViewModelBinder BinderAccessor => Binder;
}