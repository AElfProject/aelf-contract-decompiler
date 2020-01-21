using DecompileTool;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace AElfContractDecompiler.Service
{
    public interface IContractDecompileService
    {
        Task ExecuteDecompileAsync(string[] args); // DLL Output path.
    }

    public class ContractDecompileService : IContractDecompileService, ITransientDependency
    {
        public async Task ExecuteDecompileAsync(string[] args)
        {
            var iLSpyCmd = new ILSpyCmdProgram();
            await iLSpyCmd.DecompileAsync(args);
        }
    }
}