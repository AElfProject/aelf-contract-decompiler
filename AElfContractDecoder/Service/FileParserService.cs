﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AElfContractDecoder.Models;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace AElfContractDecoder.Service
{
    public interface IFileParserService
    {
        Task<ResponseTemplate> GetResponseTemplateByPath(string dictPath);
    }

    public class FileParserService : IFileParserService, ITransientDependency
    {
        private ILogger<FileParserService> Logger { get; set; }
        public FileParserService(ILogger<FileParserService> logger)
        {
            Logger = logger;
        }
        public async Task<ResponseTemplate> GetResponseTemplateByPath(string dictPath)
        {
            var request = new ResponseTemplate { Data = new List<SingleDirectory>() };
            try
            {
                if (!Directory.Exists(dictPath))
                {
                    Logger.LogWarning($"Invalid path : {dictPath}");
                    return null;
                }
                var treeItem = new DynatreeItem(new DirectoryInfo(dictPath));

                foreach (var item in treeItem.children)
                {
                    if (item.IsFolder)
                    {
                        var dict = new SingleDirectory
                        {
                            DictOrFileName = item.Title,
                            IsFolder = true,
                            Files = new List<SingleFile>()
                        };

                        foreach (var child in item.children)
                        {
                            if (!child.IsFolder && !child.Title.StartsWith('.'))
                            {
                                dict.Files.Add(new SingleFile
                                {
                                    FileName = child.Title,
                                    FileContent = await Base64StringFromBytes(child.FileFullPath), //fix
                                    FileType = GetFileType(child)
                                });
                            }

                            if (child.IsFolder)
                            {
                                dict.Directories = new List<SingleDirectory>
                                {
                                    new SingleDirectory {DictOrFileName = child.Title, IsFolder = true}
                                };
                            }
                        }

                        request.Data.Add(dict);
                    }

                    if (!item.IsFolder && !item.Title.StartsWith('.'))
                    {
                        var dict = new SingleDirectory
                        {
                            DictOrFileName = item.Title,
                            DictContent = await Base64StringFromBytes(item.FileFullPath),
                            IsFolder = false,
                            DictType = GetFileType(item)
                        };

                        request.Data.Add(dict);
                    }
                }
                request.Code = 0;
                request.Message = "success";

                return request;
            }
            catch (Exception e)
            {
                request.Code = -1;
                request.Message = $"failed:{e.Message}";
                return null;
            }
        }

        private static async Task<string> Base64StringFromBytes(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return "Invalid file path";
            }

            var content = await Base64StringFromDll(filePath);
            return content;
        }

        private static async Task<string> Base64StringFromDll(string path)
        {
            //Read base64String from bytes
            try
            {
                byte[] bytes;
                await using (var fs = new FileStream(path, FileMode.Open))
                {
                    bytes = new byte[(int)fs.Length];
                    await fs.ReadAsync(bytes, 0, bytes.Length);
                }

                var base64 = Convert.ToBase64String(bytes);
                return base64;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Read base64 from dll failed:{e.Message}");
                throw new Exception($"Read base64 from dll failed:{e.Message}");
            }
        }

        private static string GetFileType(DynatreeItem child)
        {
            var str = child.Title.Substring(child.Title.LastIndexOf('.')) == ".cs" ? "txt" : "xml";
            return str;
        }
    }
}