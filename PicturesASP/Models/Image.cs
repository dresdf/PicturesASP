using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PicturesASP.Models
{
    public class Image
    {
        public string Name { get; set; }
        public string Link { get; set; }
        public string ParentFolder { get; set; }
        public string DisplayLink { get; set; }

        public Image() { }

        public Image(string name, string parentFolder)
        {
            Name = name;
            ParentFolder = parentFolder;
            Link = parentFolder + "\\" + name; //DO NOT prefix link with WebRoothPath. Error in VS
        }
    }
}
