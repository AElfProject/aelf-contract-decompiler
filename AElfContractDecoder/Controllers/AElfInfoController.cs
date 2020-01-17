using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using AElfContractDecoder.Extension;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using AElfContractDecoder.Models;
using AElfContractDecoder.Service;
using Newtonsoft.Json;

namespace AElfContractDecoder.Controllers
{
    interface IRegularController
    {
        Task<IActionResult> GetFilesFromBase64Async(Base64StringDto base64StringDto);
    }

    public class AElfInfoController : AbpController, IRegularController
    {
        private readonly IContractDecoderService _contractDecoderService;
        private readonly IFileParserService _fileParserService;
        private new ILogger<AElfInfoController> Logger { get; }

        public AElfInfoController(IContractDecoderService contractDecoderService, IFileParserService fileParserService,
            ILogger<AElfInfoController> logger)
        {
            _contractDecoderService = contractDecoderService;
            _fileParserService = fileParserService;
            Logger = logger;
        }

        [Route("GetFiles")]
        [HttpPost("GetFiles")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFilesFromBase64Async([FromBody] Base64StringDto base64StringDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                        {status = "error", message = "Invalid input.", code = StatusCodes.Status400BadRequest});
                }

                var base64String = base64StringDto?.Base64String.Trim();

                if (!base64String.IsBase64String() || string.IsNullOrEmpty(base64String))
                {
                    Logger.LogError("Invalid input.");
                    return BadRequest(new
                        {status = "error", message = "Invalid input.", code = StatusCodes.Status400BadRequest});
                }

                var bytesFromBase64 = Convert.FromBase64String(base64String);
                var name = GetUniqueName();
                var dllPath = Path.Combine(DecoderConstants.SystemPath, name + ".dll");

                var isWriteBytesToDllSuccess = await ConvertBytesToFileAsync(dllPath, bytesFromBase64);
                if (isWriteBytesToDllSuccess == false)
                {
                    Logger.LogError($"Write bytes to dll failed!");
                    return Json(new
                    {
                        status = "error", message = "Write bytes to dll failed!", code = StatusCodes.Status400BadRequest
                    });
                }

                var outputPath = Path.Combine(DecoderConstants.OutPathByDll, $"{name}");
                CheckValidDirectory(outputPath);
                string[] args = {"-p", "-o", $"{outputPath}", $"{dllPath}"};
                await _contractDecoderService.ExecuteDecodeAsync(args);

                var response = await _fileParserService.GetResponseTemplateByPath(outputPath);
                Logger.LogDebug("Get json from decompiled files successfully.");

                return Json(response, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            }
            catch (Exception e)
            {
                Logger.LogError($"Get decompiled files failed : {e.Message}");
                return BadRequest(new {status = "error", message = $"{e.Message}", code = StatusCodes.Status400BadRequest});
            }
        }

        #region private methods

        private async Task<bool> ConvertBytesToFileAsync(string fileName, byte[] byteArray)
        {
            try
            {
                await using var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                await fs.WriteAsync(byteArray, 0, byteArray.Length);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError("Exception caught in process: {0}", ex);
                return false;
            }
        }

        private static void CheckValidDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private static string GetUniqueName()
        {
            var name = DateTime.UtcNow.ToString("yyyy_MM_dd_HH_mm_ss");
            var guid = Guid.NewGuid().ToString().Substring(0, 10);
            name = string.Concat(new[] {name, guid});
            return name;
        }

        #endregion
    }
}