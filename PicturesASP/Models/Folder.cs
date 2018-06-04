﻿using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PicturesASP.Models
{
    public class Folder
    {
        public string Name { get; set; }
        public string UrlName { get; set; }
        public bool HasChildren { get; set; }
        public List<Folder> SubFolders { get; set; }
        public List<Image> Images { get; set; }

    }
}
