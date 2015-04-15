using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.DisplayManagement.Implementation;
using Orchard.Environment;
using Orchard.Mvc;
using Orchard.UI.Admin;
using Orchard.UI.Resources;
using Piedone.ThemeOverride.Services;

namespace Piedone.ThemeOverride.Services
{
    public class OverridesInjector : IShapeDisplayEvents
    {
        private readonly IHttpContextAccessor _hca;
        private readonly Work<IResourceManager> _resourceManagerWork;
        private readonly IThemeOverrideService _themeOverrideService;


        // Injecting IResourceManager is necessary because on shell start in the end IShapeDisplayEvents are instantiated, thus this class
        // as well. ResourceManager also implementing IUnitOfWorkDependency causes an Autofac DependencyResolutionException with the message
        // "No scope with a Tag matching 'work' is visible from the scope in which the instance was requested.".
        // See: https://github.com/OrchardCMS/Orchard/issues/4852
        public OverridesInjector(
            IHttpContextAccessor hca,
            Work<IResourceManager> resourceManagerWork,
            IThemeOverrideService themeOverrideService)
        {
            _hca = hca;
            _resourceManagerWork = resourceManagerWork;
            _themeOverrideService = themeOverrideService;
        }


        public void Displaying(ShapeDisplayingContext context)
        {
            if (context.ShapeMetadata.Type != "DocumentZone" || context.Shape.ZoneName != "Head") return;

            var httpContext = _hca.Current();
            if (httpContext == null) return;
            if (AdminFilter.IsApplied(httpContext.Request.RequestContext)) return;

            var resourceManager = _resourceManagerWork.Value;

            var overrides = _themeOverrideService.GetOverrides();
            if (overrides.FaviconUri != null)
            {
                resourceManager.RegisterLink(new LinkEntry { Type = "image/x-icon", Rel = "shortcut icon", Href = overrides.FaviconUri.ToString() });
            }

            if (overrides.StylesheetUris.Any())
            {
                foreach (var uri in overrides.StylesheetUris)
                {
                    var url = uri.ToString();
                    resourceManager.Include("stylesheet", url, url);
                }
            }

            if (overrides.CustomStyles.Uri != null)
            {
                var url = overrides.CustomStyles.Uri.ToString();
                resourceManager.Include("stylesheet", url, url);
            }

            if (overrides.HeadScriptUris.Any())
            {
                foreach (var uri in overrides.HeadScriptUris)
                {
                    var url = uri.ToString();
                    resourceManager.Include("script", url, url).AtHead();
                }
            }

            if (overrides.CustomHeadScript.Uri != null)
            {
                var url = overrides.CustomHeadScript.Uri.ToString();
                resourceManager.Include("script", url, url).AtHead();
            }


            if (overrides.FootScriptUris.Any())
            {
                foreach (var uri in overrides.FootScriptUris)
                {
                    var url = uri.ToString();
                    resourceManager.Include("script", url, url).AtFoot();
                }
            }

            if (overrides.CustomFootScript.Uri != null)
            {
                var url = overrides.CustomFootScript.Uri.ToString();
                resourceManager.Include("script", url, url).AtFoot();
            }
        }

        public void Displayed(ShapeDisplayedContext context)
        {
        }
    }
}
