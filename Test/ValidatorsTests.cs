using APINet.DTOs;
using APINet.Validators;
using FluentValidation.TestHelper;

namespace Tests
{
    [TestFixture]
    public class CreateLibroValidatorTests
    {
        private CreateLibroValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new CreateLibroValidator();
        }

        [Test]
        public void Validate_DeberiaSerValido_ConDatosCorrectos()
        {
            // Arrange
            var dto = new CreateLibroDto
            {
                Titulo = "Clean Code",
                Autor = "Robert Martin",
                Anio = 2008,
                Genero = "Programación"
            };

            // Act
            var resultado = _validator.TestValidate(dto);

            // Assert
            resultado.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void Validate_DeberiaTenerError_CuandoTituloEstaVacio()
        {
            // Arrange
            var dto = new CreateLibroDto
            {
                Titulo = "",
                Autor = "Autor",
                Anio = 2000,
                Genero = "Drama"
            };

            // Act
            var resultado = _validator.TestValidate(dto);

            // Assert
            resultado.ShouldHaveValidationErrorFor(x => x.Titulo)
                .WithErrorMessage("El título del libro es obligatorio.");
        }

        [Test]
        public void Validate_DeberiaTenerError_CuandoTituloEsMuyLargo()
        {
            // Arrange
            var dto = new CreateLibroDto
            {
                Titulo = new string('A', 201), // 201 caracteres
                Autor = "Autor",
                Anio = 2000,
                Genero = "Drama"
            };

            // Act
            var resultado = _validator.TestValidate(dto);

            // Assert
            resultado.ShouldHaveValidationErrorFor(x => x.Titulo)
                .WithErrorMessage("El título no puede exceder 200 caracteres.");
        }

        [Test]
        public void Validate_DeberiaTenerError_CuandoAutorEstaVacio()
        {
            // Arrange
            var dto = new CreateLibroDto
            {
                Titulo = "Titulo",
                Autor = "",
                Anio = 2000,
                Genero = "Drama"
            };

            // Act
            var resultado = _validator.TestValidate(dto);

            // Assert
            resultado.ShouldHaveValidationErrorFor(x => x.Autor)
                .WithErrorMessage("El autor del libro es obligatorio.");
        }

        [Test]
        public void Validate_DeberiaTenerError_CuandoAutorEsMuyLargo()
        {
            // Arrange
            var dto = new CreateLibroDto
            {
                Titulo = "Titulo",
                Autor = new string('A', 101), // 101 caracteres
                Anio = 2000,
                Genero = "Drama"
            };

            // Act
            var resultado = _validator.TestValidate(dto);

            // Assert
            resultado.ShouldHaveValidationErrorFor(x => x.Autor)
                .WithErrorMessage("El autor no puede exceder 100 caracteres.");
        }

        [Test]
        public void Validate_DeberiaTenerError_CuandoAnioEsMenorA1500()
        {
            // Arrange
            var dto = new CreateLibroDto
            {
                Titulo = "Titulo",
                Autor = "Autor",
                Anio = 1499,
                Genero = "Drama"
            };

            // Act
            var resultado = _validator.TestValidate(dto);

            // Assert
            resultado.ShouldHaveValidationErrorFor(x => x.Anio);
        }

        [Test]
        public void Validate_DeberiaTenerError_CuandoAnioEsMayorAlActual()
        {
            // Arrange
            var dto = new CreateLibroDto
            {
                Titulo = "Titulo",
                Autor = "Autor",
                Anio = DateTime.Now.Year + 1,
                Genero = "Drama"
            };

            // Act
            var resultado = _validator.TestValidate(dto);

            // Assert
            resultado.ShouldHaveValidationErrorFor(x => x.Anio);
        }

        [Test]
        public void Validate_DeberiaTenerError_CuandoGeneroEsMuyLargo()
        {
            // Arrange
            var dto = new CreateLibroDto
            {
                Titulo = "Titulo",
                Autor = "Autor",
                Anio = 2000,
                Genero = new string('A', 51) // 51 caracteres
            };

            // Act
            var resultado = _validator.TestValidate(dto);

            // Assert
            resultado.ShouldHaveValidationErrorFor(x => x.Genero)
                .WithErrorMessage("El género no puede exceder 50 caracteres.");
        }

        [Test]
        public void Validate_DeberiaSerValido_CuandoGeneroEsNulo()
        {
            // Arrange
            var dto = new CreateLibroDto
            {
                Titulo = "Titulo",
                Autor = "Autor",
                Anio = 2000,
                Genero = null
            };

            // Act
            var resultado = _validator.TestValidate(dto);

            // Assert
            resultado.ShouldNotHaveValidationErrorFor(x => x.Genero);
        }
    }

    [TestFixture]
    public class UpdateLibroValidatorTests
    {
        private UpdateLibroValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new UpdateLibroValidator();
        }

