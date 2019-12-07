using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using AElfAspNetCoreApplication.Extension;
using AElfAspNetCoreApplication.Service;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.AspNetCore.Http;
using AElfAspNetCoreApplication.Models;
using Newtonsoft.Json;

namespace AElfAspNetCoreApplication.Controllers
{
    interface IRegularController
    {
        Task<IActionResult> GetFilesByBase64Async(Base64InfoDto base64InfoDto);
    }

    public class AElfInfoController : AbpController, IRegularController
    {
        private const string OutPathByDll = @"C:\\Xxx\\OutPathByDll"; // necessary
        private const string SystemPath = @"C:\\Xxx\\TestDll"; // necessary
        
        private readonly IStreamService _streamService;
        private readonly IResponseService _responseService;
        private new ILogger<AElfInfoController> Logger { get; }

        public AElfInfoController(IStreamService streamService, IResponseService responseService,
            ILogger<AElfInfoController> logger)
        {
            _streamService = streamService;
            _responseService = responseService;
            Logger = logger;
        }

        [Route("GetFiles")]
        [HttpPost("GetFiles")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFilesByBase64Async([FromBody]Base64InfoDto base64InfoDto)
        {
            try
            {
                var base64String = base64InfoDto.Base64String.Trim();
                
                if (!base64String.IsBase64String() || string.IsNullOrEmpty(base64String))
                {
                    Logger.LogError($"Invalid input.");
                    return Json(new { status = "error", message = "Invalid input" });
                };
                var bytes = Convert.FromBase64String(base64String);

                var name = DateTime.UtcNow.ToString("yyyy_MM_dd_HH_mm_ss");
                var dllName = name + ".dll";
                var dllPath = Path.Combine(SystemPath, dllName);

                var isWriteBytesToDllSuccess = await ByteArrayToFileAsync(dllPath, bytes);
                if (isWriteBytesToDllSuccess == false)
                {
                    Logger.LogError($"Write bytes to dll failed!");
                    return Json(new { status = "error", message = "Write bytes to dll failed!" });
                }

                var outputPath = Path.Combine(OutPathByDll, $"{name}");
                if (!Directory.Exists(outputPath))
                {
                    Directory.CreateDirectory(outputPath);
                }

                string[] args = { "-p", "-o", $"{outputPath}", $"{dllPath}" };

                try
                {
                    await _streamService.GetLSpyOutputPathAsync(args);
                }
                catch (Exception e)
                {
                    Logger.LogError($"LSpyCmdProgram's Execute meet errors : {e.Message}");
                    return Json(new { status = "error", message = "ILsycmd execute failed." });
                }

                if (!Directory.Exists(outputPath))
                {
                    Logger.LogError($"ILsycmd execute failed.");
                    return Json(new { status = "error", message = "ILsycmd execute failed." });
                }

                var jsonResult = await _responseService.GetDictJsonByPath(outputPath);

                Logger.LogDebug("Get json from decompiled files successfully.");

                return Json(jsonResult, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            }
            catch (Exception e)
            {
                Logger.LogError($"Get decompiled files failed : {e.Message}");
                throw;
            }
        }

        #region private methods
        private async Task<bool> ByteArrayToFileAsync(string fileName, byte[] byteArray)
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

        #endregion
    }
}