using AwesomeAssertions;
using Sitecore.AspNetCore.SDK.Pages.Configuration;
using Sitecore.AspNetCore.SDK.Pages.Extensions;
using Xunit;

namespace Sitecore.AspNetCore.SDK.Pages.Tests.Extensions
{
    public class PagesAppConfigurationExtensionsFixture
    {
        [Fact]
        public void UseSitecorePages_AppIsNull_ExceptionIsThrown()
        {
            // Act
            Action action = () => PagesAppConfigurationExtensions.UseSitecorePages(null!, new PagesOptions());

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }
    }
}