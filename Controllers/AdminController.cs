using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Piedone.ThemeOverride.Services;
using Piedone.ThemeOverride.ViewModels;
using Orchard.UI.Notify;
using Orchard.Localization;
using Orchard.UI.Resources;

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
                CustomStylesContent = overrides.CustomStyles.Content,
                HeadScriptUrl = overrides.HeadScriptUri != null ? overrides.HeadScriptUri.ToString() : string.Empty,
                CustomHeadScriptContent = overrides.CustomHeadScript.Content,
                FootScriptUrl = overrides.FootScriptUri != null ? overrides.FootScriptUri.ToString() : string.Empty,
                CustomFootScriptContent = overrides.CustomFootScript.Content,
            });
        }

        [HttpPost]
        public ActionResult Index(EditorViewModel viewModel)
        {
            Uri stylesheetUri;
            if (!TryCreateUri(viewModel.StylesheetUrl, out stylesheetUri))
            {
                ModelState.AddModelError("StylesheetUrlMalformed", T("The given stylesheet URL is not a proper URL.").Text);
            }

            Uri headScriptUri;
            if (!TryCreateUri(viewModel.HeadScriptUrl, out headScriptUri))
            {
                ModelState.AddModelError("HeadScriptUrlMalformed", T("The given head script URL is not a proper URL.").Text);
            }

            Uri footScriptUri;
            if (!TryCreateUri(viewModel.FootScriptUrl, out footScriptUri))
            {
                ModelState.AddModelError("FootScriptUrlMalformed", T("The given foot script URL is not a proper URL.").Text);
            }


            if (!ModelState.IsValid) return View(viewModel);


            _themeOverrideService.SaveStyles(stylesheetUri, viewModel.CustomStylesContent);
            _themeOverrideService.SaveScripts(headScriptUri, viewModel.CustomHeadScriptContent, ResourceLocation.Head);
            _themeOverrideService.SaveScripts(footScriptUri, viewModel.CustomFootScriptContent, ResourceLocation.Foot);

            _notifier.Information(T("The settings have been saved."));

            return RedirectToAction("Index");
        }


        private static bool TryCreateUri(string url, out Uri uri)
        {
            uri = null;

            if (!string.IsNullOrEmpty(url))
            {
                if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
                {
                    return false;
                }

                var stylesheetUriKind = Uri.IsWellFormedUriString(url, UriKind.Absolute) ? UriKind.Absolute : UriKind.Relative;
                uri = new Uri(url, stylesheetUriKind);
            }

            return true;
        }
    }
}
