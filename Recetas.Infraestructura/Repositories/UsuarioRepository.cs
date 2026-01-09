using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Recetas.Core.Entities;
using Recetas.Core.Interfaces;
using Recetas.Infrastructure.Data;

namespace Recetas.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly RecetasContext _ctx;
        public UsuarioRepository(RecetasContext ctx) => _ctx = ctx;

        public async Task<IEnumerable<Usuario>> GetAllAsync() =>
            await _ctx.Usuarios
                .Include(u => u.Recetas)
                .ToListAsync();

        public async Task<Usuario?> GetByIdAsync(int id) =>
            await _ctx.Usuarios
                .Include(u => u.Recetas)
                .FirstOrDefaultAsync(u => u.Id == id);

        public async Task InsertAsync(Usuario usuario)
        {
            _ctx.Usuarios.Add(usuario);
            await _ctx.SaveChangesAsync();
        }

        public async Task UpdateAsync(Usuario usuario)
        {
            _ctx.Usuarios.Update(usuario);
            await _ctx.SaveChangesAsync();
        }

        public async Task DeleteAsync(Usuario usuario)
        {
            _ctx.Usuarios.Remove(usuario);
            await _ctx.SaveChangesAsync();
        }

        public async Task<Usuario?> GetByCorreoAsync(string correo) =>
            await _ctx.Usuarios
                .Include(u => u.Recetas)
                .FirstOrDefaultAsync(u => u.correo == correo);

        public async Task<IEnumerable<Usuario>> GetActiveUsersAsync() =>
            await _ctx.Usuarios
                .Include(u => u.Recetas)
                .Where(u => u.IsActive)
                .ToListAsync();
    }
}
