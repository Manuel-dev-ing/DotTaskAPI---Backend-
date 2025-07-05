using DotTaskAPI.DTOs;
using DotTaskAPI.Entidades;
using Microsoft.EntityFrameworkCore;

namespace DotTaskAPI.Servicios
{
    public interface IRepositorioNotas
    {
        Task eliminar(Nota nota);
        Task guardar(Nota nota);
        Task guardarCambios();
        Task<Nota> obtenerNotaPorId(int id);
        Task<Nota> obtenerNotaPorIdTarea(int id);
        Task<List<NotaDTO>> obtenerNotasTareas(int id);
    }
    public class RepositorioNotas: IRepositorioNotas
    {
        private readonly ApplicationDbContext context;

        public RepositorioNotas(ApplicationDbContext context)
        {
            this.context = context;
        }

        //obtener notas que pertenecen a una tarea
        public async Task<List<NotaDTO>> obtenerNotasTareas(int id)
        {
            var notaDTO = await context.Notas
                .Where(x => x.IdTarea == id)
                .Select(x => new NotaDTO()
                {
                    Id = x.Id,
                    IdTarea = (int)x.IdTarea,
                    Contenido = x.Contenido,
                    CreadoPor = (int)x.CreadoPor
                }).ToListAsync();

            return notaDTO;
        }


        public async Task<Nota> obtenerNotaPorId(int id)
        {
            var nota = await context.Notas.FirstOrDefaultAsync(x => x.Id == id);

            return nota;
        }

        public async Task<Nota> obtenerNotaPorIdTarea(int id)
        {
            var nota = await context.Notas
                .FirstOrDefaultAsync(x => x.IdTarea == id);

            return nota;
        }

        public async Task guardar(Nota nota)
        {
            context.Notas.Add(nota);
            await context.SaveChangesAsync();
        }

        public async Task eliminar(Nota nota)
        {
            context.Notas.Remove(nota);
        }

        public async Task guardarCambios()
        {
            await context.SaveChangesAsync();
        }




    }
}
