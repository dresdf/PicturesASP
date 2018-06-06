using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PicturesASP.Utils
{
    public class ImageUtils
    {
        private static readonly string[] extensions = { ".jpg", ".jpeg", ".gif", ".png" };
        public static bool IsImageFile(string file)
        {
            string ext = Path.GetExtension(file);
            return extensions.Contains(ext, StringComparer.OrdinalIgnoreCase);
        }

    }
}
