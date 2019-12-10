using System.ComponentModel.DataAnnotations;

namespace AElfContractDecoder.Models
{
    public class Base64InfoDto
    {
        [Required]
        public string Base64String { get; set; }
    }
}