        [Test]
        public void Validate_DeberiaSerValido_ConDatosCorrectos()
        {
            // Arrange
            var dto = new UpdateLibroDto
            {
                Id = 1,
                Titulo = "Clean Code",
                Autor = "Robert Martin",
                Anio = 2008,
                Genero = "Programación"
            };

            // Act
            var resultado = _validator.TestValidate(dto);

            // Assert
            resultado.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void Validate_DeberiaTenerError_CuandoIdEsCero()
        {
            // Arrange
            var dto = new UpdateLibroDto
            {
                Id = 0,
                Titulo = "Titulo",
                Autor = "Autor",
                Anio = 2000,
                Genero = "Drama"
            };

            // Act
            var resultado = _validator.TestValidate(dto);

            // Assert
            resultado.ShouldHaveValidationErrorFor(x => x.Id)
                .WithErrorMessage("El Id debe ser mayor a cero.");
        }

        [Test]
        public void Validate_DeberiaTenerError_CuandoIdEsNegativo()
        {
            // Arrange
            var dto = new UpdateLibroDto
            {
                Id = -1,
                Titulo = "Titulo",
                Autor = "Autor",
                Anio = 2000,
                Genero = "Drama"
            };

            // Act
            var resultado = _validator.TestValidate(dto);

            // Assert
            resultado.ShouldHaveValidationErrorFor(x => x.Id)
                .WithErrorMessage("El Id debe ser mayor a cero.");
        }

        [Test]
        public void Validate_DeberiaTenerError_CuandoTituloEstaVacio()
        {
            // Arrange
            var dto = new UpdateLibroDto
            {
                Id = 1,
                Titulo = "",
                Autor = "Autor",
                Anio = 2000,
                Genero = "Drama"
            };

            // Act
            var resultado = _validator.TestValidate(dto);

            // Assert
            resultado.ShouldHaveValidationErrorFor(x => x.Titulo)
                .WithErrorMessage("El título del libro es obligatorio.");
        }

        [Test]
        public void Validate_DeberiaTenerError_CuandoTituloEsMuyLargo()
        {
            // Arrange
            var dto = new UpdateLibroDto
            {
                Id = 1,
                Titulo = new string('A', 201),
                Autor = "Autor",
                Anio = 2000,
                Genero = "Drama"
            };

            // Act
            var resultado = _validator.TestValidate(dto);

            // Assert
            resultado.ShouldHaveValidationErrorFor(x => x.Titulo)
                .WithErrorMessage("El título no puede exceder 200 caracteres.");
        }

        [Test]
        public void Validate_DeberiaTenerError_CuandoAutorEstaVacio()
        {
            // Arrange
            var dto = new UpdateLibroDto
            {
                Id = 1,
                Titulo = "Titulo",
                Autor = "",
                Anio = 2000,
                Genero = "Drama"
            };

            // Act
            var resultado = _validator.TestValidate(dto);

            // Assert
            resultado.ShouldHaveValidationErrorFor(x => x.Autor)
                .WithErrorMessage("El autor del libro es obligatorio.");
        }

        [Test]
        public void Validate_DeberiaTenerError_CuandoAutorEsMuyLargo()
        {
            // Arrange
            var dto = new UpdateLibroDto
            {
                Id = 1,
                Titulo = "Titulo",
                Autor = new string('A', 101),
                Anio = 2000,
                Genero = "Drama"
            };

            // Act
            var resultado = _validator.TestValidate(dto);

            // Assert
            resultado.ShouldHaveValidationErrorFor(x => x.Autor)
                .WithErrorMessage("El autor no puede exceder 100 caracteres.");
        }

        [Test]
        public void Validate_DeberiaTenerError_CuandoAnioEsMenorA1500()
        {
            // Arrange
            var dto = new UpdateLibroDto
            {
                Id = 1,
                Titulo = "Titulo",
                Autor = "Autor",
                Anio = 1499,
                Genero = "Drama"
            };

            // Act
            var resultado = _validator.TestValidate(dto);

            // Assert
            resultado.ShouldHaveValidationErrorFor(x => x.Anio);
        }

        [Test]
        public void Validate_DeberiaTenerError_CuandoAnioEsMayorAlActual()
        {
            // Arrange
            var dto = new UpdateLibroDto
            {
                Id = 1,
                Titulo = "Titulo",
                Autor = "Autor",
                Anio = DateTime.Now.Year + 1,
                Genero = "Drama"
            };

            // Act
            var resultado = _validator.TestValidate(dto);

            // Assert
            resultado.ShouldHaveValidationErrorFor(x => x.Anio);
        }

        [Test]
        public void Validate_DeberiaTenerError_CuandoGeneroEsMuyLargo()
        {
            // Arrange
            var dto = new UpdateLibroDto
            {
                Id = 1,
                Titulo = "Titulo",
                Autor = "Autor",
                Anio = 2000,
                Genero = new string('A', 51)
            };

            // Act
            var resultado = _validator.TestValidate(dto);

            // Assert
            resultado.ShouldHaveValidationErrorFor(x => x.Genero)
                .WithErrorMessage("El género no puede exceder 50 caracteres.");
        }

        [Test]
        public void Validate_DeberiaSerValido_CuandoGeneroEsNulo()
        {
            // Arrange
            var dto = new UpdateLibroDto
            {
                Id = 1,
                Titulo = "Titulo",
                Autor = "Autor",
                Anio = 2000,
                Genero = null
            };

            // Act
            var resultado = _validator.TestValidate(dto);

            // Assert
            resultado.ShouldNotHaveValidationErrorFor(x => x.Genero);
        }
    }
}
