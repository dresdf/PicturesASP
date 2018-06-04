using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PicturesASP.Models;

namespace PicturesASP.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment env;

        public HomeController(IHostingEnvironment environment)
        {
            this.env = environment;
        }
        public IActionResult Index(string path)
        {
            string root = env.WebRootPath + "\\gallery";

            Folder gallery = new Folder()
            {
                SubFolders = GetFolders(root),
                Images = new List<Image>()
            };

            if (!String.IsNullOrEmpty(path))
            {
                gallery.Images = GetDisplayImages(path);
            }


            return View(gallery);
        }

        //create list of folders
        private List<Folder> GetFolders(string path)
        {
            List<Folder> result = new List<Folder>();
            string parent = new DirectoryInfo(path).Name;
            var folders = Directory.EnumerateDirectories(path);
            foreach (var item in folders)
            {
                Folder ret = new Folder
                {
                    Name = new DirectoryInfo(item).Name,
                    UrlName = parent + "\\" + new DirectoryInfo(item).Name
                };
                var subfolders = Directory.EnumerateDirectories(item);

                if (subfolders.Count() > 0)
                {
                    ret.HasChildren = true;
                    ret.SubFolders = GetFolders(item);
                }
                result.Add(ret);
            }
            return result;
        }

        //get list of images to display
        private List<Image> GetDisplayImages(string dirName)
        {
            List<Image> images = new List<Image>();
            string dirPath = env.WebRootPath + "\\gallery" + "\\" + dirName;
            string dirLink = "gallery" + "\\" + dirName;


            var folder = Directory.EnumerateFiles(dirPath);
            foreach (var item in folder)
            {
                Image img = new Image()
                {
                    Name = new DirectoryInfo(item).Name,

                };
                img.Link = dirLink + "\\" + img.Name;
                images.Add(img);
            }

            return images;
        }









        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
