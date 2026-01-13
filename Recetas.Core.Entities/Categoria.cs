using System;
using System.Collections.Generic;

namespace Recetas.Core.Entities
{
    public partial class Categoria : BaseEntity
    {
        public string nombre { get; set; } = null!;
        public string? descripcion { get; set; }
        public DateTime fecha_creacion { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        // Usuario creador — ahora opcional
        public int? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        // Relaciones
        public ICollection<Receta> Recetas { get; set; } = new List<Receta>();
    }
}