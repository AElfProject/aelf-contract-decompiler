using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BasicAspNetCoreApplication.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Volo.Abp.DependencyInjection;

namespace BasicAspNetCoreApplication.Service
{
    public interface IResponseService
    {
        Task<string> GetDictJsonByPath(string dictPath);
    }

    public class ResponseService : IResponseService, ITransientDependency
    {
        private ILogger<ResponseService> Logger { get; set; }
        public ResponseService()
        {
            Logger = NullLogger<ResponseService>.Instance;
        }
        public async Task<string> GetDictJsonByPath(string dictPath)
        {
            var request = new TemplateResponse { Data = new List<SingleDirectory>() };
            try
            {
                if (!Directory.Exists(dictPath))
                {
                    Logger.LogWarning($"Invalid path : {dictPath}");
                    return null;
                }
                DynatreeItem treeItem = new DynatreeItem(new DirectoryInfo(dictPath));

                foreach (var item in treeItem.children)
                {
                    if (item.isFolder)
                    {
                        var dict = new SingleDirectory
                        {
                            DictOrFileName = item.Title,
                            IsFolder = true,
                            Files = new List<SingleFile>()
                        };

                        foreach (var child in item.children)
                        {
                            if (!child.isFolder && !child.Title.StartsWith('.'))
                            {
                                dict.Files.Add(new SingleFile
                                {
                                    FileName = child.Title,
                                    FileContent = await Base64StringFromBytes(child.FileFullPath), //fix
                                    FileType = child.Title.Substring(child.Title.LastIndexOf('.'))
                                });
                            }

                            if (child.isFolder)
                            {
                                dict.Directories = new List<SingleDirectory>
                                {
                                    new SingleDirectory {DictOrFileName = child.Title, IsFolder = true}
                                };
                            }
                        }

                        request.Data.Add(dict);
                    }

                    if (!item.isFolder && !item.Title.StartsWith('.'))
                    {
                        var dict = new SingleDirectory
                        {
                            DictOrFileName = item.Title,
                            DictContent = await Base64StringFromBytes(item.FileFullPath)
                        };

                        request.Data.Add(dict);
                    }
                }
                request.Code = 0;
                request.Msg = "success";
                var response = request.JsonToDynatree();

                return response;
            }
            catch (Exception e)
            {
                request.Code = -1;
                request.Msg = $"failed:{e.Message}";
                return null;
            }
        }

        private async Task<string> Base64StringFromBytes(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                return "Invalid file path";
            }

            var content = await ReadBase64FromDll(filePath);
            return content;
        }

        static async Task<string> ReadBase64FromDll(string path)
        {
            //Read base64String from bytes from file
            var getBytes = await File.ReadAllBytesAsync(path);
            var base64 = Convert.ToBase64String(getBytes);
            return base64;
        }
    }
}