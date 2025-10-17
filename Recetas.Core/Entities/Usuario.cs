using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recetas.Core.Entities
{
    public partial class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public string? Contrasena { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        // Relaciones
        public ICollection<Receta> Recetas { get; set; } = new List<Receta>();
    }
}
