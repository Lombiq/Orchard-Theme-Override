using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.UI.Navigation;
using Orchard.Localization;

namespace Piedone.ThemeOverride
{
    public class AdminMenu : INavigationProvider
    {
        public Localizer T { get; set; }
        public string MenuName { get { return "admin"; } }


        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(T("Themes"),
                menu => menu.Add(T("Theme Override Settings"), "4", item => item.Action("Index", "Admin", new { area = "Piedone.ThemeOverride" }).LocalNav())
            );
        }
    }
}
