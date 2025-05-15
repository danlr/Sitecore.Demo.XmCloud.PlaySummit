using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using DocumentFormat.OpenXml.Vml.Office;
using Newtonsoft.Json;
using Sitecore.Abstractions;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.LayoutService.Configuration;
using Sitecore.LayoutService.ItemRendering.ContentsResolvers;
using Sitecore.LayoutService.Serialization.Pipelines.GetFieldSerializer;
using Sitecore.Links;
using Sitecore.Links.UrlBuilders;
using Sitecore.Mvc.Presentation;

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

            ImageField logoField = (ImageField)configItem.Fields["HeaderLogo"];
            var imageUrl = this.mediaManager.GetMediaUrl(logoField.MediaItem);
            var altText = logoField.Alt;

            dynamic links = new List<ExpandoObject>();
            var pages = rootItem.Children.InnerChildren.Where(x => x["ShowInMainNavigation"] == "1");
            foreach (var page in pages)
            {
                dynamic link = new
                {
                    displayName = !string.IsNullOrWhiteSpace(page["NavigationTitle"]) ? page["NavigationTitle"] : page.DisplayName,
                    field = new
                    {
                        jsonValue = new
                        {
                            value = new
                            {
                                href = this.linkManager.GetItemUrl(page, new ItemUrlBuilderOptions { AlwaysIncludeServerUrl = false }),
                                id = page.ID,
                                linktype = "internal"
                            }
                        }
                    }
                };

                links.Add(link);
            }

            dynamic item = new
            {
                id = configItem.ID.ToString(),
                path = configItem.Paths.FullPath,
                headerLogo = new
                {
                    jsonValue = new
                    {
                        value = new
                        {
                            src = imageUrl,
                            alt = altText
                        }
                    },
                    alt = altText
                },
                links = new
                {
                    displayName = "Main navigation",
                    children = new
                    {
                        results = links
                    }
        }
            };

            var jsonLog = JsonConvert.SerializeObject(item, Formatting.Indented);
            Log.Info(jsonLog, this);

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
