using System.Reflection.PortableExecutable;

namespace DotTaskAPI.DTOs
{
    public class RespuestaAutenticacionDTO
    {
        public string Token { get; set; }
        public DateTime Expiracion { get; set; }
    }
}
