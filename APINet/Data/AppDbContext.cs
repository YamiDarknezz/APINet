using APINet.Models;
using Microsoft.EntityFrameworkCore;

namespace APINet.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public virtual DbSet<Libro> Libros { get; set; }
    }
}
