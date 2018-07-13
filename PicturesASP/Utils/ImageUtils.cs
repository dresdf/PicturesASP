using Microsoft.AspNetCore.Hosting;
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

        public static string RenameDuplicates(IHostingEnvironment env, string currentFolder, string fileName)
        {
            string ret = fileName;
            string filePath = env.WebRootPath + "\\" + currentFolder + "\\" + fileName;
            var folderFiles = Directory.EnumerateFiles(env.WebRootPath + "\\" + currentFolder);
            foreach (string item in folderFiles)
            {
                if (item.Equals(filePath))
                {
                    //file with same name exists
                    string name = Path.GetFileNameWithoutExtension(fileName);
                    string extension = fileName.Replace(name, "");
                    name = name + "c";
                    fileName = name + extension;
                    ret = RenameDuplicates(env, currentFolder, fileName);
                }
            }
            return ret;
        }

    }
}
