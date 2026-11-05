using BibliotecaMVC.Data;
using BibliotecaMVC.Models;

namespace BibliotecaMVC.Services
{
    public class LibroService : ILibroService
    {
        private readonly BibliotecaDbContext _context;

        public LibroService(BibliotecaDbContext context)
        {
            _context = context;
        }

        public IReadOnlyList<Libro> ObtenerTodos()
        {
            return _context.Libros
                .OrderBy(l => l.ID)
                .ToList();
        }

        public Libro? ObtenerPorId(int id)
        {
            return _context.Libros.Find(id);
        }

        public Libro Agregar(Libro libro)
        {
            if (string.IsNullOrWhiteSpace(libro.Imagen))
            {
                libro.Imagen = "/images/libro-default.jpg";
            }

            _context.Libros.Add(libro);
            _context.SaveChanges();

            return libro;
        }

        public bool Actualizar(int id, Libro libro)
        {
            var libroExistente = _context.Libros.Find(id);

            if (libroExistente == null)
            {
                return false;
            }

            libroExistente.Titulo = libro.Titulo;
            libroExistente.Autor = libro.Autor;
            libroExistente.Categoria = libro.Categoria;
            libroExistente.AnioPublicacion = libro.AnioPublicacion;
            libroExistente.Disponible = libro.Disponible;
            libroExistente.Imagen = string.IsNullOrWhiteSpace(libro.Imagen)
                ? "/images/libro-default.jpg"
                : libro.Imagen;

            _context.Libros.Update(libroExistente);
            _context.SaveChanges();

            return true;
        }

        public bool Eliminar(int id)
        {
            var libro = _context.Libros.Find(id);

            if (libro == null)
            {
                return false;
            }

            _context.Libros.Remove(libro);
            _context.SaveChanges();

            return true;
        }
    }
}
