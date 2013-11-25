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

namespace Piedone.ThemeOverride.Services
{
    public class ThemeOverrideService : IThemeOverrideService
    {
        private readonly IStorageProvider _storageProvider;
        private readonly ISiteService _siteService;

        private const string RootPath = "ThemeOverride/";
        private const string CustomStylesPath = RootPath + "OverridingStyles.css";


        public ThemeOverrideService(IStorageProvider storageProvider, ISiteService siteService)
        {
            _storageProvider = storageProvider;
            _siteService = siteService;
        }


        public void SaveStyles(Uri stylesheetUri, string customStyles)
        {
            var part = GetPart();

            if (stylesheetUri != null) part.StylesheetUrl = stylesheetUri.ToString();

            if (_storageProvider.FileExists(CustomStylesPath)) _storageProvider.DeleteFile(CustomStylesPath);

            if (!String.IsNullOrEmpty(customStyles))
            {
                using (var stream = _storageProvider.CreateFile(CustomStylesPath).OpenWrite())
                {
                    var bytes = Encoding.UTF8.GetBytes(customStyles);
                    stream.Write(bytes, 0, bytes.Length);
                }

                part.CustomStylesSaved = true;
            }
        }

        public IOverrides GetOverrides()
        {
            var part = GetPart();

            var overrides = new Overrides();

            if (!string.IsNullOrEmpty(part.StylesheetUrl) && Uri.IsWellFormedUriString(part.StylesheetUrl, UriKind.RelativeOrAbsolute))
            {
                overrides.StylesheetUri = new Uri(part.StylesheetUrl, Uri.IsWellFormedUriString(part.StylesheetUrl, UriKind.Absolute) ? UriKind.Absolute : UriKind.Relative);
            }

            if (part.CustomStylesSaved && _storageProvider.FileExists(CustomStylesPath))
            {
                overrides.CustomStyles = new CustomStyles
                {
                    UriFactory = new Lazy<Uri>(() => new Uri(_storageProvider.GetPublicUrl(CustomStylesPath), UriKind.Relative)),
                    ContentFactory = new Lazy<string>(() =>
                        {
                            using (var stream = _storageProvider.GetFile(CustomStylesPath).OpenRead())
                            {
                                using (var streamReader = new StreamReader(stream))
                                {
                                    return streamReader.ReadToEnd();
                                }
                            }
                        })
                };
            }
            else overrides.CustomStyles = new CustomStyles();

            return overrides;
        }


        private ThemeOverrideSettingsPart GetPart()
        {
            return _siteService.GetSiteSettings().As<ThemeOverrideSettingsPart>();
        }


        private class Overrides : IOverrides
        {
            public Uri StylesheetUri { get; set; }
            public ICustomStyles CustomStyles { get; set; }
        }

        private class CustomStyles : ICustomStyles
        {
            public Uri Uri { get { return UriFactory != null ? UriFactory.Value : null; } }
            public Lazy<Uri> UriFactory { get; set; }

            public string Content { get { return ContentFactory != null ? ContentFactory.Value : string.Empty; } }
            public Lazy<string> ContentFactory { get; set; }
        }
    }
}
