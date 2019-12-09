using System.Collections.Generic;
using System.IO;

namespace AElfAspNetCoreApplication.Models
{
    public class DynatreeItem
    {
        public string Title { get; set; }
        public bool IsFolder { get; set; }
        public string Key { get; set; }
        public List<DynatreeItem> children;

        public string FileFullPath { get; set; }

        public DynatreeItem(FileSystemInfo fsi)
        {
            Title = fsi.Name;
            children = new List<DynatreeItem>();

            if (fsi.Attributes == FileAttributes.Directory)
            {
                IsFolder = true;
                foreach (FileSystemInfo f in (fsi as DirectoryInfo).GetFileSystemInfos())
                {
                    children.Add(new DynatreeItem(f));
                }
            }
            else
            {
                IsFolder = false;
                FileFullPath = fsi.FullName;
            }
            Key = Title.Replace(" ", "").ToLower();
        }
    }
}