using System.IO;

namespace PicturesASP.Models
{
    public class Image
    {
        //file name
        public string Name { get; set; }

        //disk address, without webroot
        public string Link { get; set; }

        //folder containing the file
        public string ParentFolder { get; set; }

        public Image() { }

        public Image(string name, string parentFolder)
        {
            Name = name;
            ParentFolder = parentFolder;
            Link = Path.Combine(parentFolder, name); //DO NOT prefix link with WebRoothPath. Error in VS
        }
    }
}
