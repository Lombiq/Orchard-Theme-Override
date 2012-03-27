using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Orchard.Mvc.Filters;
using Orchard.UI.Resources;
using Orchard;
using Orchard.DisplayManagement;
using Piedone.ThemeOverride.Services;

namespace Piedone.ThemeOverride
{
    public class ThemeOverrideFilter : FilterProvider, IResultFilter
    {
        private readonly IThemeOverrideService _themeOverrideService;
        private readonly IResourceManager _resourceManager;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly dynamic _shapeFactory;

        public ThemeOverrideFilter(
            IThemeOverrideService themeOverrideService,
            IResourceManager resourceManager,
            IWorkContextAccessor workContextAccessor,
            IShapeFactory shapeFactory)
        {
            _themeOverrideService = themeOverrideService;
            _resourceManager = resourceManager;
            _workContextAccessor = workContextAccessor;
            _shapeFactory = shapeFactory;
        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            // Don't run on admin
            if (Orchard.UI.Admin.AdminFilter.IsApplied(filterContext.RequestContext)) return;

            string styleUrl;
            if (_themeOverrideService.TryGetStylePublicUrl(out styleUrl))
            {
                var resourceSettings = _resourceManager.Include("stylesheet", styleUrl, styleUrl);
                var resource = _resourceManager.FindResource(resourceSettings);
                _resourceManager.NotRequired("stylesheet", resource.Name);

                _workContextAccessor.GetContext(filterContext).Layout.Head.Add(_shapeFactory.Style(
                                    Url: resource.Url,
                                    Condition: resourceSettings.Condition,
                                    Resource: resource,
                                    TagAttributes: resourceSettings.Attributes), "999");
            }
        }

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
        }
    }
}
