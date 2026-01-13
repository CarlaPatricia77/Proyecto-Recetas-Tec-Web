namespace Recetas.Infrastructure.DTOs;

public class RecetaDto
{
    public int Id { get; set; }
    public int? UsuarioId { get; set; }
    public int? categoria_id { get; set; }

    public string titulo { get; set; } = null!;
    public string? descripcion { get; set; }
    public string ingredientes { get; set; } = null!;
    public int tiempo_preparacion { get; set; }
    public DateTime fecha_creacion { get; set; }

    // Datos opcionales calculados o derivados
    public string? NombreUsuario { get; set; }
    public string? NombreCategoria { get; set; }

}