using Recetas.Core.Entities;
using Recetas.Core.Exceptions;
using Recetas.Core.Interfaces;
using Recetas.Core.QueryFilters;

namespace Recetas.Core.Services
{
    /// <summary>
    /// Implementación del servicio de lógica de negocio para Recetas
    /// </summary>
    public class RecetaService : IRecetaService
    {
        private readonly IUnitOfWork _unitOfWork;

        // Palabras no permitidas en títulos/descripciones
        private readonly string[] ForbiddenWords = { "spam", "fraude", "engaño" };

        public RecetaService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Receta> GetAllRecetas(RecetaQueryFilter filters)
        {
            var recetas = _unitOfWork.RecetaRepository.GetAllAsync().Result;

            // Aplicar filtros
            if (filters.UsuarioId.HasValue)
            {
                recetas = recetas.Where(r => r.UsuarioId == filters.UsuarioId.Value);
            }

            if (filters.CategoriaId.HasValue)
            {
                recetas = recetas.Where(r => r.CategoriaId == filters.CategoriaId.Value);
            }

            if (!string.IsNullOrWhiteSpace(filters.Busqueda))
            {
                recetas = recetas.Where(r =>
                    r.Titulo.Contains(filters.Busqueda, StringComparison.OrdinalIgnoreCase) ||
                    (r.Descripcion != null && r.Descripcion.Contains(filters.Busqueda, StringComparison.OrdinalIgnoreCase))
                );
            }

            if (filters.TiempoMaximo.HasValue)
            {
                recetas = recetas.Where(r => r.TiempoPreparacion <= filters.TiempoMaximo.Value);
            }

            if (!string.IsNullOrWhiteSpace(filters.Ingrediente))
            {
                recetas = recetas.Where(r =>
                    r.Ingredientes.Contains(filters.Ingrediente, StringComparison.OrdinalIgnoreCase)
                );
            }

            return recetas;
        }

        public async Task<Receta> GetRecetaById(int id)
        {
            return await _unitOfWork.RecetaRepository.GetByIdAsync(id);
        }

        public async Task InsertReceta(Receta receta)
        {
            // REGLA DE NEGOCIO 1: Verificar que el usuario exista
            if (receta.UsuarioId.HasValue)
            {
                var usuario = await _unitOfWork.UsuarioRepository.GetById(receta.UsuarioId.Value);
                if (usuario == null)
                {
                    throw new BusinessException("El usuario no existe", 400);
                }

                // Solo usuarios activos pueden crear recetas
                if (!usuario.IsActive)
                {
                    throw new BusinessException("El usuario no está activo", 403);
                }
            }

            // REGLA DE NEGOCIO 2: Verificar que la categoría exista
            if (receta.CategoriaId.HasValue)
            {
                var categoria = await _unitOfWork.CategoriaRepository.GetById(receta.CategoriaId.Value);
                if (categoria == null)
                {
                    throw new BusinessException("La categoría no existe", 400);
                }
            }

            // REGLA DE NEGOCIO 3: No permitir contenido inapropiado
            if (ContainsForbiddenContent(receta.Titulo) ||
                (receta.Descripcion != null && ContainsForbiddenContent(receta.Descripcion)))
            {
                throw new BusinessException("El contenido contiene palabras no permitidas", 400);
            }

            // REGLA DE NEGOCIO 4: Tiempo de preparación debe ser positivo
            if (receta.TiempoPreparacion <= 0)
            {
                throw new BusinessException("El tiempo de preparación debe ser mayor a 0", 400);
            }

            await _unitOfWork.RecetaRepository.InsertAsync(receta);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateReceta(Receta receta)
        {
            // Validar contenido inapropiado
            if (ContainsForbiddenContent(receta.Titulo) ||
         (receta.Descripcion != null && ContainsForbiddenContent(receta.Descripcion)))
            {
                throw new BusinessException("El contenido contiene palabras no permitidas", 400);
            }

            await _unitOfWork.RecetaRepository.UpdateAsync(receta);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteReceta(int id)
        {
            var receta = await _unitOfWork.RecetaRepository.GetByIdAsync(id);
            if (receta == null)
            {
                throw new BusinessException("La receta no existe", 404);
            }
            await _unitOfWork.RecetaRepository.DeleteAsync(receta);
            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Verifica si el texto contiene palabras prohibidas
        /// </summary>
        private bool ContainsForbiddenContent(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;

            foreach (var word in ForbiddenWords)
            {
                if (text.Contains(word, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }
    }
}