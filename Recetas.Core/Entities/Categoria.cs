using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recetas.Core.Entities
{
    public partial class Categoria : BaseEntity
    {
        // Remover: public int Id { get; set; } (ahora heredado)
        public string nombre { get; set; } = null!;
        public string? descripcion { get; set; }
        public DateTime fecha_creacion { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        // Relaciones
        public ICollection<Receta> Recetas { get; set; } = new List<Receta>();
    }
}
