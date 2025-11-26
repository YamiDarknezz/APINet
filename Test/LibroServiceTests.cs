using APINet.Models;
using APINet.Repositories;
using APINet.Service;
using Moq;

namespace Tests
{
    [TestFixture]
    public class LibroServiceTests
    {
        private Mock<ILibroRepository> _repoMock;
        private LibroService _service;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<ILibroRepository>();
            _service = new LibroService(_repoMock.Object);
        }

        [Test]
        public async Task Crear_DeberiaCrearLibro_Exitoso()
        {
            // Arrange
            var nuevoLibro = new Libro { Id = 1, Titulo = "Clean Code", Autor = "Robert Martin", Anio = 2008, Genero = "Programación" };
            _repoMock.Setup(r => r.ObtenerTodosAsync()).ReturnsAsync(new List<Libro>());
            _repoMock.Setup(r => r.CrearAsync(It.IsAny<Libro>())).ReturnsAsync(nuevoLibro);

            // Act
            var resultado = await _service.Crear(nuevoLibro);

            // Assert
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.Titulo, Is.EqualTo("Clean Code"));
        }

        // NOTA: Los tests de validación de campos (titulo vacío, autor vacío, etc.)
        // se eliminaron porque ahora FluentValidation se encarga de esas validaciones
        // antes de que llegue al servicio. El servicio solo maneja lógica de negocio.

        [Test]
        public void Crear_DeberiaLanzarExcepcion_CuandoExisteDuplicado()
        {
            // Arrange
            var libroExistente = new Libro { Id = 1, Titulo = "Clean Code", Autor = "Robert C. Martin", Anio = 2008, Genero = "Software" };
            var nuevoLibro = new Libro { Id = 2, Titulo = "Clean Code", Autor = "Robert C. Martin", Anio = 2010, Genero = "Programación" };
            _repoMock.Setup(r => r.ObtenerTodosAsync()).ReturnsAsync(new List<Libro> { libroExistente });

            // Act
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _service.Crear(nuevoLibro));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Ya existe un libro con el mismo título y autor."));
        }

        // Caso exitoso: devuelve los libros ordenados por Id descendente
        [Test]
        public async Task ObtenerTodos_DeberiaRetornarLibrosOrdenadosDesc()
        {
            // Data falsa: lista de libros creada en memoria con IDs 1 y 2
            var libros = new List<Libro>
            {
                new Libro { Id = 1, Titulo = "A", Autor = "X", Anio = 2000, Genero = "Drama" },
                new Libro { Id = 2, Titulo = "B", Autor = "Y", Anio = 2001, Genero = "Terror" }
            };
            // Se configura el mock del repositorio para devolver la lista de libros
            _repoMock.Setup(r => r.ObtenerTodosAsync()).ReturnsAsync(libros);

            // Act: se ejecuta el método del servicio
            var resultado = await _service.ObtenerTodos();

            // Assert: se verifica que el primer libro devuelto sea el de ID más alto (orden descendente)
            Assert.That(resultado.First().Id, Is.EqualTo(2));
        }

        // Caso exitoso: elimina el libro y retorna true
        [Test]
        public async Task Eliminar_DeberiaEliminarLibro_Exitoso()
        {
            // Data falsa: libro agregado al mock para simular existencia
            var libro = new Libro { Id = 1, Titulo = "Libro X", Autor = "Autor X", Anio = 2000, Genero = "Drama" };
            _repoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(libro);
            _repoMock.Setup(r => r.EliminarAsync(1)).ReturnsAsync(true);

            // Act: se ejecuta el método del servicio para eliminar
            var resultado = await _service.Eliminar(1);

            // Assert: se verifica que el método retorne true indicando eliminación correcta
            Assert.That(resultado, Is.True);
        }

        // Caso de error: Id <= 0 → lanza ArgumentException
        [Test]
        public void ObtenerPorId_DeberiaLanzarExcepcion_SiIdEsMenorIgualCero()
        {
            // Act: se prueba con ID inválido (0)
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _service.ObtenerPorId(0));

            // Assert: se verifica que la excepción tenga el mensaje esperado
            Assert.That(ex.Message, Is.EqualTo("El Id debe ser mayor a cero."));
        }

        [Test]
        public async Task ObtenerPorId_DeberiaRetornarLibro_CuandoExiste()
        {
            // Arrange
            var libro = new Libro { Id = 1, Titulo = "Test", Autor = "Autor", Anio = 2000, Genero = "Drama" };
            _repoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(libro);

            // Act
            var resultado = await _service.ObtenerPorId(1);

            // Assert
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.Id, Is.EqualTo(1));
            Assert.That(resultado.Titulo, Is.EqualTo("Test"));
        }

        [Test]
        public void ObtenerPorId_DeberiaLanzarKeyNotFoundException_CuandoNoExiste()
        {
            // Arrange
            _repoMock.Setup(r => r.ObtenerPorIdAsync(999)).ReturnsAsync((Libro?)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => _service.ObtenerPorId(999));
            Assert.That(ex.Message, Is.EqualTo("No se encontró un libro con Id 999."));
        }

        [Test]
        public async Task Actualizar_DeberiaActualizarLibro_Exitoso()
        {
            // Arrange
            var libro = new Libro { Id = 1, Titulo = "Actualizado", Autor = "Autor", Anio = 2000, Genero = "Drama" };
            _repoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(libro);
            _repoMock.Setup(r => r.ObtenerTodosAsync()).ReturnsAsync(new List<Libro> { libro });
            _repoMock.Setup(r => r.ActualizarAsync(libro)).ReturnsAsync(true);

            // Act
            var resultado = await _service.Actualizar(libro);

            // Assert
            Assert.That(resultado, Is.True);
            _repoMock.Verify(r => r.ActualizarAsync(libro), Times.Once);
        }

        [Test]
        public void Actualizar_DeberiaLanzarKeyNotFoundException_SiNoSeActualiza()
        {
            // Arrange
            var libro = new Libro { Id = 999, Titulo = "No existe", Autor = "Autor", Anio = 2000, Genero = "Drama" };
            _repoMock.Setup(r => r.ObtenerPorIdAsync(999)).ReturnsAsync((Libro?)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => _service.Actualizar(libro));
            Assert.That(ex.Message, Is.EqualTo("No se encontró un libro con Id 999."));
        }

        [Test]
        public void Eliminar_DeberiaLanzarArgumentException_SiIdEsCero()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _service.Eliminar(0));
            Assert.That(ex.Message, Is.EqualTo("El Id debe ser mayor a cero."));
        }

        [Test]
        public void Eliminar_DeberiaLanzarArgumentException_SiIdEsNegativo()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _service.Eliminar(-1));
            Assert.That(ex.Message, Is.EqualTo("El Id debe ser mayor a cero."));
        }

        [Test]
        public async Task ObtenerTodos_DeberiaRetornarListaVacia_CuandoNoHayLibros()
        {
            // Arrange
            _repoMock.Setup(r => r.ObtenerTodosAsync()).ReturnsAsync(new List<Libro>());

            // Act
            var resultado = await _service.ObtenerTodos();

            // Assert
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado, Is.Empty);
        }

    }
}
