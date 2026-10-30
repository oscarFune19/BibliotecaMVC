using BibliotecaMVC.Data;
using BibliotecaMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaMVC.Controllers
{
    [Authorize(Roles = IdentitySeeder.RolesConsulta)]
    public class LibrosController : Controller
    {
        private readonly BibliotecaDbContext _context;
        private readonly ILogger<LibrosController> _logger;

        public LibrosController(
            BibliotecaDbContext context,
            ILogger<LibrosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ==========================================
        // MOSTRAR LIBROS
        // ==========================================

        [HttpGet]
        public IActionResult Index()
        {
            _logger.LogInformation("Iniciando consulta del listado de libros.");

            try
            {
                var libros = _context.Libros
                    .OrderBy(l => l.ID)
                    .ToList();

                _logger.LogInformation(
                    "Consulta de libros completada. Se encontraron {Cantidad} registros.",
                    libros.Count);

                return View(libros);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Ocurrió un error al consultar el listado de libros.");

                throw;
            }
        }

        // ==========================================
        // VER DETALLE DE LIBRO
        // ==========================================

        [HttpGet]
        public IActionResult Details(int id)
        {
            _logger.LogInformation(
                "Iniciando consulta del detalle del libro con ID {LibroId}.",
                id);

            try
            {
                var libro = _context.Libros.Find(id);

                if (libro == null)
                {
                    _logger.LogWarning(
                        "No se encontró el libro con ID {LibroId}.",
                        id);

                    return NotFound();
                }

                return View(libro);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Ocurrió un error al consultar el libro con ID {LibroId}.",
                    id);

                throw;
            }
        }

        // ==========================================
        // AGREGAR LIBRO - GET
        // ==========================================

        [Authorize(Roles = IdentitySeeder.RolesGestion)]
        [HttpGet]
        public IActionResult Create()
        {
            _logger.LogInformation(
                "Iniciando apertura del formulario para crear un libro.");

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

        [Authorize(Roles = IdentitySeeder.RolesGestion)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Libro libro)
        {
            _logger.LogInformation(
                "Iniciando creación de un nuevo libro.");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning(
                    "La creación del libro fue detenida porque los datos no son válidos.");

                return View(libro);
            }

            try
            {
                if (string.IsNullOrWhiteSpace(libro.Imagen))
                {
                    libro.Imagen = "/images/libro-default.jpg";
                }

                _context.Libros.Add(libro);
                _context.SaveChanges();

                _logger.LogInformation(
                    "Libro creado correctamente con ID {LibroId}.",
                    libro.ID);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Ocurrió un error al crear un libro.");

                throw;
            }
        }

        // ==========================================
        // EDITAR LIBRO - GET
        // ==========================================

        [Authorize(Roles = IdentitySeeder.RolesGestion)]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            _logger.LogInformation(
                "Iniciando apertura del formulario de edición para el libro con ID {LibroId}.",
                id);

            try
            {
                var libro = _context.Libros.Find(id);

                if (libro == null)
                {
                    _logger.LogWarning(
                        "No se encontró el libro con ID {LibroId} para editar.",
                        id);

                    return NotFound();
                }

                return View(libro);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Ocurrió un error al cargar el libro con ID {LibroId} para editar.",
                    id);

                throw;
            }
        }

        // ==========================================
        // EDITAR LIBRO - POST
        // ==========================================

        [Authorize(Roles = IdentitySeeder.RolesGestion)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Libro libro)
        {
            _logger.LogInformation(
                "Iniciando actualización del libro con ID {LibroId}.",
                id);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning(
                    "La edición del libro con ID {LibroId} fue detenida porque los datos no son válidos.",
                    id);

                return View(libro);
            }

            try
            {
                var libroExistente = _context.Libros.Find(id);

                if (libroExistente == null)
                {
                    _logger.LogWarning(
                        "No se encontró el libro con ID {LibroId} para actualizar.",
                        id);

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

                _logger.LogInformation(
                    "Libro con ID {LibroId} actualizado correctamente.",
                    id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Ocurrió un error al actualizar el libro con ID {LibroId}.",
                    id);

                throw;
            }
        }

        // ==========================================
        // ELIMINAR LIBRO - GET
        // ==========================================

        [Authorize(Roles = IdentitySeeder.RolesGestion)]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            _logger.LogInformation(
                "Iniciando consulta para eliminar el libro con ID {LibroId}.",
                id);

            try
            {
                var libro = _context.Libros.Find(id);

                if (libro == null)
                {
                    _logger.LogWarning(
                        "No se encontró el libro con ID {LibroId} para eliminar.",
                        id);

                    return NotFound();
                }

                return View(libro);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Ocurrió un error al cargar el libro con ID {LibroId} para eliminar.",
                    id);

                throw;
            }
        }

        // ==========================================
        // ELIMINAR LIBRO - POST
        // ==========================================

        [Authorize(Roles = IdentitySeeder.RolesGestion)]
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _logger.LogInformation(
                "Iniciando eliminación del libro con ID {LibroId}.",
                id);

            try
            {
                var libro = _context.Libros.Find(id);

                if (libro == null)
                {
                    _logger.LogWarning(
                        "No se encontró el libro con ID {LibroId} para confirmar su eliminación.",
                        id);

                    return NotFound();
                }

                _context.Libros.Remove(libro);
                _context.SaveChanges();

                _logger.LogInformation(
                    "Libro con ID {LibroId} eliminado correctamente.",
                    id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Ocurrió un error al eliminar el libro con ID {LibroId}.",
                    id);

                throw;
            }
        }
    }
}
