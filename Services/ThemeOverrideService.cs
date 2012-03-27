using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Settings;
using Piedone.ThemeOverride.Models;
using Orchard.ContentManagement;
using Orchard.FileSystems.Media;
using System.IO;

namespace Piedone.ThemeOverride.Services
{
    /// <remarks>
    /// The oddities with file handling here are because there is currently no existence check in IStorageProvider: http://orchard.codeplex.com/workitem/18279
    /// </remarks>
    public class ThemeOverrideService : IThemeOverrideService
    {
        private readonly IStorageProvider _storageProvider;
        private const string _rootPath = "ThemeOverride/";
        private const string _stylePath = _rootPath + "Style.css";

        public ThemeOverrideService(
            IStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
        }

        public void SaveStyle(string css)
        {
            try
            {
                _storageProvider.DeleteFile(_stylePath);
            }
            catch (Exception)
            {
            }

            using (var stream = _storageProvider.CreateFile(_stylePath).OpenWrite())
            {
                var bytes = Encoding.UTF8.GetBytes(css);
                stream.Write(bytes, 0, bytes.Length);
            }
        }

        public string GetStyle()
        {
            try
            {
                var file = _storageProvider.GetFile(_stylePath);
                using (var stream = file.OpenRead())
                {
                    using (var streamReader = new StreamReader(stream))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        public bool TryGetStylePublicUrl(out string publicUrl)
        {
            try
            {
                _storageProvider.GetFile(_stylePath);
                publicUrl = _storageProvider.GetPublicUrl(_stylePath);
                return true;
            }
            catch (Exception)
            {
                publicUrl = "";
                return false;
            }
        }
    }
}
