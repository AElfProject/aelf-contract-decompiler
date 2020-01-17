using System.Collections.Generic;
using Newtonsoft.Json;

namespace AElfContractDecoder.Models
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
    }

    public class SingleFile
    {
        [JsonProperty("name")] public string FileName { get; set; }

        [JsonProperty("content")] public string FileContent { get; set; }

        [JsonProperty("fileType")] public string FileType { get; set; }
    }
}