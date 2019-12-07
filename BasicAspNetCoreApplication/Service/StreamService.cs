using DecompileProj;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace BasicAspNetCoreApplication.Service
{
    public interface IStreamService
    {
        Task<Stream> GetStreamAsync(string[] args); // To transfer files by stream.
        Task GetLSpyOutputPathAsync(string[] args); // DLL Output path.
    }

    public class StreamService : IStreamService, ITransientDependency
    {
        //private const string FilePath = "/Users/aelf/TestProgram/Test2/Decompiled.zip";

        public async Task<Stream> GetStreamAsync(string[] args)
        {
            byte[] bytes;
            var filePath = args.FirstOrDefault(Directory.Exists);
            if (filePath == null)
                return null;

            await using (var fs = new FileStream(filePath, FileMode.Open))
            {
                bytes = new byte[(int)fs.Length];
                await fs.ReadAsync(bytes, 0, bytes.Length);
            }

            Stream stream = new MemoryStream(bytes);
            return stream;
        }

        public async Task GetLSpyOutputPathAsync(string[] args)
        {
            await ILSpyCmdProgram.GetDecompiledDir(args);
        }
    }
}