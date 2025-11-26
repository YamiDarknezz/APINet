using APINet.Data;
using APINet.Models;
using Microsoft.EntityFrameworkCore;

namespace APINet.Repositories
{
    public class LibroRepository : ILibroRepository
    {
        private readonly AppDbContext _context;

        public LibroRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Libro>> ObtenerTodosAsync()
        {
            return await _context.Libros.AsNoTracking().ToListAsync();
        }

        public async Task<Libro> ObtenerPorIdAsync(int id)
        {
            return await _context.Libros
                         .AsNoTracking()
                         .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<Libro> CrearAsync(Libro libro)
        {
            _context.Libros.Add(libro);
            await _context.SaveChangesAsync();
            return libro;
        }

        public async Task<bool> ActualizarAsync(Libro libro)
        {
            var libroExistente = await _context.Libros.FindAsync(libro.Id);
            if (libroExistente == null)
                return false;

            // Asignar valores
            libroExistente.Titulo = libro.Titulo;
            libroExistente.Autor = libro.Autor;
            libroExistente.Anio = libro.Anio;
            libroExistente.Genero = libro.Genero;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var libro = await _context.Libros.FindAsync(id);
            if (libro == null)
                return false;

            _context.Libros.Remove(libro);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
