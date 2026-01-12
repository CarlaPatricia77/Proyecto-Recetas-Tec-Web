using FluentValidation;
using Recetas.Infrastructure.DTOs;

namespace Recetas.Infrastructure.Validators
{
    public class RecetaValidator : AbstractValidator<RecetaDto>
    {
        public RecetaValidator()
        {
            RuleFor(x => x.Titulo)
                .NotEmpty().WithMessage("El título es requerido")
                .MaximumLength(100).WithMessage("El título no puede exceder 100 caracteres")
                .MinimumLength(5).WithMessage("El título debe tener al menos 5 caracteres");

            RuleFor(x => x.Descripcion)
                .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres");

            RuleFor(x => x.Ingredientes)
                .NotEmpty().WithMessage("Los ingredientes son requeridos")
                .MinimumLength(10).WithMessage("Debe proporcionar más detalle en los ingredientes");

            RuleFor(x => x.TiempoPreparacion)
                .GreaterThan(0).WithMessage("El tiempo de preparación debe ser mayor a 0")
                .LessThanOrEqualTo(1440).WithMessage("El tiempo de preparación no puede exceder 24 horas (1440 minutos)");

            RuleFor(x => x.UsuarioId)
                .GreaterThan(0).WithMessage("Dbe especficar un usuario válido")
                .When(x => x.UsuarioId.HasValue);

            RuleFor(x => x.CategoriaId)
                .GreaterThan(0).WithMessage("Debe especificar una categoría válida")
                .When(x => x.CategoriaId.HasValue);
        }
    }
}