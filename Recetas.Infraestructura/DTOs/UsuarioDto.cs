using System;

namespace Recetas.Infrastructure.DTOs;

public class UsuarioDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string Correo { get; set; } = null!;
    public DateTime FechaCreacion { get; set; }
    public bool IsActive { get; set; }

    // Propiedad opcional de resumen
    public int RecetasCount { get; set; }
}