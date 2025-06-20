namespace DotTaskAPI.DTOs
{
    public class ProyectosUsuariosDTO
    {
        public int Id { get; set; }

        public int IdProyecto { get; set; }

        public int IdUsuario { get; set; }

        public bool IsManager { get; set; }

        public DateTime FechaAsignacion { get; set; }

    }
}
