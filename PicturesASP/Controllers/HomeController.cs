using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PicturesASP.Models;
using PicturesASP.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PicturesASP.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment env;
        private readonly ImageProcessor imageProcessor;
        private readonly IConfiguration conf;

        public HomeController(IHostingEnvironment environment, ImageProcessor imgProc, IConfiguration Configuration)
        {
            this.env = environment;
            this.imageProcessor = imgProc;
            this.conf = Configuration;
        }

        [HttpGet]
        public IActionResult Index(string path)
        {
            string root = conf["gallery"];

            Folder rootFolder = new Folder();
            if (String.IsNullOrEmpty(path))
            {
                rootFolder.SubFolders = DisplayUtils.GetFolders(env, root);
                rootFolder.Images = DisplayUtils.GetDisplayImages(env, root);
                rootFolder.CurrentFolder = root;
            }
            else
            {
                string dirPath = Path.Combine(env.WebRootPath, path);
                DirectoryInfo dirInfo = new DirectoryInfo(dirPath);

                string rootName = dirInfo.Name;
                string parent = "";
                if (!rootName.Equals(root))
                {
                    string directoryName = Path.GetDirectoryName(dirPath);
                    parent = directoryName.Substring(directoryName.IndexOf(root));
                    rootFolder = new Folder(rootName, parent);
                }
                else
                {
                    rootFolder = new Folder()
                    {
                        Name = rootName,
                        CurrentFolder = rootName
                    };
                }

                rootFolder.SubFolders = DisplayUtils.GetFolders(env, path);
                rootFolder.Images = DisplayUtils.GetDisplayImages(env, path);

            }
            return View(rootFolder);
        }

        //create and save folder 
        [HttpPost]
        public IActionResult CreateFolder(string currentFolder, string name)
        {
            try
            {
                string path = Path.Combine(env.WebRootPath, currentFolder, name);
                var newFolder = Directory.CreateDirectory(path);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }
            return RedirectToAction("Index", "Home", new { path = currentFolder });
        }

        //create and save images. Redirect to folder view
        [HttpPost]
        public async Task<IActionResult> UploadImages(string currentFolder, ICollection<IFormFile> files)
        {
            try
            {
                foreach (var file in files.Where(f => DisplayUtils.IsImageFile(f.FileName)))
                {
                    imageProcessor.SaveImage(file, env, currentFolder);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                return View(new Folder { CurrentFolder = currentFolder });
            }

            return RedirectToAction("Index", "Home", new { path = currentFolder });
        }

        //delete the image and remain in the same folder view
        public IActionResult DeleteImage(string currentFolder, string link)
        {
            try
            {
                string filePath = Path.Combine(env.WebRootPath, link);
                System.IO.File.Delete(filePath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }

            return RedirectToAction("Index", "Home", new { path = currentFolder });
        }

        //delete the folder and return to root view
        public IActionResult DeleteFolder(string folderPath, string currentFolder)
        {
            try
            {
                string path = Path.Combine(env.WebRootPath, folderPath);

                //check if folder is empty before delete. skip delete if not
                if (!Directory.EnumerateFileSystemEntries(path).Any())
                {
                    Directory.Delete(path, true);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }
            return RedirectToAction("Index", "Home", new { path = currentFolder });
        }
    }
}
