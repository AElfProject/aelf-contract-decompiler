using System.ComponentModel.DataAnnotations;

namespace AElfContractDecoder.Models
{
    public class Base64StringDto
    {
        [Required]
        public string Base64String { get; set; }
    }
}
