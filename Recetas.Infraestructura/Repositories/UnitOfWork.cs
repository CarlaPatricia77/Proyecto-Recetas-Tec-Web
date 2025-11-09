using Recetas.Core.Entities;
using Recetas.Core.Interfaces;
using Recetas.Infrastructure.Data;

namespace Recetas.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación del patrón Unit of Work
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly RecetasContext _context;
        private readonly IRecetasRepository? _recetaRepository;
        private readonly IBaseRepository<Usuario>? _usuarioRepository;
        private readonly IBaseRepository<Categoria>? _categoriaRepository;

        public UnitOfWork(RecetasContext context)
        {
            _context = context;
        }

        public IRecetasRepository RecetaRepository =>
            _recetaRepository ?? new RecetaRepository(_context);

        public IBaseRepository<Usuario> UsuarioRepository =>
            _usuarioRepository ?? new BaseRepository<Usuario>(_context);

        public IBaseRepository<Categoria> CategoriaRepository =>
            _categoriaRepository ?? new BaseRepository<Categoria>(_context);

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            var affectedRows = await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            if (_context != null)
            {
                _context.Dispose();
            }
        }
    }
}
