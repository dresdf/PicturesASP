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
                    Folder ret = new Folder
                    {
                        Name = new DirectoryInfo(item).Name
                    };
                    ret.UrlName = path + "\\" + ret.Name;
                    var subfolders = Directory.EnumerateDirectories(item);

                    if (subfolders.Count() > 0)
                    {
                        ret.HasChildren = true;
                        ret.SubFolders = GetFolders(env, ret.UrlName);
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
                    Image img = new Image
                    {
                        Name = new DirectoryInfo(item).Name,

                    };
                    //DO NOT prefix link with WebRoothPath. Error in VS
                    img.Link = dirUrlName + "\\" + img.Name;
                    images.Add(img);
                }
            }
            catch (DirectoryNotFoundException) { return images; }
            return images;
        }
    }
}
