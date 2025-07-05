using DotTaskAPI.Entidades;

namespace DotTaskAPI.DTOs
{
    public class NotaDTO
    {
        public int Id { get; set; }

        public int IdTarea { get; set; }

        public string Contenido { get; set; }

        public int CreadoPor { get; set; }
        public CreadoPorDTO NotaCreadoPor { get; set; }


    }
}
