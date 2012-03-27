using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Settings;
using Piedone.ThemeOverride.Models;
using Orchard.ContentManagement;

namespace Piedone.ThemeOverride.Services
{
    public class ThemeOverrideService : IThemeOverrideService
    {
        private readonly ISiteService _siteService;

        public ThemeOverrideService(
            ISiteService siteService)
        {
            _siteService = siteService;
        }

        public void SaveStyle(string css)
        {
            var settings = _siteService.GetSiteSettings().As<ThemeOverrideSettingsPart>();
            settings.Style = css;
        }

        public string GetStyle()
        {
            return _siteService.GetSiteSettings().As<ThemeOverrideSettingsPart>().Style;
        }
    }
}
