using System.ComponentModel.DataAnnotations;

namespace DotTaskAPI.DTOs
{
    public class VerificarPasswordDTO
    {
        [Required]
        [MinLength(8, ErrorMessage = "El password es muy corto, minimo 8 caracteres")]
        public string Password { get; set; }

    }
}
