namespace Recetas.Infrastructure.DTOs;

public class RecetaDto
{
    public int Id { get; set; }
    public int? UsuarioId { get; set; }
    public int? CategoriaId { get; set; }

    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string Ingredientes { get; set; } = null!;
    public int TiempoPreparacion { get; set; }
    public DateTime FechaCreacion { get; set; }

    // Datos opcionales calculados o derivados
    public string? NombreUsuario { get; set; }
    public string? NombreCategoria { get; set; }
}