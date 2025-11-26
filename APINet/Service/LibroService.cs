using APINet.Models;
using APINet.Repositories;

namespace APINet.Service
{
    public class LibroService : ILibroService
    {
        private readonly ILibroRepository _libroRepository;

        public LibroService(ILibroRepository libroRepository)
        {
            _libroRepository = libroRepository;
        }

        public async Task<IEnumerable<Libro>> ObtenerTodos()
        {
            var libros = await _libroRepository.ObtenerTodosAsync();

            // Regla de negocio: devolver ordenados por ID descendente
            return libros.OrderByDescending(l => l.Id);
        }

        public async Task<Libro> ObtenerPorId(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El Id debe ser mayor a cero.");

            var libro = await _libroRepository.ObtenerPorIdAsync(id);

            if (libro == null)
                throw new KeyNotFoundException($"No se encontró un libro con Id {id}.");

            return libro;
        }

        public async Task<Libro> Crear(Libro libro)
        {
            // Validación de lógica de negocio: no permitir duplicados (título + autor)
            // Nota: Las validaciones básicas (required, length, etc.) se manejan en FluentValidation
            var existentes = await _libroRepository.ObtenerTodosAsync();
            if (existentes.Any(l =>
                l.Titulo.Equals(libro.Titulo, StringComparison.OrdinalIgnoreCase) &&
                l.Autor.Equals(libro.Autor, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("Ya existe un libro con el mismo título y autor.");
            }

            return await _libroRepository.CrearAsync(libro);
        }

        public async Task<bool> Actualizar(Libro libro)
        {
            // Verificar que el libro existe
            var existente = await _libroRepository.ObtenerPorIdAsync(libro.Id);
            if (existente == null)
                throw new KeyNotFoundException($"No se encontró un libro con Id {libro.Id}.");

            // Validación de lógica de negocio: no permitir duplicados (excluyendo el libro actual)
            // Nota: Las validaciones básicas (required, length, etc.) se manejan en FluentValidation
            var todos = await _libroRepository.ObtenerTodosAsync();
            if (todos.Any(l =>
                l.Id != libro.Id &&
                l.Titulo.Equals(libro.Titulo, StringComparison.OrdinalIgnoreCase) &&
                l.Autor.Equals(libro.Autor, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("Ya existe otro libro con el mismo título y autor.");
            }

            return await _libroRepository.ActualizarAsync(libro);
        }

        public async Task<bool> Eliminar(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El Id debe ser mayor a cero.");

            var libro = await _libroRepository.ObtenerPorIdAsync(id);
            if (libro == null)
                throw new KeyNotFoundException($"No se encontró un libro con Id {id}.");

            // Eliminación física desde repositorio
            return await _libroRepository.EliminarAsync(id);
        }
    }
    }
