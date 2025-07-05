using System.ComponentModel.DataAnnotations;

namespace DotTaskAPI.DTOs
{
    public class NotasCreacionDTO
    {
        [Required]
        public string Contenido { get; set; }

    }
}
