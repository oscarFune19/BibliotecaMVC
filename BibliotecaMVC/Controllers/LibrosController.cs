using BibliotecaMVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaMVC.Controllers
{
    public class LibrosController : Controller
    {
        private static List<Libro> libros = new List<Libro>
        {
            new Libro
            {
                ID = 1,
                Titulo = "Cien años de soledad",
                Autor = "Gabriel García Márquez",
                Categoria = "Realismo mágico",
                AnioPublicacion = 1967,
                Disponible = true,
                Imagen = "/images/cien-anios.jpg"
            },

            new Libro
            {
                ID = 2,
                Titulo = "1984",
                Autor = "George Orwell",
                Categoria = "Distopía",
                AnioPublicacion = 1949,
                Disponible = false,
                Imagen = "/images/1984.jpg"
            },

            new Libro
            {
                ID = 3,
                Titulo = "El principito",
                Autor = "Antoine de Saint-Exupéry",
                Categoria = "Literatura infantil",
                AnioPublicacion = 1943,
                Disponible = true,
                Imagen = "/images/principito.jpg"
            },

            new Libro
            {
                ID = 4,
                Titulo = "Orgullo y prejuicio",
                Autor = "Jane Austen",
                Categoria = "Romance",
                AnioPublicacion = 1813,
                Disponible = true,
                Imagen = "/images/orgullo-prejuicio.jpg"
            },

            new Libro
            {
                ID = 5,
                Titulo = "Fahrenheit 451",
                Autor = "Ray Bradbury",
                Categoria = "Ciencia ficción",
                AnioPublicacion = 1953,
                Disponible = false,
                Imagen = "/images/fahrenheit451.jpg"
            }
        };

        // LISTAR
        public IActionResult Index()
        {
            return View(libros);
        }

        // DETALLE
        public IActionResult Details(int id)
        {
            var libro = libros.FirstOrDefault(l => l.ID == id);

            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }

        // CREAR - GET
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Libro
            {
                Disponible = true,
                Imagen = "/images/libro-default.jpg"
            });
        }

        // CREAR - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Libro libro)
        {
            if (libros.Count == 0)
            {
                libro.ID = 1;
            }
            else
            {
                libro.ID = libros.Max(l => l.ID) + 1;
            }

            libros.Add(libro);

            return RedirectToAction(nameof(Index));
        }

        // EDITAR - GET
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var libro = libros.FirstOrDefault(l => l.ID == id);

            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }

        // EDITAR - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Libro libro)
        {
            var libroExistente =
                libros.FirstOrDefault(l => l.ID == libro.ID);

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

            return RedirectToAction(nameof(Index));
        }

        // ELIMINAR - GET
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var libro = libros.FirstOrDefault(l => l.ID == id);

            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }

        // ELIMINAR - POST
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var libro = libros.FirstOrDefault(l => l.ID == id);

            if (libro != null)
            {
                libros.Remove(libro);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}