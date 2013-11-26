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
    [ValidateInput(false)]
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
                StylesheetUrls = string.Join(Environment.NewLine, overrides.StylesheetUris),
                CustomStylesContent = overrides.CustomStyles.Content,
                HeadScriptUrls = string.Join(Environment.NewLine, overrides.HeadScriptUris),
                CustomHeadScriptContent = overrides.CustomHeadScript.Content,
                FootScriptUrls = string.Join(Environment.NewLine, overrides.FootScriptUris),
                CustomFootScriptContent = overrides.CustomFootScript.Content,
                CustomPlacementContent = overrides.CustomPlacementContent
            });
        }

        [HttpPost]
        public ActionResult Index(EditorViewModel viewModel)
        {
            var stylesheetUris = new List<Uri>();
            if (!string.IsNullOrEmpty(viewModel.StylesheetUrls))
            {
                foreach (var url in viewModel.StylesheetUrls.Split(Environment.NewLine.ToArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    Uri stylesheetUri;
                    if (!TryCreateUri(url, out stylesheetUri))
                    {
                        ModelState.AddModelError("StylesheetUrlMalformed", T("The stylesheet URL {0} is not a proper URL.", url).Text);
                    }
                    else stylesheetUris.Add(stylesheetUri);
                } 
            }

            var headScriptUris = new List<Uri>();
            if (!string.IsNullOrEmpty(viewModel.HeadScriptUrls))
            {
                foreach (var url in viewModel.HeadScriptUrls.Split(Environment.NewLine.ToArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    Uri headScriptUri;
                    if (!TryCreateUri(url, out headScriptUri))
                    {
                        ModelState.AddModelError("HeadScriptUrlMalformed", T("The head script URL {0} is not a proper URL.", url).Text);
                    }
                    else headScriptUris.Add(headScriptUri);
                }
            }

            var footScriptUris = new List<Uri>();
            if (!string.IsNullOrEmpty(viewModel.FootScriptUrls))
            {
                foreach (var url in viewModel.FootScriptUrls.Split(Environment.NewLine.ToArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    Uri footScriptUri;
                    if (!TryCreateUri(url, out footScriptUri))
                    {
                        ModelState.AddModelError("FootScriptUrlMalformed", T("The foot script URL {0} is not a proper URL.", url).Text);
                    }
                    else footScriptUris.Add(footScriptUri);
                }
            }


            if (!ModelState.IsValid) return View(viewModel);


            _themeOverrideService.SaveStyles(stylesheetUris, viewModel.CustomStylesContent);
            _themeOverrideService.SaveScripts(headScriptUris, viewModel.CustomHeadScriptContent, ResourceLocation.Head);
            _themeOverrideService.SaveScripts(footScriptUris, viewModel.CustomFootScriptContent, ResourceLocation.Foot);
            _themeOverrideService.SavePlacement(viewModel.CustomPlacementContent);

            _notifier.Information(T("The settings have been saved."));

            return RedirectToAction("Index");
        }


        private static bool TryCreateUri(string url, out Uri uri)
        {
            uri = null;

            if (!string.IsNullOrEmpty(url))
            {
                if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute)) return false;

                var stylesheetUriKind = Uri.IsWellFormedUriString(url, UriKind.Absolute) ? UriKind.Absolute : UriKind.Relative;
                uri = new Uri(url, stylesheetUriKind);

                return true;
            }

            return false;
        }
    }
}
