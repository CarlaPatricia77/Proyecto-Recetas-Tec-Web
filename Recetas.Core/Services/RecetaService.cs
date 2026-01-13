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
                recetas = recetas.Where(r => r.usuarioId == filters.UsuarioId.Value);
            }

            if (filters.categoria_id.HasValue)
            {
                recetas = recetas.Where(r => r.categoria_id == filters.categoria_id.Value);
            }

            if (!string.IsNullOrWhiteSpace(filters.Busqueda))
            {
                recetas = recetas.Where(r =>
                    r.titulo.Contains(filters.Busqueda, StringComparison.OrdinalIgnoreCase) ||
                    (r.descripcion != null && r.descripcion.Contains(filters.Busqueda, StringComparison.OrdinalIgnoreCase))
                );
            }

            if (filters.TiempoMaximo.HasValue)
            {
                recetas = recetas.Where(r => r.tiempo_preparacion <= filters.TiempoMaximo.Value);
            }

            if (!string.IsNullOrWhiteSpace(filters.Ingrediente))
            {
                recetas = recetas.Where(r =>
                    r.ingredientes.Contains(filters.Ingrediente, StringComparison.OrdinalIgnoreCase)
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
            if (receta.usuarioId.HasValue)
            {
                var usuario = await _unitOfWork.UsuarioRepository.GetById(receta.usuarioId.Value);
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
            if (receta.categoria_id.HasValue)
            {
                var categoria = await _unitOfWork.CategoriaRepository.GetById(receta.categoria_id.Value);
                if (categoria == null)
                {
                    throw new BusinessException("La categoría no existe", 400);
                }
            }

            // REGLA DE NEGOCIO 3: No permitir contenido inapropiado
            if (ContainsForbiddenContent(receta.titulo) ||
                (receta.descripcion != null && ContainsForbiddenContent(receta.descripcion)))
            {
                throw new BusinessException("El contenido contiene palabras no permitidas", 400);
            }

            // REGLA DE NEGOCIO 4: Tiempo de preparación debe ser positivo
            if (receta.tiempo_preparacion <= 0)
            {
                throw new BusinessException("El tiempo de preparación debe ser mayor a 0", 400);
            }

            // Asignar la fecha de creación si no está seteada
            if (receta.fecha_creacion == default)
                receta.fecha_creacion = DateTime.UtcNow;

            await _unitOfWork.RecetaRepository.InsertAsync(receta);
            await _unitOfWork.SaveChangesAsync();
        }


        public async Task UpdateReceta(Receta receta)
        {
            // Validar contenido inapropiado
            if (ContainsForbiddenContent(receta.titulo) ||
         (receta.descripcion != null && ContainsForbiddenContent(receta.descripcion)))
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

        public async Task<IEnumerable<Receta>> GetRecetasUltimoMesAsync()
        {
            var desde = DateTime.UtcNow.AddMonths(-1);
            var hasta = DateTime.UtcNow;

            // Convert 'hasta' to the expected type 'System.Runtime.InteropServices.JavaScript.JSType.Date'
            var hastaJsDate = (System.Runtime.InteropServices.JavaScript.JSType.Date)(object)hasta;

            var recetas = await _unitOfWork.RecetaRepository.GetRecetasPorRangoFechasAsync(desde, hasta);

            // Convert IEnumerable<object> to IEnumerable<Receta>
            return recetas.Cast<Receta>();
        }


    }
}