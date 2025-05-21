using DotTaskAPI.DTOs;
using DotTaskAPI.Entidades;
using Microsoft.EntityFrameworkCore;

namespace DotTaskAPI.Servicios
{
    public interface IRepositorioTareas
    {
        Task actualizarTarea(Tarea tarea);
        Task<int> eliminarTarea(int id);
        Task<bool> existeProyecto(int IdProyecto);
        Task<bool> existeTarea(int id);
        Task guardarTarea(Tarea tarea);
        Task<List<TareaDTO>> obtienerTareas();
        Task<TareaDTO> obtieneTareaPorId(int id);
    }

    public class RepositorioTareas: IRepositorioTareas
    {
        private readonly ApplicationDbContext context;

        public RepositorioTareas(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<TareaDTO> obtieneTareaPorId(int id)
        {
            var tareaDTO = await context.Tareas
                .Select(x => new TareaDTO()
                {
                    Id = x.Id,
                    IdProyecto = (int)x.IdProyecto,
                    Nombre = x.Nombre,
                    Descripcion = x.Descripcion,
                    Estado = x.Estado,
                    IdProyectoNavigation = x.IdProyectoNavigation

                }).FirstOrDefaultAsync(x => x.Id == id);

            return tareaDTO;
        }

        public async Task<bool> existeTarea(int id)
        {
            var entidad = await context.Tareas.AnyAsync(x => x.Id == id);

            return entidad;
        }

        public async Task<bool> existeProyecto(int IdProyecto)
        {

            var entidad = await context.Proyectos.AnyAsync(x => x.Id == IdProyecto);

            return entidad;
        }

        public async Task<List<TareaDTO>> obtienerTareas()
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

        public async Task guardarTarea(Tarea tarea)
        {
            context.Tareas.Add(tarea);
            await context.SaveChangesAsync();
        }

        public async Task actualizarTarea(Tarea tarea)
        {
            context.Tareas.Update(tarea);
            await context.SaveChangesAsync();
        }

        public async Task<int> eliminarTarea(int id)
        {

            var resultado = await context.Tareas.Where(x => x.Id == id).ExecuteDeleteAsync();

            return resultado;
        }



    }
}
