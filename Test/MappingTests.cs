using APINet.DTOs;
using APINet.Mappings;
using APINet.Models;
using AutoMapper;

namespace Tests
{
    [TestFixture]
    public class LibroMappingProfileTests
    {
        private IMapper _mapper;

        [SetUp]
        public void Setup()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<LibroMappingProfile>();
            });

            _mapper = configuration.CreateMapper();
        }

        [Test]
        public void Configuration_DeberiaPoderseConstruir()
        {
            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                var configuration = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<LibroMappingProfile>();
                });
                var mapper = configuration.CreateMapper();
                Assert.That(mapper, Is.Not.Null);
            });
        }

        [Test]
        public void Map_DeberiaMappearCreateLibroDtoALibro_Correctamente()
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
            var libro = _mapper.Map<Libro>(dto);

            // Assert
            Assert.That(libro, Is.Not.Null);
            Assert.That(libro.Titulo, Is.EqualTo(dto.Titulo));
            Assert.That(libro.Autor, Is.EqualTo(dto.Autor));
            Assert.That(libro.Anio, Is.EqualTo(dto.Anio));
            Assert.That(libro.Genero, Is.EqualTo(dto.Genero));
        }

        [Test]
        public void Map_DeberiaMappearUpdateLibroDtoALibro_Correctamente()
        {
            // Arrange
            var dto = new UpdateLibroDto
            {
                Id = 1,
                Titulo = "Clean Architecture",
                Autor = "Robert Martin",
                Anio = 2017,
                Genero = "Arquitectura"
            };

            // Act
            var libro = _mapper.Map<Libro>(dto);

            // Assert
            Assert.That(libro, Is.Not.Null);
            Assert.That(libro.Id, Is.EqualTo(dto.Id));
            Assert.That(libro.Titulo, Is.EqualTo(dto.Titulo));
            Assert.That(libro.Autor, Is.EqualTo(dto.Autor));
            Assert.That(libro.Anio, Is.EqualTo(dto.Anio));
            Assert.That(libro.Genero, Is.EqualTo(dto.Genero));
        }

        [Test]
        public void Map_DeberiaMappearLibroALibroResponseDto_Correctamente()
        {
            // Arrange
            var libro = new Libro
            {
                Id = 1,
                Titulo = "The Pragmatic Programmer",
                Autor = "Hunt & Thomas",
                Anio = 1999,
                Genero = "Programación"
            };

            // Act
            var dto = _mapper.Map<LibroResponseDto>(libro);

            // Assert
            Assert.That(dto, Is.Not.Null);
            Assert.That(dto.Id, Is.EqualTo(libro.Id));
            Assert.That(dto.Titulo, Is.EqualTo(libro.Titulo));
            Assert.That(dto.Autor, Is.EqualTo(libro.Autor));
            Assert.That(dto.Anio, Is.EqualTo(libro.Anio));
            Assert.That(dto.Genero, Is.EqualTo(libro.Genero));
        }

        [Test]
        public void Map_DeberiaManejarGeneroNulo_EnCreateLibroDto()
        {
            // Arrange
            var dto = new CreateLibroDto
            {
                Titulo = "Test",
                Autor = "Autor",
                Anio = 2000,
                Genero = null
            };

            // Act
            var libro = _mapper.Map<Libro>(dto);

            // Assert
            Assert.That(libro, Is.Not.Null);
            Assert.That(libro.Genero, Is.Null);
        }

        [Test]
        public void Map_DeberiaMappearMultiplesLibros_EnLista()
        {
            // Arrange
            var libros = new List<Libro>
            {
                new Libro { Id = 1, Titulo = "Libro 1", Autor = "Autor 1", Anio = 2000, Genero = "Drama" },
                new Libro { Id = 2, Titulo = "Libro 2", Autor = "Autor 2", Anio = 2001, Genero = "Terror" }
            };

            // Act
            var dtos = _mapper.Map<List<LibroResponseDto>>(libros);

            // Assert
            Assert.That(dtos, Is.Not.Null);
            Assert.That(dtos.Count, Is.EqualTo(2));
            Assert.That(dtos[0].Id, Is.EqualTo(1));
            Assert.That(dtos[1].Id, Is.EqualTo(2));
        }

        [Test]
        public void Map_DeberiaPreservarCamposVacios()
        {
            // Arrange
            var dto = new CreateLibroDto
            {
                Titulo = "",
                Autor = "",
                Anio = 0,
                Genero = ""
            };

            // Act
            var libro = _mapper.Map<Libro>(dto);

            // Assert
            Assert.That(libro, Is.Not.Null);
            Assert.That(libro.Titulo, Is.EqualTo(""));
            Assert.That(libro.Autor, Is.EqualTo(""));
        }
    }
}
