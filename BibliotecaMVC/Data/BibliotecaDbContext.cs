using BibliotecaMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaMVC.Data
{
    public class BibliotecaDbContext : DbContext
    {
        public BibliotecaDbContext(
            DbContextOptions<BibliotecaDbContext> options)
            : base(options)
        {
        }

        public DbSet<Libro> Libros { get; set; }
    }
}