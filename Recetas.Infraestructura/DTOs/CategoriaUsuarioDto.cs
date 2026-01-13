namespace Recetas.Infrastructure.DTOs
{
    public class CategoriaUsuarioDto
    {
        public int categoria_id { get; set; }
        public string CategoriaNombre { get; set; } = null!;

        public int UsuarioId { get; set; }
        public string UsuarioNombre { get; set; } = null!;

        public DateTime fecha_creacion { get; set; }
    }
}
