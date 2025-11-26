using APINet.DTOs;
using FluentValidation;

namespace APINet.Validators
{
    /// <summary>
    /// Validador para CreateLibroDto
    /// </summary>
    public class CreateLibroValidator : AbstractValidator<CreateLibroDto>
    {
        public CreateLibroValidator()
        {
            RuleFor(x => x.Titulo)
                .NotEmpty().WithMessage("El título del libro es obligatorio.")
                .MaximumLength(200).WithMessage("El título no puede exceder 200 caracteres.");

            RuleFor(x => x.Autor)
                .NotEmpty().WithMessage("El autor del libro es obligatorio.")
                .MaximumLength(100).WithMessage("El autor no puede exceder 100 caracteres.");

            RuleFor(x => x.Anio)
                .InclusiveBetween(1500, DateTime.Now.Year)
                .WithMessage($"El año debe estar entre 1500 y {DateTime.Now.Year}.");

            RuleFor(x => x.Genero)
                .MaximumLength(50).WithMessage("El género no puede exceder 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Genero));
        }
    }
}
