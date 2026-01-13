using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recetas.Core.QueryFilters
{
    /// <summary>
    /// Filtros de consulta específicos para Recetas
    /// </summary>
    public class RecetaQueryFilter : PaginationQueryFilter
    {
        /// <summary>
        /// Filtrar por ID de usuario
        /// </summary>
        public int? UsuarioId { get; set; }

        /// <summary>
        /// Filtrar por ID de categoría
        /// </summary>
        public int? categoria_id { get; set; }

        /// <summary>
        /// Buscar por título o descripción
        /// </summary>
        public string? Busqueda { get; set; }

        /// <summary>
        /// Filtrar por tiempo máximo de preparación (en minutos)
        /// </summary>
        public int? TiempoMaximo { get; set; }

        /// <summary>
        /// Filtrar por ingrediente específico
        /// </summary>
        public string? Ingrediente { get; set; }
    }
}
