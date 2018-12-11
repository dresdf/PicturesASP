using System.Collections.Generic;

namespace PicturesASP.Models
{
    public class Folder
    {
        //name of the folder
        public string Name { get; set; }

        //full url of the current folder, no webroot
        public string CurrentFolder { get; set; }

        //full url of the parent folder, no webroot
        public string Parent { get; set; }    
        public List<Folder> SubFolders { get; set; }
        public List<Image> Images { get; set; }

        public Folder() { }

        public Folder(string name, string parent)
        {
            Name = name;
            Parent = parent;
            CurrentFolder = parent + "\\" + name;
        }
    }
}
