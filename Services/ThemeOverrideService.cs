﻿using Orchard.Caching.Services;
using Orchard.ContentManagement;
using Orchard.DisplayManagement.Descriptors;
using Orchard.FileSystems.Media;
using Orchard.Services;
using Orchard.Settings;
using Orchard.UI.Resources;
using Piedone.ThemeOverride.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Orchard.Environment;

namespace Piedone.ThemeOverride.Services
{
    public class ThemeOverrideService : IThemeOverrideService, IShapeTableEventHandler
    {
        private readonly IStorageProvider _storageProvider;
        private readonly ISiteService _siteService;
        private readonly IJsonConverter _jsonConverter;
        private readonly IPlacementProcessor _placementProcessor;
        private readonly ICacheService _cacheService;
        private readonly Work<IThemeOverrideService> _themeOverrideServiceWork; // See comment below.
        private readonly IClock _clock;

        private const string RootPath = "_PiedoneModules/ThemeOverride/";
        private const string CustomStylesPath = RootPath + "OverridingStyles.css";
        private const string CustomHeadScriptPath = RootPath + "OverridingHeadScript.js";
        private const string CustomFootScriptPath = RootPath + "OverridingFootScript.js";
        private const string CustomPlacementPath = RootPath + "Placement.info";

        private const string PlacementsCacheKey = "Piedone.ThemeOverride.Services.ThemeOverrideService.Placements";


        public ThemeOverrideService(
            IStorageProvider storageProvider,
            ISiteService siteService,
            IJsonConverter jsonConverter,
            IPlacementProcessor placementProcessor,
            ICacheService cacheService,
            Work<IThemeOverrideService> themeOverrideServiceWork,
            IClock clock)
        {
            _storageProvider = storageProvider;
            _siteService = siteService;
            _jsonConverter = jsonConverter;
            _placementProcessor = placementProcessor;
            _cacheService = cacheService;
            _themeOverrideServiceWork = themeOverrideServiceWork;
            _clock = clock;
        }


        public void SaveFaviconUri(Uri uri)
        {
            GetPart().FaviconUrl = uri != null ? uri.ToString() : string.Empty;
        }

        public void SaveStyles(IEnumerable<Uri> stylesheetUris, string customStyles)
        {
            var part = GetPart();

            if (stylesheetUris == null) stylesheetUris = Enumerable.Empty<Uri>();

            part.StylesheetUrisJson = _jsonConverter.Serialize(stylesheetUris);

            if (_storageProvider.FileExists(CustomStylesPath)) _storageProvider.DeleteFile(CustomStylesPath);

            if (!string.IsNullOrEmpty(customStyles))
            {
                using (var stream = _storageProvider.CreateFile(CustomStylesPath).OpenWrite())
                {
                    var bytes = Encoding.UTF8.GetBytes(customStyles);
                    stream.Write(bytes, 0, bytes.Length);
                }

                part.CustomStylesIsSaved = true;
                part.CustomStylesModifieddUtc = _clock.UtcNow;
            }
            else
            {
                part.CustomStylesIsSaved = false;
            }
        }

        public void SaveScripts(IEnumerable<Uri> scriptUris, string customScript, ResourceLocation location)
        {
            var part = GetPart();

            if (scriptUris == null) scriptUris = Enumerable.Empty<Uri>();

            var urisJson = _jsonConverter.Serialize(scriptUris);

            string customScriptPath;
            if (location == ResourceLocation.Head)
            {
                customScriptPath = CustomHeadScriptPath;
                part.HeadScriptUrisJson = urisJson;
            }
            else
            {
                customScriptPath = CustomFootScriptPath;
                part.FootScriptUrisJson = urisJson;
            }

            if (_storageProvider.FileExists(customScriptPath)) _storageProvider.DeleteFile(customScriptPath);

            if (!string.IsNullOrEmpty(customScript))
            {
                using (var stream = _storageProvider.CreateFile(customScriptPath).OpenWrite())
                {
                    var bytes = Encoding.UTF8.GetBytes(customScript);
                    stream.Write(bytes, 0, bytes.Length);
                }

                if (location == ResourceLocation.Head)
                {
                    part.CustomHeadScriptIsSaved = true;
                    part.CustomHeadScriptModifiedUtc = _clock.UtcNow;
                }
                else
                {
                    part.CustomFootScriptIsSaved = true;
                    part.CustomFootScriptModifiedUtc = _clock.UtcNow;
                }
            }
            else
            {
                if (location == ResourceLocation.Head)
                {
                    part.CustomHeadScriptIsSaved = false;
                }
                else
                {
                    part.CustomFootScriptIsSaved = false;
                }
            }
        }

