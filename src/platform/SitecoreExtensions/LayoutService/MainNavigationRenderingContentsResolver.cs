using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Presentation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sitecore.Abstractions;
using Sitecore.Common;
using Sitecore.Configuration;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.LayoutService.Configuration;
using Sitecore.LayoutService.Helpers;
using Sitecore.LayoutService.ItemRendering.ContentsResolvers;
using Sitecore.LayoutService.Serialization.FieldSerializers;
using Sitecore.LayoutService.Serialization.Pipelines.GetFieldSerializer;
using Sitecore.Links;
using Sitecore.Links.UrlBuilders;
using Sitecore.Mvc.Presentation;
using Sitecore.Pipelines.HasPresentation;
using Sitecore.Resources.Media;
using Sitecore.Services.Infrastructure.Sitecore.Data;

namespace Sitecore.Demo.Edge.Website.SitecoreExtensions.LayoutService
{
    public class MainNavigationRenderingContentsResolver : RenderingContentsResolver
    {
        protected readonly IGetFieldSerializerPipeline getFieldSerializerPipeline;
        protected readonly BaseLinkManager linkManager;
        protected readonly BaseMediaManager mediaManager;
        protected readonly BaseLog log;

        public MainNavigationRenderingContentsResolver(IGetFieldSerializerPipeline getFieldSerializerPipeline, BaseLinkManager linkManager, BaseLog log, BaseMediaManager mediaManager)
        {
            this.getFieldSerializerPipeline = getFieldSerializerPipeline;
            this.linkManager = linkManager;
            this.mediaManager = mediaManager;
            this.log = log;
        }

        public override object ResolveContents(Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            var currentItem = Context.Item;

            var configItem = GetConfigItem(currentItem);
            var rootItem = GetRootItem(currentItem);

            Assert.IsNotNull(configItem, nameof(configItem));
            Assert.IsNotNull(rootItem, nameof(rootItem));

            dynamic item = new ExpandoObject();
            item.id = configItem.ID.ToString();
            item.path = configItem.Paths.FullPath;

            ImageField logoField = (ImageField)configItem.Fields["HeaderLogo"];
            if (logoField?.MediaItem != null)
            {
                var imageUrl = this.mediaManager.GetMediaUrl(logoField.MediaItem);
                var altText = logoField.Alt;

                item.headerLogo = new ExpandoObject();
                item.headerLogo.jsonValue = new ExpandoObject();
                item.headerLogo.jsonValue.value = new ExpandoObject();
                item.headerLogo.jsonValue.value.src = imageUrl;
                item.headerLogo.jsonValue.value.alt = altText;
                item.headerLogo.alt = altText;
            }

            item.links = new ExpandoObject();
            item.links.displayName = "Main navigation";
            item.links.children = new ExpandoObject();

            dynamic links = new List<ExpandoObject>();
            var pages = rootItem.Children.InnerChildren.Where(x => x["ShowInMainNavigation"] == "1");
            foreach (var page in pages)
            {
                dynamic link = new ExpandoObject();

                link.displayName = !string.IsNullOrWhiteSpace(page["NavigationTitle"]) ? page["NavigationTitle"] : page.DisplayName;
                link.field = new ExpandoObject();
                link.field.jsonValue = new ExpandoObject();
                link.field.jsonValue.value = new ExpandoObject();
                link.field.jsonValue.value.href = this.linkManager.GetItemUrl(item, new ItemUrlBuilderOptions { AlwaysIncludeServerUrl = false });
                link.field.jsonValue.value.id = page.ID;
                link.field.jsonValue.value.linktype = "internal";

                links.Add(link);
            }

            item.links.children.results = links;

            return item;
        }

        private Item GetConfigItem(Item currentItem)
        {
            var configId = Parameters["Config"];
            if (configId != null)
            {
                return currentItem.Database.GetItem(configId);
            }

            return null;
        }

        private Item GetRootItem(Item currentItem)
        {
            var rootId = Parameters["Root"];
            if (rootId != null)
            {
                return currentItem.Database.GetItem(rootId);
            }

            return null;
        }
    }
}
