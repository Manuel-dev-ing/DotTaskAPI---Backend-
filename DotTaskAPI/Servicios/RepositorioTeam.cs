using DotTaskAPI.DTOs;
using DotTaskAPI.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace DotTaskAPI.Servicios
{
    public interface IRepositorioTeam
    {
        Task actualizar(Usuario usuario);
        Task<UsuarioDTO> buscar(string email);
        Task<Usuario> buscarPorId(int id);
        Task<Usuario> buscarUsuarioPorProyecto(int proyectoId, int id);
        Task<bool> existeUsuarioProyecto(int proyectoId, int id);
        Task<IEnumerable<UsuarioDTO>> obtenerMiembrosProyecto(int proyectoId);
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
            var usuarios = await context.Usuarios
                .Where(x => x.IdProyecto == proyectoId)
                .Select(x => new UsuarioDTO()
                {
                    Id = x.Id,
                    Nombre = x.Nombre,
                    Email = x.Email
                
                }).ToListAsync();

            return usuarios;
        }

        public async Task<bool> existeUsuarioProyecto(int proyectoId, int id)
        {
            var resultado = await context.Usuarios
                .AnyAsync(x => x.IdProyecto == proyectoId && x.Id == id);

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

        public async Task<Usuario> buscarUsuarioPorProyecto(int proyectoId, int id)
        {
            var usuario = await context.Usuarios.FirstOrDefaultAsync(x => x.IdProyecto == proyectoId && x.Id == id);

            return usuario;
        }

        public async Task actualizar(Usuario usuario)
        {
            context.Usuarios.Update(usuario);
            await context.SaveChangesAsync();
        }


    }
}
