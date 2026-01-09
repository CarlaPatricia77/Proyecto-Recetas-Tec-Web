using FluentValidation;
using Recetas.Infrastructure.DTOs;

namespace Recetas.Infrastructure.Validators
{
    public class CategoriaValidator : AbstractValidator<CategoriaDto>
    {
        public CategoriaValidator()
        {
            RuleFor(x => x.nombre)
                .NotEmpty().WithMessage("El nombre de la categoría es requerido")
                .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres")
                .MinimumLength(2).WithMessage("El nombre debe tener al menos 2 caracteres");

            RuleFor(x => x.descripcion)
                .MaximumLength(250).WithMessage("La descripción no puede exceder 250 caracteres");
        }
    }
}
