using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PicturesASP.Models;
using PicturesASP.Utils;

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
            string root = "gallery";

            Folder rootFolder = new Folder();          
            if (String.IsNullOrEmpty(path))
            {
                rootFolder.SubFolders = DisplayUtils.GetFolders(env, root);
                rootFolder.Images = DisplayUtils.GetDisplayImages(env, root);
                string dirPath = env.WebRootPath + "\\gallery" + "\\" + path;
                rootFolder.CurrentFolder = dirPath;
            }
            else
            {
                rootFolder.SubFolders = DisplayUtils.GetFolders(env,path);
                rootFolder.Images = DisplayUtils.GetDisplayImages(env, path);
                string dirPath = env.WebRootPath + "\\gallery" + "\\" + path;
                rootFolder.CurrentFolder = dirPath;
            }

            return View(rootFolder);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult Create(string currentFolder)
        {
            Folder ret = new Folder()
            {
                CurrentFolder = env.WebRootPath + "\\gallery" + "\\" + currentFolder
            };
            return View(ret);
        }

        [HttpPost]
        public IActionResult CreateFolder(string currentFolder, string name)
        {
            string path = currentFolder + "\\" + name;
            var newFolder = Directory.CreateDirectory(path);

            return RedirectToAction("Index");
        }
    }
}
