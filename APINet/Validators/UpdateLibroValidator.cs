using APINet.DTOs;
using FluentValidation;

namespace APINet.Validators
{
    /// <summary>
    /// Validador para UpdateLibroDto
    /// </summary>
    public class UpdateLibroValidator : AbstractValidator<UpdateLibroDto>
    {
        public UpdateLibroValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("El Id debe ser mayor a cero.");

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
