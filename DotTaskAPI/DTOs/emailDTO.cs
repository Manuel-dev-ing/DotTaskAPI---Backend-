using System.ComponentModel.DataAnnotations;

namespace DotTaskAPI.DTOs
{
    public class emailDTO
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }

    }
}
