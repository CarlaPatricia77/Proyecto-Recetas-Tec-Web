namespace Recetas.Infrastructure.DTOs;

public class CategoriaDto
{
    public int id { get; set; }
    public string nombre { get; set; } = null!;
    public string? descripcion { get; set; }
    public DateTime fecha_creacion { get; set; }
    public bool IsActive { get; set; }

    // Propiedad opcional de resumen
    public int RecetasCount { get; set; }
}