using System.ComponentModel.DataAnnotations;

namespace MiApiORACLE.DTO
{
    public class CustomerDTO
    {
        [Required]
        public int CustomerId { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? EmailAdress { get; set; }
        [Required]
        [MaxLength(50)]
        public string? FullName { get; set; }
    }
}