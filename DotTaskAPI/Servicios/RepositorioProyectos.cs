using DotTaskAPI.DTOs;
using DotTaskAPI.Entidades;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace DotTaskAPI.Servicios
{
    public interface IRepositorioProyectos
    {
        Task actualizarProyecto(Proyecto proyecto);
        Task<int> eliminarProyecto(int id);
        Task<ProyectoDTO> guardarProyecto(Proyecto proyecto);
        Task<ProyectoDTO> ObtenerProyectoPorId(int id);
        Task<IEnumerable<ProyectoDTO>> obtenerProyectos();
    }

    public class RepositorioProyectos : IRepositorioProyectos
    {
        private readonly ApplicationDbContext context;

        public RepositorioProyectos(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<ProyectoDTO>> obtenerProyectos()
        {
            var proyectos = await context.Proyectos
                .Include(x => x.Tareas)
                .Select(x => new ProyectoDTO()
                {
                    Id = x.Id,
                    NombreProyecto = x.NombreProyecto,
                    NombreCliente = x.NombreCliente,
                    Descripcion = x.Descripcion,
                    Tareas = x.Tareas.Select(x => new TareaDTO()
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

        private async Task<IEnumerable<TareaDTO>> obtenerTareaDTOs()
        {
            var tareasDTO = await context.Tareas
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

        public async Task<ProyectoDTO> ObtenerProyectoPorId(int id)
        {
            var proyectoDTO = await context.Proyectos
                .Include(x => x.Tareas)
                .Select(x => new ProyectoDTO()
                {
                    Id = x.Id,
                    NombreProyecto = x.NombreProyecto,
                    NombreCliente = x.NombreCliente,
                    Descripcion = x.Descripcion,
                    Tareas = x.Tareas.Select(x => new TareaDTO()
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
                Tareas = await obtenerTareaDTOs()
            };

            return proyectoDTO;
        }

        public async Task actualizarProyecto(Proyecto proyecto)
        {
            context.Proyectos.Update(proyecto);
            await context.SaveChangesAsync();
        }

        public async Task<int> eliminarProyecto(int id)
        {
            var resultado = await context.Proyectos.Where(x => x.Id == id).ExecuteDeleteAsync();

            return resultado;
        }


    }
}
