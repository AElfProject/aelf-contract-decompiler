using System;
using System.IO;

namespace AElfContractDecompiler.Models
{
    public static class DecoderConstants
    {
        private static readonly string UserPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        // directory of decompiled contracts
        public static readonly string ContractsPath = Path.Combine(UserPath, "ContractsDecompiled");

        // directory of input dll
        public static readonly string DllPath = Path.Combine(UserPath, "DllInputs");
    }
}