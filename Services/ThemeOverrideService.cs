using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Settings;
using Orchard.ContentManagement;
using Orchard.FileSystems.Media;
using System.IO;
using Orchard.Exceptions;
using Piedone.ThemeOverride.Models;
using Orchard.UI.Resources;

namespace Piedone.ThemeOverride.Services
{
    public class ThemeOverrideService : IThemeOverrideService
    {
        private readonly IStorageProvider _storageProvider;
        private readonly ISiteService _siteService;

        private const string RootPath = "ThemeOverride/";
        private const string CustomStylesPath = RootPath + "OverridingStyles.css";
        private const string CustomHeadScriptPath = RootPath + "OverridingHeadScript.js";
        private const string CustomFootScriptPath = RootPath + "OverridingFootScript.js";


        public ThemeOverrideService(IStorageProvider storageProvider, ISiteService siteService)
        {
            _storageProvider = storageProvider;
            _siteService = siteService;
        }


        public void SaveStyles(Uri stylesheetUri, string customStyles)
        {
            var part = GetPart();

            if (stylesheetUri != null) part.StylesheetUrl = stylesheetUri.ToString();
            else part.StylesheetUrl = string.Empty;

            if (_storageProvider.FileExists(CustomStylesPath)) _storageProvider.DeleteFile(CustomStylesPath);

            if (!String.IsNullOrEmpty(customStyles))
            {
                using (var stream = _storageProvider.CreateFile(CustomStylesPath).OpenWrite())
                {
                    var bytes = Encoding.UTF8.GetBytes(customStyles);
                    stream.Write(bytes, 0, bytes.Length);
                }

                part.CustomStylesIsSaved = true;
            }
        }

        public void SaveScripts(Uri scriptUri, string customScript, ResourceLocation location)
        {
            var part = GetPart();

            string customScriptPath;
            if (location == ResourceLocation.Head)
            {
                customScriptPath = CustomHeadScriptPath;
                if (scriptUri != null) part.HeadScriptUrl = scriptUri.ToString();
                else part.HeadScriptUrl = string.Empty;
            }
            else
            {
                customScriptPath = CustomFootScriptPath;
                if (scriptUri != null) part.FootScriptUrl = scriptUri.ToString();
                else part.FootScriptUrl = string.Empty;
            }

            if (_storageProvider.FileExists(customScriptPath)) _storageProvider.DeleteFile(customScriptPath);

            if (!String.IsNullOrEmpty(customScript))
            {
                using (var stream = _storageProvider.CreateFile(customScriptPath).OpenWrite())
                {
                    var bytes = Encoding.UTF8.GetBytes(customScript);
                    stream.Write(bytes, 0, bytes.Length);
                }

                if (location == ResourceLocation.Head) part.CustomHeadScriptIsSaved = true;
                else part.CustomFootScriptIsSaved = true;
            }
        }

        public IOverrides GetOverrides()
        {
            var part = GetPart();

            var overrides = new Overrides();

            overrides.StylesheetUri = CreateUri(part.StylesheetUrl);
            overrides.CustomStyles = CreateCustomResource(part.CustomStylesIsSaved, CustomStylesPath);

            overrides.HeadScriptUri = CreateUri(part.HeadScriptUrl);
            overrides.CustomHeadScript = CreateCustomResource(part.CustomHeadScriptIsSaved, CustomHeadScriptPath);

            overrides.FootScriptUri = CreateUri(part.FootScriptUrl);
            overrides.CustomFootScript = CreateCustomResource(part.CustomFootScriptIsSaved, CustomFootScriptPath);

            return overrides;
        }


        private ThemeOverrideSettingsPart GetPart()
        {
            return _siteService.GetSiteSettings().As<ThemeOverrideSettingsPart>();
        }

        private CustomResource CreateCustomResource(bool isSaved, string path)
        {
            if (isSaved && _storageProvider.FileExists(path))
            {
                return new CustomResource
                {
                    UriFactory = new Lazy<Uri>(() => new Uri(_storageProvider.GetPublicUrl(path), UriKind.Relative)),
                    ContentFactory = new Lazy<string>(() =>
                    {
                        using (var stream = _storageProvider.GetFile(path).OpenRead())
                        {
                            using (var streamReader = new StreamReader(stream))
                            {
                                return streamReader.ReadToEnd();
                            }
                        }
                    })
                };
            }
            else return new CustomResource();
        }


        private static Uri CreateUri(string url)
        {
            if (!string.IsNullOrEmpty(url) && Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
            {
                return new Uri(url, Uri.IsWellFormedUriString(url, UriKind.Absolute) ? UriKind.Absolute : UriKind.Relative);
            }

            return null;
        }


        private class Overrides : IOverrides
        {
            public Uri StylesheetUri { get; set; }
            public ICustomResource CustomStyles { get; set; }
            public Uri HeadScriptUri { get; set; }
            public ICustomResource CustomHeadScript { get; set; }
            public Uri FootScriptUri { get; set; }
            public ICustomResource CustomFootScript { get; set; }
        }

        private class CustomResource : ICustomResource
        {
            public Uri Uri { get { return UriFactory != null ? UriFactory.Value : null; } }
            public Lazy<Uri> UriFactory { get; set; }
            public string Content { get { return ContentFactory != null ? ContentFactory.Value : string.Empty; } }
            public Lazy<string> ContentFactory { get; set; }
        }
    }
}
