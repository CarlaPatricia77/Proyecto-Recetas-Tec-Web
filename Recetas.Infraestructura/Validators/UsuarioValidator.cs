using FluentValidation;
using Recetas.Infrastructure.DTOs;

namespace Recetas.Infrastructure.Validators
{
    public class UsuarioValidator : AbstractValidator<UsuarioDto>
    {
        public UsuarioValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre es requerido")
                .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres")
                .MinimumLength(2).WithMessage("El nombre debe tener al menos 2 caracteres");

            RuleFor(x => x.Correo)
                .NotEmpty().WithMessage("El correo es requerido")
                .EmailAddress().WithMessage("Debe proporcionar un correo válido")
                .MaximumLength(100).WithMessage("El correo no puede exceder 100 caracteres");
        }
    }
}