using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recetas.Core.Entities
{
    public partial class Receta : BaseEntity
    {
        // Remover: public int Id { get; set; } (ahora heredado)
        public int? usuarioId { get; set; }
        public int? categoria_id { get; set; }
        public string titulo { get; set; } = null!;
        public string? descripcion { get; set; }
        public string ingredientes { get; set; } = null!;
        public int tiempo_preparacion { get; set; }
        public DateTime fecha_creacion { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        // Relaciones
        public Usuario? Usuario { get; set; }
        public Categoria? Categoria { get; set; }
    }
}
