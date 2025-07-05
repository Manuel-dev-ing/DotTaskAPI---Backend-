using DotTaskAPI.Entidades;

namespace DotTaskAPI.Servicios
{
    public interface IRepositorioHistorialCambios
    {
        Task guardar(HistorialCambiosTarea historial);
    }
    public class RepositorioHistorialCambios: IRepositorioHistorialCambios
    {
        private readonly ApplicationDbContext context;

        public RepositorioHistorialCambios(ApplicationDbContext context)
        {
            this.context = context;
        }


        public async Task guardar(HistorialCambiosTarea historial)
        {
            context.HistorialCambiosTareas.Add(historial);
        }




    }
}
