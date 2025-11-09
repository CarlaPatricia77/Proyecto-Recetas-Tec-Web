using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recetas.Core.QueryFilters
{
    /// <summary>
    /// Clase base abstracta para paginación
    /// </summary>
    public abstract class PaginationQueryFilter
    {
        /// <summary>
        /// Cantidad de registros por página
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Número de página a mostrar
        /// </summary>
        public int PageNumber { get; set; } = 1;
    }
}
