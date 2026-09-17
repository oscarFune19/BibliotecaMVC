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
    }
}