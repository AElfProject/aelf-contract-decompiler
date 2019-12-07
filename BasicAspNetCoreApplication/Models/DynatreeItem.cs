using System.Collections.Generic;
using System.IO;

namespace BasicAspNetCoreApplication.Models
{
    public class DynatreeItem
    {
        public string Title { get; set; }
        public bool isFolder { get; set; }
        public string Key { get; set; }
        public List<DynatreeItem> children;

        public string FileFullPath { get; set; }

        public DynatreeItem(FileSystemInfo fsi)
        {
            Title = fsi.Name;
            children = new List<DynatreeItem>();

            if (fsi.Attributes == FileAttributes.Directory)
            {
                isFolder = true;
                foreach (FileSystemInfo f in (fsi as DirectoryInfo).GetFileSystemInfos())
                {
                    children.Add(new DynatreeItem(f));
                }
            }
            else
            {
                isFolder = false;
                FileFullPath = fsi.FullName;
            }
            Key = Title.Replace(" ", "").ToLower();
        }
    }
}