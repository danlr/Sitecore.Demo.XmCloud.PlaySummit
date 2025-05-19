using Sitecore.LayoutService.Configuration;
using Sitecore.Mvc.Presentation;

namespace Sitecore.Demo.Edge.Website.SitecoreExtensions.LayoutService
{
    public class NavigationContentResolver : Sitecore.XA.JSS.Feature.Navigation.ContentsResolvers.NavigationContentResolver
    {
        public new object ResolveContents(Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            var result = base.ResolveContents(rendering, renderingConfig);

            this.UrlOptions.Language = Sitecore.Context.Item.Language;

            return result;
        }
    }
}