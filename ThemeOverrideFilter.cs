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
        private readonly IWorkContextAccessor _wca;
        private readonly dynamic _shapeFactory;


        public ThemeOverrideFilter(
            IThemeOverrideService themeOverrideService,
            IWorkContextAccessor wca,
            IShapeFactory shapeFactory)
        {
            _themeOverrideService = themeOverrideService;
            _wca = wca;
            _shapeFactory = shapeFactory;
        }


        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            // Don't run on admin
            if (Orchard.UI.Admin.AdminFilter.IsApplied(filterContext.RequestContext)) return;

            // Should only run on a full view rendering result
            if (!(filterContext.Result is ViewResult)) return;

            _wca.GetContext().Layout.Head.Insert(_shapeFactory.ThemeOverride_OverridesInclusion(Overrides: _themeOverrideService.GetOverrides()));
        }

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
        }
    }
}
