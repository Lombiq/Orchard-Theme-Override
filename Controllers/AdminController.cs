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
            var overrides = _themeOverrideService.GetOverrides();
            return View(new EditorViewModel
            {
                StylesheetUrl = overrides.StylesheetUri != null ? overrides.StylesheetUri.ToString() : string.Empty,
                CustomStylesContent = overrides.CustomStyles.Content
            });
        }

        [HttpPost]
        public ActionResult Index(EditorViewModel viewModel)
        {
            Uri stylesheetUri = null;

            if (!string.IsNullOrEmpty(viewModel.StylesheetUrl))
            {
                if (!Uri.IsWellFormedUriString(viewModel.StylesheetUrl, UriKind.RelativeOrAbsolute))
                {
                    ModelState.AddModelError("StylesheetUrlMalformed", T("The given stylesheet URL is not a proper URL.").Text);
                    return View(viewModel);
                }

                var stylesheetUriKind = Uri.IsWellFormedUriString(viewModel.StylesheetUrl, UriKind.Absolute) ? UriKind.Absolute : UriKind.Relative;
                stylesheetUri = new Uri(viewModel.StylesheetUrl, stylesheetUriKind);
            }


            _themeOverrideService.SaveStyles(stylesheetUri, viewModel.CustomStylesContent);

            _notifier.Information(T("The settings have been saved."));

            return RedirectToAction("Index");
        }
    }
}
