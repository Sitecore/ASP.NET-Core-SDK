using Microsoft.AspNetCore.Mvc;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding;
using Sitecore.AspNetCore.SDK.RenderingEngine.ViewComponents;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.ViewComponents;

public class Component1ViewComponent(IViewModelBinder binder)
    : BindingViewComponent(binder)
{
    public Task<IViewComponentResult> InvokeAsync()
    {
        return Task.FromResult<IViewComponentResult>(View());
    }
}