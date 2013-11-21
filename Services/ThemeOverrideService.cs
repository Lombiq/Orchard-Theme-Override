using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Settings;
using Orchard.ContentManagement;
using Orchard.FileSystems.Media;
using System.IO;
using Orchard.Exceptions;

namespace Piedone.ThemeOverride.Services
{
    public class ThemeOverrideService : IThemeOverrideService
    {
        private readonly IStorageProvider _storageProvider;

        private const string RootPath = "ThemeOverride/";
        private const string StylePath = RootPath + "OverridingStyles.css";


        public ThemeOverrideService(IStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
        }


        public void SaveStyle(string css)
        {
            if (_storageProvider.FileExists(StylePath)) _storageProvider.DeleteFile(StylePath);

            if (!String.IsNullOrEmpty(css))
            {
                using (var stream = _storageProvider.CreateFile(StylePath).OpenWrite())
                {
                    var bytes = Encoding.UTF8.GetBytes(css);
                    stream.Write(bytes, 0, bytes.Length);
                }
            }
        }

        public string GetStyle()
        {
            if (_storageProvider.FileExists(StylePath))
            {
                using (var stream = _storageProvider.GetFile(StylePath).OpenRead())
                {
                    using (var streamReader = new StreamReader(stream))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }

            return string.Empty;
        }

        public bool TryGetStylePublicUrl(out string publicUrl)
        {
            if (_storageProvider.FileExists(StylePath))
            {
                publicUrl = _storageProvider.GetPublicUrl(StylePath);
                return true;
            }

            publicUrl = string.Empty;
            return false;
        }
    }
}
