using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AElfContractDecompiler.Models;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace AElfContractDecompiler.Service
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
                    var single = new SingleDirectory(item);
                    await FillContentsAsync(single);
                    request.Data.Add(single);
                }
                request.Code = 0;
                request.Message = "success";
                request.Version = await GetVersionAsync(Path.Combine(dictPath, "Properties","AssemblyInfo.cs"));
                return request;
            }
            catch (Exception e)
            {
                request.Code = -1;
                request.Message = $"error : {e.Message}";
                return request;
            }
        }

        private static async Task FillContentsAsync(SingleDirectory item)
        {
            if (!item.IsFolder)
            {
                item.DictContent = await Base64StringFromBytes(item.FileFullPath);
                return;
            }

            if (item.Files != null)
            {
                foreach (var file in item.Files)
                {
                    file.FileContent = await Base64StringFromBytes(file.FileFullPath);
                }
            }

            if (item.Directories != null)
            {
                foreach (var child in item.Directories)
                {
                    await FillContentsAsync(child);
                }
            }
        }

        private static async Task<string> GetVersionAsync(string filePath)
        {
            try
            {
                var array = (await File.ReadAllLinesAsync(filePath)).LastOrDefault();
                if (string.IsNullOrEmpty(array))
                {
                    return "assembly info not exists.";
                }

                var i = array.IndexOf('"');
                var j = array.LastIndexOf('"');
                var version = array.Substring(i + 1, j - i - 1);
                return version;
            }
            catch (Exception e)
            {
                return $"{e.Message}";
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
    }
}