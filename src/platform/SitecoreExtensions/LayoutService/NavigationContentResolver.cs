namespace Sitecore.Demo.Edge.Website.SitecoreExtensions.LayoutService
{
    public class NavigationContentResolver : Sitecore.XA.JSS.Feature.Navigation.ContentsResolvers.NavigationContentResolver
    {
        public NavigationContentResolver()
        {
            this.UrlOptions.Language = Sitecore.Context.Item.Language;
        }
    }
}