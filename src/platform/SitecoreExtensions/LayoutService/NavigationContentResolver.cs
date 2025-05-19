using Sitecore.XA.JSS.Feature.Navigation.Models;
using System.Collections.Generic;
using Sitecore.Data.Items;

namespace Sitecore.Demo.Edge.Website.SitecoreExtensions.LayoutService
{
    public class NavigationContentResolver : Sitecore.XA.JSS.Feature.Navigation.ContentsResolvers.NavigationContentResolver
    {
        protected override NavigationItemModel CreateNavigationModel(Item item, int level, int index, int siblingCount, List<NavigationItemModel> children = null, int flatLevel = -1)
        {
            this.UrlOptions.Language = Sitecore.Context.Item.Language;
            this.UrlOptions.LanguageEmbedding = Links.LanguageEmbedding.Always;
            this.UrlOptions.AlwaysIncludeServerUrl = false;
            Sitecore.Diagnostics.Log.Info($"Url Always: {this.LinkProvider.GetItemUrl(item, this.UrlOptions)}", this);

            var model = base.CreateNavigationModel(item, level, index, siblingCount, children, flatLevel);

            this.UrlOptions.LanguageEmbedding = Links.LanguageEmbedding.AsNeeded;
            Sitecore.Diagnostics.Log.Info($"Url AsNeeded: {this.LinkProvider.GetItemUrl(item, this.UrlOptions)}", this);

            return model;
        }
    }
}