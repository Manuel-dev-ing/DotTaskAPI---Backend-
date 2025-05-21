using System.ComponentModel.DataAnnotations;

namespace DotTaskAPI.DTOs
{
    public class ProyectosCreacionDTO
    {

        [Required]
        public string NombreProyecto { get; set; }

        [Required]
        public string NombreCliente { get; set; }

        public string Descripcion { get; set; }

    }
}
