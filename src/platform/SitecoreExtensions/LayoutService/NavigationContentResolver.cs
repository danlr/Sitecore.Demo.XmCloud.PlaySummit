using Sitecore.XA.JSS.Feature.Navigation.Models;
using System.Collections.Generic;
using Sitecore.Data.Items;

namespace Sitecore.Demo.Edge.Website.SitecoreExtensions.LayoutService
{
    public class NavigationContentResolver : Sitecore.XA.JSS.Feature.Navigation.ContentsResolvers.NavigationContentResolver
    {
        protected virtual NavigationItemModel CreateNavigationModel(Item item, int level, int index, int siblingCount, List<NavigationItemModel> children = null, int flatLevel = -1)
        {
            this.UrlOptions.Language = Sitecore.Context.Item.Language;
            
            return base.CreateNavigationModel(item, level, index, siblingCount, children, flatLevel); 
        }
    }
}