using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PicturesASP.Models;
using PicturesASP.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PicturesASP.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment env;

        public HomeController(IHostingEnvironment environment)
        {
            this.env = environment;
        }

        [HttpGet]
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

        //create and save folder 
        [HttpPost]
        public IActionResult CreateFolder(string currentFolder, string name)
        {
            string path = env.WebRootPath + "\\" + currentFolder + "\\" + name;
            var newFolder = Directory.CreateDirectory(path);

            return RedirectToAction("Index", "Home", new { path = currentFolder });
        }

        //create and save images. Redirect to folder view
        [HttpPost]
        public async Task<IActionResult> UploadImages(string currentFolder, ICollection<IFormFile> files)
        {
            try
            {
                foreach (var file in files)
                {
                    if (!ImageUtils.IsImageFile(file.FileName))
                    {
                        continue;
                    }

                    string fileName = Path.GetFileName(file.FileName);
                    fileName = ImageUtils.RenameDuplicates(env, currentFolder, fileName);
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

        public IActionResult DeleteImage(string currentFolder, string link)
        {
            string filePath = env.WebRootPath + "\\" + link;
            System.IO.File.Delete(filePath);
            return RedirectToAction("Index", "Home", new { path = currentFolder });
        }

        public IActionResult DeleteFolder(string folderPath, string currentFolder)
        {
            string path = env.WebRootPath + "\\" + folderPath;
            Directory.Delete(path, true);

            return RedirectToAction("Index", "Home", new { path = currentFolder });
        }
    }
}
