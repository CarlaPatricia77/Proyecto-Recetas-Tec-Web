using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Recetas.Core.Entities;
using Recetas.Core.Interfaces;
using Recetas.Infrastructure.Data;
using System;


namespace Recetas.Infrastructure.Repositories
{
    public class CategoriaRepository : ICategoriasRepository
    {
        private readonly RecetasContext _ctx;
        public CategoriaRepository(RecetasContext ctx) => _ctx = ctx;

        public async Task<IEnumerable<Categoria>> GetAllAsync() =>
            await _ctx.Categorias
                .Include(c => c.Recetas)
                .ToListAsync();

        public async Task<Categoria?> GetByIdAsync(int id) =>
            await _ctx.Categorias
                .Include(c => c.Recetas)
                .FirstOrDefaultAsync(c => c.Id == id);

        public async Task InsertAsync(Categoria categoria)
        {
            _ctx.Categorias.Add(categoria);
            await _ctx.SaveChangesAsync();
        }

        public async Task UpdateAsync(Categoria categoria)
        {
            _ctx.Categorias.Update(categoria);
            await _ctx.SaveChangesAsync();
        }

        public async Task DeleteAsync(Categoria categoria)
        {
            _ctx.Categorias.Remove(categoria);
            await _ctx.SaveChangesAsync();
        }

        public async Task<Categoria?> GetByNombreAsync(string nombre) =>
            await _ctx.Categorias
                .Include(c => c.Recetas)
                .FirstOrDefaultAsync(c => c.nombre == nombre);
        public async Task<IEnumerable<Categoria>> GetCategoriasConMasDeNRecetasAsync(int cantidad) =>
    await _ctx.Categorias
        .Include(c => c.Recetas)
        .Where(c => c.Recetas.Count > cantidad)
        .ToListAsync();

        public async Task<IEnumerable<Categoria>> BuscarCategoriasPorDescripcionAsync(string texto) =>
     await _ctx.Categorias
         .Include(c => c.Recetas)
         .Where(c => c.descripcion != null && c.descripcion.Contains(texto))
         .ToListAsync();
        //nuevo metodo
        public async Task<IEnumerable<Categoria>> GetCategoriasPorRangoFechasAsync(
    DateTime fechaInicio,
    DateTime fechaFin)
        {
            return await _ctx.Categorias
                .Include(c => c.Usuario)
                .Where(c => c.fecha_creacion >= fechaInicio &&
                            c.fecha_creacion <= fechaFin)
                .ToListAsync();
        }
        public async Task<IEnumerable<Categoria>> GetCategoriasPorFecha(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _ctx.Categorias
                .Include(c => c.Usuario)
                .Where(c => c.fecha_creacion >= fechaInicio && c.fecha_creacion <= fechaFin)
                .ToListAsync();
        }


    }
}
