using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recetas.Core.Entities
{
    public partial class Receta
    {
        public int Id { get; set; }
        public int? UsuarioId { get; set; }
        public int? CategoriaId { get; set; }

        public string Titulo { get; set; } = null!;
        public string? Descripcion { get; set; }
        public string Ingredientes { get; set; } = null!;
        public int TiempoPreparacion { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        // Relaciones
        public Usuario? Usuario { get; set; }
        public Categoria? Categoria { get; set; }
    }
}
