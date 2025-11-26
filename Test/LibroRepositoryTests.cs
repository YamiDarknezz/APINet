using APINet.Data;
using APINet.Models;
using APINet.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests
{
    [TestFixture]
    public class LibroRepositoryTests
    {
        private AppDbContext _context;
        private ILibroRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                // Cada prueba usa una BD en memoria diferente (para evitar contaminación de datos).
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _repository = new LibroRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }

        [Test]
        public async Task CrearAsync_DeberiaAgregarLibro_Exitoso()
        {
            // Arrange
            var libro = new Libro
            {
                Titulo = "Cien Años de Soledad",
                Autor = "Gabriel García Márquez",
                Anio = 1967,
                Genero = "Novela"
            };

            // Act
            var result = await _repository.CrearAsync(libro);
            var librosEnDb = await _repository.ObtenerTodosAsync();

            // Assert
            Assert.That(result.Id, Is.GreaterThan(0));
            Assert.That(librosEnDb.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task ObtenerPorIdAsync_DeberiaRetornarNull_SiNoExiste()
        {
            // Arrange
            int idInexistente = 999;

            // Act
            var result = await _repository.ObtenerPorIdAsync(idInexistente);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task ActualizarAsync_DeberiaRetornarTrue_SiLibroExiste()
        {
            // Arrange
            var libro = new Libro
            {
                Titulo = "Original",
                Autor = "Autor X",
                Anio = 2000,
                Genero = "Drama"
            };

            _context.Libros.Add(libro);
            await _context.SaveChangesAsync();

            libro.Titulo = "Modificado";

            // Act
            var result = await _repository.ActualizarAsync(libro);
            var actualizado = await _repository.ObtenerPorIdAsync(libro.Id);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(actualizado.Titulo, Is.EqualTo("Modificado"));
        }

        [Test]
        public async Task ActualizarAsync_DeberiaRetornarFalse_SiLibroNoExiste()
        {
            // Arrange
            var libro = new Libro
            {
                Id = 1234, // Id inexistente
                Titulo = "Libro fantasma",
                Autor = "Desconocido",
                Anio = 2021,
                Genero = "Terror"
            };

            // Act
            var result = await _repository.ActualizarAsync(libro);

            // Assert
            Assert.That(result, Is.False);
        }

        // Caso exitoso: eliminar un libro existente debe retornar true
        [Test]
        public async Task EliminarAsync_DeberiaRetornarTrue_SiLibroExiste()
        {
            // Data falsa: libro creado en memoria
            var libro = new Libro
            {
                Titulo = "Para Eliminar",
                Autor = "Autor Z",
                Anio = 2010,
                Genero = "Acción"
            };

            // Agrego el libro a la base de datos en memoria
            _context.Libros.Add(libro);
            await _context.SaveChangesAsync();

            // Se ejecuta el método a probar y se obtiene el resultado
            var result = await _repository.EliminarAsync(libro.Id);

            // Se consulta nuevamente para verificar que efectivamente se eliminó
            var libroEnDb = await _repository.ObtenerPorIdAsync(libro.Id);

            // Se comparan los resultados esperados
            Assert.That(result, Is.True);
            Assert.That(libroEnDb, Is.Null);
        }

        // Caso de error: intentar eliminar un libro inexistente debe retornar false
        [Test]
        public async Task EliminarAsync_DeberiaRetornarFalse_SiLibroNoExiste()
        {
            int idInexistente = 555; // ID falso, no agregado a la BD

            // Se ejecuta el método a probar
            var result = await _repository.EliminarAsync(idInexistente);

            // Se compara el resultado con lo esperado
            Assert.That(result, Is.False);
        }

        // Caso exitoso: si hay libros en la BD, debe retornarlos todos
        [Test]
        public async Task ObtenerTodosAsync_DeberiaRetornarListaConLibros()
        {
            // Data falsa: dos libros creados en memoria
            var libro1 = new Libro { Titulo = "Libro 1", Autor = "Autor A", Anio = 2000, Genero = "Drama" };
            var libro2 = new Libro { Titulo = "Libro 2", Autor = "Autor B", Anio = 2005, Genero = "Terror" };

            // Agrego los libros a la base de datos en memoria
            _context.Libros.AddRange(libro1, libro2);
            await _context.SaveChangesAsync();

            // Se ejecuta el método a probar
            var resultado = await _repository.ObtenerTodosAsync();

            // Se compara que la cantidad de libros devuelta sea la esperada
            Assert.That(resultado.Count(), Is.EqualTo(2));
        }

        // Caso de error: si no hay libros, debe retornar lista vacía
        [Test]
        public async Task ObtenerTodosAsync_DeberiaRetornarListaVacia_SiNoHayLibros()
        {
            // No se agrega ningún libro a la base de datos

            // Se ejecuta el método a probar
            var resultado = await _repository.ObtenerTodosAsync();

            // Se compara el resultado con la lista vacía esperada
            Assert.That(resultado, Is.Empty);
        }
    }
}
