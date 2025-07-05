using DotTaskAPI.DTOs;
using DotTaskAPI.Entidades;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace DotTaskAPI.Servicios
{
    public interface IRepositorioProyectos
    {
        Task actualizarProyecto(Proyecto proyecto);
        Task eliminarProyecto(Proyecto proyecto);
        Task eliminarProyectoUsuario(ProyectosUsuario proyectosUsuario);
        Task<bool> existeProyecto(int id);
        Task<ProyectoDTO> guardarProyecto(Proyecto proyecto);
        Task<Proyecto> obtenerProyecto(int id);
        Task<ProyectoDTO> ObtenerProyectoIdManager(int id, int manager_id);
        Task<ProyectoDTO> ObtenerProyectoPorId(int id);
        Task<IEnumerable<ProyectoDTO>> obtenerProyectos(int manager);
        Task<ProyectosUsuario> ObtenerProyectosUsuario(int id);
    }

    public class RepositorioProyectos : IRepositorioProyectos
    {
        private readonly ApplicationDbContext context;

        public RepositorioProyectos(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<Proyecto> obtenerProyecto(int id)
        {
            var proyecto = await context.Proyectos
                            .Include(x => x.Tareas)
                                .ThenInclude(t => t.Nota)
                            .FirstOrDefaultAsync(x => x.Id == id);
            return proyecto;
        }

        public async Task<bool> existeProyecto(int id)
        {
            var existe = await context.ProyectosUsuarios.AnyAsync(x => x.IdProyecto == id);

            return existe;
        }

        public async Task<IEnumerable<ProyectoDTO>> obtenerProyectos(int manager)
        {
            var proyectos = await context.ProyectosUsuarios
                .Include(x => x.IdProyectoNavigation)
                .Include(x => x.IdUsuarioNavigation)
                    .ThenInclude(r => r.IdRolNavigation)
                .Where(x => x.IdUsuario == manager)
                .Select(x => new ProyectoDTO()
                {
                    Id = x.IdProyectoNavigation.Id,
                    RolNombre = x.IdUsuarioNavigation.IdRolNavigation.Nombre,
                    Is_Manager = (bool)x.IsManager,
                    NombreProyecto = x.IdProyectoNavigation.NombreProyecto,
                    NombreCliente = x.IdProyectoNavigation.NombreCliente,
                    Descripcion = x.IdProyectoNavigation.Descripcion,
                    Tareas = x.IdProyectoNavigation.Tareas
                    .Select(x => new TareasDTO()
                    {
                        Id = x.Id,
                        Nombre = x.Nombre,
                        IdProyecto = (int)x.IdProyecto,
                        Descripcion = x.Descripcion,
                        Estado = x.Estado
                    }).ToList()

                }).ToListAsync();

            return proyectos;
        }

        private async Task<IEnumerable<TareaDTO>> obtenerTareaDTOs(int IdProyecto)
        {
            var tareasDTO = await context.Tareas
                .Where(x => x.IdProyecto == IdProyecto)
                .Select(x => new TareaDTO()
                {
                    Id = x.Id,
                    IdProyecto = (int)x.IdProyecto,
                    Nombre = x.Nombre,
                    Descripcion = x.Descripcion,
                    Estado = x.Estado

                }).ToListAsync();

            return tareasDTO;
        }

        public async Task<ProyectoDTO> ObtenerProyectoIdManager(int id, int manager_id)
        {
            var proyectoDTO = await context.Proyectos
                .Include(x => x.Tareas)
                .Include(x => x.ProyectosUsuarios)
                .Select(x => new ProyectoDTO()
                {
                    Id = x.Id,
                    Is_Manager = x.ProyectosUsuarios.FirstOrDefault(x => x.IdProyecto == id && x.IdUsuario == manager_id).IsManager.Value,
                    NombreProyecto = x.NombreProyecto,
                    NombreCliente = x.NombreCliente,
                    Descripcion = x.Descripcion,
                    Tareas = x.Tareas.Select(x => new TareasDTO()
                    {
                        Id = x.Id,
                        Nombre = x.Nombre,
                        IdProyecto = (int)x.IdProyecto,
                        Descripcion = x.Descripcion,
                        Estado = x.Estado

                    }).ToList()
                })
                .FirstOrDefaultAsync(x => x.Id == id);

            return proyectoDTO;
        }

        public async Task<ProyectoDTO> ObtenerProyectoPorId(int id)
        {
            var proyectoDTO = await context.Proyectos
                .Include(x => x.Tareas)
                .Include(x => x.ProyectosUsuarios)
                .Select(x => new ProyectoDTO()
                {
                    Id = x.Id,
                    NombreProyecto = x.NombreProyecto,
                    NombreCliente = x.NombreCliente,
                    Descripcion = x.Descripcion,
                    Tareas = x.Tareas.Select(x => new TareasDTO()
                    {
                        Id = x.Id,
                        Nombre = x.Nombre,
                        IdProyecto = (int)x.IdProyecto,
                        Descripcion = x.Descripcion,
                        Estado = x.Estado

                    }).ToList()
                })
                .FirstOrDefaultAsync(x => x.Id == id);

            return proyectoDTO;
        }


        public async Task<ProyectoDTO> guardarProyecto(Proyecto proyecto)
        {

            context.Proyectos.Add(proyecto);
            await context.SaveChangesAsync();


            var proyectoDTO = new ProyectoDTO()
            {
                Id = proyecto.Id,
                NombreProyecto = proyecto.NombreProyecto,
                NombreCliente = proyecto.NombreCliente,
                Descripcion = proyecto.Descripcion,
            };

            return proyectoDTO;
        }

        public async Task actualizarProyecto(Proyecto proyecto)
        {
            context.Proyectos.Update(proyecto);
            await context.SaveChangesAsync();
        }

        public async Task eliminarProyecto(Proyecto proyecto)
        {
            context.Proyectos.Remove(proyecto);
            await context.SaveChangesAsync();
        
        }

        public async Task<ProyectosUsuario> ObtenerProyectosUsuario(int id)
        {
            var proyecto_usuario = await context.ProyectosUsuarios
                .Include(x => x.IdProyectoNavigation)
                    .ThenInclude(t => t.Tareas)
                        .ThenInclude(n => n.Nota)
                .FirstOrDefaultAsync(x => x.IdProyecto == id);

            return proyecto_usuario;
        } 


        public async Task eliminarProyectoUsuario(ProyectosUsuario proyectosUsuario)
        {
         
            foreach (var tarea in proyectosUsuario.IdProyectoNavigation.Tareas)
            {
                context.Notas.RemoveRange(tarea.Nota);
            }

            context.Tareas.RemoveRange(proyectosUsuario.IdProyectoNavigation.Tareas);
            context.Proyectos.Remove(proyectosUsuario.IdProyectoNavigation);
            context.ProyectosUsuarios.Remove(proyectosUsuario);


            await context.SaveChangesAsync();


        }


    }
}
