using System.ComponentModel.DataAnnotations;

namespace AElfContractDecompiler.Models
{
    public class Base64StringDto
    {
        [Required]
        public string Base64String { get; set; }
    }
}
