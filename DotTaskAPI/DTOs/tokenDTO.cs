using System.ComponentModel.DataAnnotations;

namespace DotTaskAPI.DTOs
{
    public class tokenDTO
    {
        [Required]
        public int Token { get; set; }
    }
}
