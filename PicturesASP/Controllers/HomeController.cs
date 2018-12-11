﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public HomeController(IHostingEnvironment environment, ImageProcessor imgProc)
        {
            this.env = environment;
            this.imageProcessor = imgProc;
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
                rootFolder.CurrentFolder = root;
            }
            else
            {
                string dirPath = env.WebRootPath + "\\" + path;
                string rootName = new DirectoryInfo(dirPath).Name;
                string parent = path.Replace("\\" + rootName, "");
                rootFolder = new Folder(rootName, parent)
                {
                    SubFolders = DisplayUtils.GetFolders(env, path),
                    Images = DisplayUtils.GetDisplayImages(env, path),
                };
            }
            return View(rootFolder);
        }

        //create and save folder 
        [HttpPost]
        public IActionResult CreateFolder(string currentFolder, string name)
        {
            try
            {
                string path = env.WebRootPath + "\\" + currentFolder + "\\" + name;
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
                string filePath = env.WebRootPath + "\\" + link;
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
                string path = env.WebRootPath + "\\" + folderPath;

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
