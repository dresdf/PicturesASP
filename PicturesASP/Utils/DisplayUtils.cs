using Microsoft.AspNetCore.Hosting;
using PicturesASP.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PicturesASP.Utils
{
    public class DisplayUtils
    {
        private static readonly string[] extensions = { ".jpg", ".jpeg", ".gif", ".png" };

        //create list of folders
        public static List<Folder> GetFolders(IHostingEnvironment env, string path)
        {
            List<Folder> result = new List<Folder>();
            try
            {
                string link = env.WebRootPath + "\\" + path;
                var folders = Directory.EnumerateDirectories(link);
                foreach (var item in folders)
                {
                    string dirName = new DirectoryInfo(item).Name;
                    if (dirName.ToLowerInvariant().Equals("thumbnails"))
                    {
                        continue;
                    }
                    Folder ret = new Folder(new DirectoryInfo(item).Name, path);
                    var subfolders = Directory.EnumerateDirectories(item);

                    if (subfolders.Count() > 0)
                    {
                        ret.SubFolders = GetFolders(env, ret.CurrentFolder);
                    }
                    result.Add(ret);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                return result;
            }
            return result;
        }

        //get list of images to display
        public static List<Image> GetDisplayImages(IHostingEnvironment env, string dirUrlName)
        {
            List<Image> images = new List<Image>();
            try
            {
                string folderUrl = env.WebRootPath + "\\" + dirUrlName;
                var folder = Directory.EnumerateFiles(folderUrl);
                foreach (var item in folder)
                {
                    Image img = new Image(new DirectoryInfo(item).Name, dirUrlName);
                    images.Add(img);
                }
            }
            catch (DirectoryNotFoundException) { return images; }
            return images;
        }

        //check if file is image type
        public static bool IsImageFile(string file)
        {
            string ext = Path.GetExtension(file);
            return extensions.Contains(ext, StringComparer.OrdinalIgnoreCase);
        }

        //search target for files with same name. renames the new file
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
                    name = name + "x";
                    fileName = name + extension;
                    ret = RenameDuplicates(env, currentFolder, fileName);
                }
            }
            return ret;
        }
    }
}
