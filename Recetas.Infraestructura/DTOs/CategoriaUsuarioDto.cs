namespace Recetas.Infrastructure.DTOs
{
    public class CategoriaUsuarioDto
    {
        public int CategoriaId { get; set; }
        public string CategoriaNombre { get; set; } = null!;

        public int UsuarioId { get; set; }
        public string UsuarioNombre { get; set; } = null!;

        public DateTime FechaCreacion { get; set; }
    }
}
