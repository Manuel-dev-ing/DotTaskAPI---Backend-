using DotTaskAPI.DTOs;
using DotTaskAPI.Entidades;
using Microsoft.EntityFrameworkCore;

namespace DotTaskAPI.Servicios
{
    public interface IRepositorioTareas
    {
        Task actualizarTarea(Tarea tarea);
        Task eliminarTarea(Tarea tarea);
        Task<bool> existeProyecto(int IdProyecto);
        Task<bool> existeTarea(int id);
        Task guardarCambios();
        Task guardarTarea(Tarea tarea);
        Task<Tarea> obtenerTarea(int id);
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

        public async Task<Tarea> obtenerTarea(int id)
        {
            var tarea = await context.Tareas
                .Include(x => x.Nota)
                .FirstOrDefaultAsync(x => x.Id == id);

            return tarea;
        }

        public async Task<TareaDTO> obtieneTareaPorId(int id)
        {
            var tareaDTO = await context.Tareas
                .Include(x => x.CompletadoPorNavigation)
                .Include(x => x.HistorialCambiosTareas)
                .Select(x => new TareaDTO()
                {
                    Id = x.Id,
                    IdProyecto = (int)x.IdProyecto,
                    Nombre = x.Nombre,
                    Descripcion = x.Descripcion,
                    Estado = x.Estado,
                    CompletadoPor = x.CompletadoPor,
                    IdUsuario = x.CompletadoPorNavigation.Id,
                    NombreUsuario = x.CompletadoPorNavigation.Nombre,
                    EmailUsuario = x.CompletadoPorNavigation.Email,
                    HistorialCambiosTareas = x.HistorialCambiosTareas,
                    Notas = x.Nota.Select(x => new NotaDTO()
                    {
                        Id = x.Id, 
                        IdTarea = (int)x.IdTarea, 
                        Contenido = x.Contenido,
                        CreadoPor = (int)x.CreadoPor,
                        NotaCreadoPor = new CreadoPorDTO()
                        {
                            Id = x.CreadoPorNavigation.Id,
                            Nombre = x.CreadoPorNavigation.Nombre,
                            Email = x.CreadoPorNavigation.Email
                        }
                    }).ToList()

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
        }

        public async Task guardarCambios()
        {
            await context.SaveChangesAsync();
        }



        public async Task eliminarTarea(Tarea tarea)
        {
            context.Tareas.Remove(tarea);
        }



    }
}
