using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Mocks;

public interface ICustomViewComponentHelper : IViewComponentHelper, IViewContextAware;