using DotTaskAPI.Entidades;
using Microsoft.EntityFrameworkCore;

namespace DotTaskAPI.Servicios
{
    public interface IRepositorioToken
    {
        Task actualizar(Token token);
        Task guardarToken(Token token);
        Task<Token> obtenerToken(string token);
    }


    public class RepositorioToken: IRepositorioToken
    {
        private readonly ApplicationDbContext context;

        public RepositorioToken(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task actualizar(Token token)
        {
            context.Tokens.Update(token);
            await context.SaveChangesAsync();
        }

        public async Task<Token> obtenerToken(string token)
        {
            var entidad = await context.Tokens.FirstOrDefaultAsync(x => x.Token1 == token);

            return entidad;
        }

        public async Task guardarToken(Token token)
        {
            context.Tokens.Add(token);
            await context.SaveChangesAsync();
        }





    }
}
