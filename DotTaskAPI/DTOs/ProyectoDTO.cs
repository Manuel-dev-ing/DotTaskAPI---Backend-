using DotTaskAPI.Entidades;

namespace DotTaskAPI.DTOs
{
    public class ProyectoDTO
    {

        public int Id { get; set; }

        public string NombreProyecto { get; set; }

        public string NombreCliente { get; set; }

        public string Descripcion { get; set; }

        public virtual IEnumerable<TareaDTO> Tareas { get; set; }


    }
}
