using DecompileTool;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace AElfContractDecoder.Service
{
    public interface IContractDecoderService
    {
        Task ExecuteDecodeAsync(string[] args); // DLL Output path.
    }

    public class ContractDecoderService : IContractDecoderService, ITransientDependency
    {
        public async Task ExecuteDecodeAsync(string[] args)
        {
            var iLSpyCmd = new ILSpyCmdProgram();
            await iLSpyCmd.ExecuteDecodeAsync(args);
        }
    }
}