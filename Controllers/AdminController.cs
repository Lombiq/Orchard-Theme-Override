using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Piedone.ThemeOverride.Services;
using Piedone.ThemeOverride.ViewModels;
using Orchard.UI.Notify;
using Orchard.Localization;

namespace Piedone.ThemeOverride.Controllers
{
    public class AdminController : Controller
    {
        private readonly IThemeOverrideService _themeOverrideService;
        private readonly INotifier _notifier;

        public Localizer T { get; set; }


        public AdminController(
            IThemeOverrideService themeOverrideService,
            INotifier notifier)
        {
            _themeOverrideService = themeOverrideService;
            _notifier = notifier;

            T = NullLocalizer.Instance;
        }


        public ActionResult Index()
        {
            return View(new EditorViewModel { Style = _themeOverrideService.GetStyle() });
        }

        [HttpPost]
        public ActionResult Index(EditorViewModel viewModel)
        {
            _themeOverrideService.SaveStyle(viewModel.Style);

            _notifier.Information(T("The settings have been saved."));

            return View(viewModel);
        }
    }
}