        public void SavePlacement(string customPlacement)
        {
            GetPart().CustomPlacementContent = customPlacement;
            _cacheService.Remove(PlacementsCacheKey);
        }

        public IOverrides GetOverrides()
        {
            var part = GetPart();

            var overrides = new Overrides();

            if (!string.IsNullOrEmpty(part.FaviconUrl)) overrides.FaviconUri = CreateUri(part.FaviconUrl);

            overrides.StylesheetUris = CreateUris(part.StylesheetUrisJson);
            overrides.CustomStyles =
                CreateCustomResource(part.CustomStylesIsSaved, CustomStylesPath, part.CustomStylesModifieddUtc);

            overrides.HeadScriptUris = CreateUris(part.HeadScriptUrisJson);
            overrides.CustomHeadScript =
                CreateCustomResource(part.CustomHeadScriptIsSaved, CustomHeadScriptPath, part.CustomHeadScriptModifiedUtc);

            overrides.FootScriptUris = CreateUris(part.FootScriptUrisJson);
            overrides.CustomFootScript =
                CreateCustomResource(part.CustomFootScriptIsSaved, CustomFootScriptPath, part.CustomFootScriptModifiedUtc);

            overrides.CustomPlacementContent = part.CustomPlacementContent;

            return overrides;
        }

        public void ShapeTableCreated(ShapeTable shapeTable)
        {
            foreach (var descriptor in shapeTable.Descriptors.Values)
            {
                var existingPlacement = descriptor.Placement;
                descriptor.Placement = ctx =>
                {
                    // This delegate will be executed at some later time, so we can't just use methods from this
                    // object directly, since the object will belong to some long disposed work context. So getting
                    // a fresh instance here. Just wrapping ISiteService into Work<> would also work at the moment but
                    // GetPlacements() will eventually fail if any of its dependencies try to access any non-trivial
                    // service from the old work context.
                    var placements = ((ThemeOverrideService)_themeOverrideServiceWork.Value).GetPlacements();

                    if (!placements.ContainsKey(descriptor.ShapeType)) return existingPlacement(ctx);

                    var declarations = placements[descriptor.ShapeType];
                    foreach (var declaration in declarations)
                    {
                        if (declaration.Predicate(ctx)) return declaration.Placement;
                    }

                    return existingPlacement(ctx);
                };
            }
        }


        private ThemeOverrideSettingsPart GetPart()
        {
            return _siteService.GetSiteSettings().As<ThemeOverrideSettingsPart>();
        }

        private IDictionary<string, IEnumerable<IPlacementDeclaration>> GetPlacements()
        {
            return _cacheService.Get(PlacementsCacheKey, () =>
                {
                    var customPlacement = GetPart().CustomPlacementContent;

                    if (string.IsNullOrEmpty(customPlacement)) return new Dictionary<string, IEnumerable<IPlacementDeclaration>>();

                    return _placementProcessor.Process(customPlacement);
                });
        }

        private CustomResource CreateCustomResource(bool isSaved, string path, DateTime modifiedUtc)
        {
            if (isSaved && _storageProvider.FileExists(path))
            {
                return new CustomResource
                {
                    UriFactory = new Lazy<Uri>(() =>
                    {
                        var uriBuilder = new UriBuilder(CreateUri(_storageProvider.GetPublicUrl(path)));
                        uriBuilder.Query = "timestamp=" + modifiedUtc.ToFileTimeUtc();
                        return uriBuilder.Uri;
                    }),
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

        private IEnumerable<Uri> CreateUris(string urlsJson)
        {
            if (string.IsNullOrEmpty(urlsJson)) return Enumerable.Empty<Uri>();
            return _jsonConverter.Deserialize<IEnumerable<Uri>>(urlsJson);
        }


        private static Uri CreateUri(string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute)) return new Uri(url, UriKind.Absolute);
            return new Uri(url, UriKind.Relative);
        }


        private class Overrides : IOverrides
        {
            public Uri FaviconUri { get; set; }
            public IEnumerable<Uri> StylesheetUris { get; set; }
            public ICustomResource CustomStyles { get; set; }
            public IEnumerable<Uri> HeadScriptUris { get; set; }
            public ICustomResource CustomHeadScript { get; set; }
            public IEnumerable<Uri> FootScriptUris { get; set; }
            public ICustomResource CustomFootScript { get; set; }
            public string CustomPlacementContent { get; set; }
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
