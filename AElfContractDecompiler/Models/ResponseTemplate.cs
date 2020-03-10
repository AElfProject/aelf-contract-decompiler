using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AElfContractDecompiler.Models
{
    public class ResponseTemplate
    {
        [JsonProperty("code")] public int Code { get; set; }

        [JsonProperty("msg")] public string Message { get; set; }

        [JsonProperty("data")] public List<SingleDirectory> Data { get; set; }
    }

    public class SingleDirectory
    {
        [JsonProperty("name")] public string DictOrFileName { get; set; }

        [JsonProperty("content")] public string DictContent { get; set; }

        [JsonProperty("files")] public List<SingleFile> Files { get; set; }

        [JsonProperty("Directories")] public List<SingleDirectory> Directories { get; set; }

        [JsonProperty("fileType")] public string DictType { get; set; }

        [JsonIgnore] public bool IsFolder { get; set; }
        [JsonIgnore] public string FileFullPath { get; set; }

        public SingleDirectory(DynatreeItem item)
        {
            if (item == null)
            {
                throw new NullReferenceException("DynatreeItem's null.");
            }

            DictOrFileName = item.Title;
            IsFolder = item.IsFolder;
            Files = new List<SingleFile>();
            Directories = new List<SingleDirectory>();

            if (!item.IsFolder && !item.Title.StartsWith('.'))
            {
                DictType = GetFileType(item);
                FileFullPath = item.FileFullPath;
            }

            else if (item.IsFolder)
            {
                foreach (var child in item.children)
                {
                    var dict = new SingleDirectory(child);
                    if (dict.IsFolder)
                    {
                        Directories = new List<SingleDirectory> {dict};
                    }
                    else
                    {
                        Files.Add(new SingleFile
                            {
                                FileName = child.Title, FileType = GetFileType(child), FileFullPath = child.FileFullPath
                            });
                    }
                }
            }
        }
        
        private static string GetFileType(DynatreeItem child)
        {
            var str = child.Title.Substring(child.Title.LastIndexOf('.')) == ".cs" ? "txt" : "xml";
            return str;
        }
    }

    public class SingleFile
    {
        [JsonProperty("name")] public string FileName { get; set; }

        [JsonProperty("content")] public string FileContent { get; set; }

        [JsonProperty("fileType")] public string FileType { get; set; }
        [JsonIgnore] public string FileFullPath { get; set; }
    }
}