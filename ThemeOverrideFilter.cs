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


        public ThemeOverrideFilter(
            IThemeOverrideService themeOverrideService,
            IResourceManager resourceManager)
        {
            _themeOverrideService = themeOverrideService;
            _resourceManager = resourceManager;
        }


        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            // Don't run on admin
            if (Orchard.UI.Admin.AdminFilter.IsApplied(filterContext.RequestContext)) return;

            // Should only run on a full view rendering result
            if (!(filterContext.Result is ViewResult)) return;

            string styleUrl;
            if (_themeOverrideService.TryGetStylePublicUrl(out styleUrl))
            {
                _resourceManager.RegisterLink(new LinkEntry() { Href = styleUrl, Rel = "stylesheet", Type = "text/css" });
            }
        }

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
        }
    }
}
