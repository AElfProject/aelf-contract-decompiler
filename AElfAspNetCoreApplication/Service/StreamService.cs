using DecompileProj;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace AElfAspNetCoreApplication.Service
{
    public interface IStreamService
    {
        Task GetLSpyOutputPathAsync(string[] args); // DLL Output path.
    }

    public class StreamService : IStreamService, ITransientDependency
    {
        public async Task GetLSpyOutputPathAsync(string[] args)
        {
            var iLSpyCmd = new ILSpyCmdProgram();
            await iLSpyCmd.GetDecompiledDir(args);
        }
    }
}