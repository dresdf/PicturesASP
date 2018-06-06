using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
                string dirPath = env.WebRootPath + "\\gallery";
                rootFolder.CurrentFolder = root;
            }
            else
            {
                rootFolder.SubFolders = DisplayUtils.GetFolders(env, path);
                rootFolder.Images = DisplayUtils.GetDisplayImages(env, path);
                string dirPath = env.WebRootPath + "\\" + path;
                rootFolder.CurrentFolder = path;
                string rootName = new DirectoryInfo(dirPath).Name;
                string parent = path.Replace("\\" + rootName, "");
                rootFolder.Parent = parent;
            }
            return View(rootFolder);
        }

        //load create page
        [HttpGet]
        public IActionResult Create(string currentFolder)
        {
            Folder ret = new Folder()
            {
                CurrentFolder = currentFolder,
                Name = new DirectoryInfo(currentFolder).Name
            };
            ret.SubFolders = DisplayUtils.GetFolders(env, currentFolder);
            ret.Parent = currentFolder.Replace("\\" + ret.Name, "");
            return View(ret);
        }

        //create and save folder 
        [HttpPost]
        public IActionResult Create(string currentFolder, string name)
        {
            string path = env.WebRootPath + "\\" + currentFolder + "\\" + name;
            var newFolder = Directory.CreateDirectory(path);

            return RedirectToAction("Index", "Home", new { path = currentFolder });
        }

        //load upload page
        [HttpGet]
        public IActionResult Upload(string currentFolder)
        {
            Folder ret = new Folder()
            {
                CurrentFolder = currentFolder,
            };
            return View(ret);
        }

        //create and save images. Redirect to folder view
        [HttpPost]
        public async Task<IActionResult> Upload(string currentFolder, ICollection<IFormFile> files)
        {
            try
            {
                foreach (var file in files)
                {
                    if (!ImageUtils.IsImageFile(file.FileName))
                    {
                        continue;
                    }

                    //TODO: check if file with same name exists

                    string fileName = Path.GetFileName(file.FileName);
                    string name = Path.GetFileNameWithoutExtension(fileName);
                    string filePath = env.WebRootPath + "\\" + currentFolder + "\\" + fileName;
                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                }
            }
            catch (Exception)
            {
                return View(new Folder { CurrentFolder = currentFolder });
            }

            return RedirectToAction("Index", "Home", new { path = currentFolder });
        }
    }
}
