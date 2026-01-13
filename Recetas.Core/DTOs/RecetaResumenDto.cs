namespace Recetas.Core.DTOs;

public class RecetaResumenDto
{
    public string NombreReceta { get; set; } = null!;
    public string? Categoria { get; set; }
    public string? Usuario { get; set; }
}
