using System.Security.Claims;
using DotTaskAPI.DTOs;
using DotTaskAPI.Entidades;
using Microsoft.EntityFrameworkCore;

namespace DotTaskAPI.Servicios
{
    public interface IRepositorioUsuarios
    {
        Task actualizar(Usuario usuario);
        Task<bool> existeUsuarioPorCorreo(string correo);
        Task guardar(Usuario usuario);
        Task<string> obtenerInformacionJWT();
        Task<UsuarioAutenticadoDTO> obtenerUsuarioAutenticado();
        Task<Usuario> obtenerUsuarioId(int id);
        Task<Usuario> obtenerUsuarioPorCorreo(string correo);
    }


    public class RepositorioUsuarios: IRepositorioUsuarios
    {
        private readonly ApplicationDbContext context;
        private readonly IHttpContextAccessor contextAccessor;

        public RepositorioUsuarios(ApplicationDbContext context, IHttpContextAccessor contextAccessor)
        {
            this.context = context;
            this.contextAccessor = contextAccessor;
        }


        public async Task<UsuarioAutenticadoDTO> obtenerUsuarioAutenticado()
        {
            var nameIdentifier = contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var Id = int.Parse(nameIdentifier!);

            var user = await context.Usuarios
                .Include(x => x.IdRolNavigation)
                .FirstOrDefaultAsync(x => x.Id == Id);
           

            var usuario = new UsuarioAutenticadoDTO()
            {
                Id = Id,
                Rol = user.IdRolNavigation.Nombre,
                Nombre = user.Nombre,
                Email = user.Email
            };

            return usuario;
        }

        public async Task<string> obtenerInformacionJWT()
        {
            var nameIdentifier = contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (nameIdentifier is null)
            {
                return null;
            }

            return nameIdentifier;
        }

        public async Task<Usuario> obtenerUsuarioPorCorreo(string correo)
        {
            var resultado = await context.Usuarios
                .Include(x => x.IdRolNavigation)
                .FirstOrDefaultAsync(x => x.Email == correo);

            return resultado;
        }

        public async Task<Usuario> obtenerUsuarioId(int id)
        {
            var usuario = await context.Usuarios.FirstOrDefaultAsync(x => x.Id == id);

            return usuario;
        }

        public async Task<bool> existeUsuarioPorCorreo(string correo)
        {
            var resultado = await context.Usuarios.AnyAsync(x => x.Email == correo);

            return resultado;
        }

        public async Task guardar(Usuario usuario)
        {
            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync();
        
        }

        public async Task actualizar(Usuario usuario)
        {
            context.Usuarios.Update(usuario);
            await context.SaveChangesAsync();

        }



    }
}
