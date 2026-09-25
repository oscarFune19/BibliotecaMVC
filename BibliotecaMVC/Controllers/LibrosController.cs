using BibliotecaMVC.Data;
using BibliotecaMVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaMVC.Controllers
{
    public class LibrosController : Controller
    {
        private readonly BibliotecaDbContext _context;

        public LibrosController(BibliotecaDbContext context)
        {
            _context = context;
        }


        // ==========================================
        // MOSTRAR LIBROS
        // ==========================================

        [HttpGet]
        public IActionResult Index()
        {
            var libros = _context.Libros
                .OrderBy(l => l.ID)
                .ToList();

            return View(libros);
        }


        // ==========================================
        // AGREGAR LIBRO - GET
        // ==========================================

        [HttpGet]
        public IActionResult Create()
        {
            var libro = new Libro
            {
                Disponible = true,
                Imagen = "/images/libro-default.jpg"
            };

            return View(libro);
        }


        // ==========================================
        // AGREGAR LIBRO - POST
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Libro libro)
        {
            if (!ModelState.IsValid)
            {
                return View(libro);
            }

            if (string.IsNullOrWhiteSpace(libro.Imagen))
            {
                libro.Imagen = "/images/libro-default.jpg";
            }

            _context.Libros.Add(libro);

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


        // ==========================================
        // EDITAR LIBRO - GET
        // ==========================================

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var libro = _context.Libros.Find(id);

            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }


        // ==========================================
        // EDITAR LIBRO - POST
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Libro libro)
        {
            if (!ModelState.IsValid)
            {
                return View(libro);
            }

            var libroExistente = _context.Libros.Find(id);

            if (libroExistente == null)
            {
                return NotFound();
            }

            libroExistente.Titulo = libro.Titulo;
            libroExistente.Autor = libro.Autor;
            libroExistente.Categoria = libro.Categoria;
            libroExistente.AnioPublicacion = libro.AnioPublicacion;
            libroExistente.Disponible = libro.Disponible;
            libroExistente.Imagen = libro.Imagen;

            if (string.IsNullOrWhiteSpace(libroExistente.Imagen))
            {
                libroExistente.Imagen = "/images/libro-default.jpg";
            }

            _context.Libros.Update(libroExistente);

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


        // ==========================================
        // ELIMINAR LIBRO - GET
        // ==========================================

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var libro = _context.Libros.Find(id);

            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }


        // ==========================================
        // ELIMINAR LIBRO - POST
        // ==========================================

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var libro = _context.Libros.Find(id);

            if (libro == null)
            {
                return NotFound();
            }

            _context.Libros.Remove(libro);

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}