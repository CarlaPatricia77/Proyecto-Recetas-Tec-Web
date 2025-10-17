namespace Recetas.Infrastructure.DTOs;

public class CategoriaDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public DateTime FechaCreacion { get; set; }
    public bool IsActive { get; set; }

    // Propiedad opcional de resumen
    public int RecetasCount { get; set; }
}