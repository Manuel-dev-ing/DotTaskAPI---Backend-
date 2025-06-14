using System.ComponentModel.DataAnnotations;

namespace DotTaskAPI.DTOs
{
    public class loginDTO
    {

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }


    }
}
