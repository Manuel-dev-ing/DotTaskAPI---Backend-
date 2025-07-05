using DotTaskAPI.Entidades;

namespace DotTaskAPI.DTOs
{
    public class TareaDTO
    {
        public int Id { get; set; }

        public int IdProyecto { get; set; }

        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        public string Estado { get; set; }

        public int? CompletadoPor { get; set; }

        public int? IdUsuario { get; set; }

        public string? NombreUsuario { get; set; }
        public string? EmailUsuario { get; set; }

        public virtual Proyecto? IdProyectoNavigation { get; set; }
        public virtual ICollection<HistorialCambiosTarea> HistorialCambiosTareas { get; set; } = new List<HistorialCambiosTarea>();
        public virtual ICollection<NotaDTO> Notas { get; set; } = new List<NotaDTO>();



    }
}
