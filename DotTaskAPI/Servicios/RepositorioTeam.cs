using DotTaskAPI.DTOs;
using DotTaskAPI.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace DotTaskAPI.Servicios
{
    public interface IRepositorioTeam
    {
        Task<UsuarioDTO> buscar(string email);
        Task<Usuario> buscarPorId(int id);
        Task<ProyectosUsuario> buscarUsuarioPorProyecto(int proyectoId, int id);
        Task eliminar(ProyectosUsuario proyectosUsuario);
        Task<bool> existeUsuarioProyecto(int proyectoId, int id);
        Task guardar(ProyectosUsuario proyectosUsuario);
        Task<IEnumerable<UsuarioDTO>> obtenerMiembrosProyecto(int proyectoId);
        Task<ProyectosUsuario> obtenerProyectosUsuarioPorUsuario(int id);
    }

    public class RepositorioTeam: IRepositorioTeam
    {
        private readonly ApplicationDbContext context;

        public RepositorioTeam(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<UsuarioDTO>> obtenerMiembrosProyecto(int proyectoId)
        {
            var usuarios = await context.ProyectosUsuarios
                .Include(x => x.IdProyectoNavigation)
                .Include(x => x.IdUsuarioNavigation)
                .Where(x => x.IdProyecto == proyectoId)
                .Select(x => new UsuarioDTO()
                {
                    Id = x.Id,
                    Nombre = x.IdUsuarioNavigation.Nombre,
                    Email = x.IdUsuarioNavigation.Email

                }).ToListAsync();

            return usuarios;
        }

        public async Task<bool> existeUsuarioProyecto(int proyectoId, int id)
        {
            var resultado = await context.ProyectosUsuarios
                .AnyAsync(x => x.IdProyecto == proyectoId && x.IdUsuario == id);

            return resultado;
        }

        public async Task<UsuarioDTO> buscar(string email)
        {

            var usuario = await context.Usuarios.Select(x => new UsuarioDTO()
            {
                Id = x.Id,
                Nombre = x.Nombre,
                Email = x.Email
            }).FirstOrDefaultAsync(x => x.Email == email);


            return usuario;
        }

        public async Task<Usuario> buscarPorId(int id)
        {

            var usuario = await context.Usuarios.FirstOrDefaultAsync(x => x.Id == id);


            return usuario;
        }

        public async Task<ProyectosUsuario> buscarUsuarioPorProyecto(int proyectoId, int id)
        {
            var usuario = await context.ProyectosUsuarios.FirstOrDefaultAsync(x => x.IdProyecto == proyectoId && x.IdUsuario == id);

            return usuario;
        }

        public async Task<ProyectosUsuario> obtenerProyectosUsuarioPorUsuario(int id)
        {
            var usuario = await context.ProyectosUsuarios.FirstOrDefaultAsync(x => x.IdUsuario == id);

            return usuario;
        }

        public async Task guardar(ProyectosUsuario proyectosUsuario)
        {
            context.ProyectosUsuarios.Add(proyectosUsuario);
            await context.SaveChangesAsync();
        }

        public async Task eliminar(ProyectosUsuario proyectosUsuario)
        {
            context.ProyectosUsuarios.Remove(proyectosUsuario);
            await context.SaveChangesAsync();
        }


    }
}
