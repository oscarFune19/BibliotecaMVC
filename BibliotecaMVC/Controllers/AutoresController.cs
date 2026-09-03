using BibliotecaMVC.Models;
using BibliotecaMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaMVC.Controllers
{
    public class AutoresController : Controller
    {
        private readonly IAutorService _autorService;

        public AutoresController(IAutorService autorService)
        {
            _autorService = autorService;
        }

        // Listado de autores
        public IActionResult Index()
        {
            var autores = _autorService.ObtenerAutores();

            return View(autores);
        }

        // Detalle de un autor
        [HttpGet]
        public IActionResult Details(int id)
        {
            var autor = _autorService.ObtenerAutorPorId(id);

            if (autor == null)
            {
                return NotFound();
            }

            return View(autor);
        }

        // Muestra el formulario para editar
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var autor = _autorService.ObtenerAutorPorId(id);

            if (autor == null)
            {
                return NotFound();
            }

            return View(autor);
        }

        // Guarda los cambios del autor
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Autor autor)
        {
            var actualizado = _autorService.ActualizarAutor(autor);

            if (!actualizado)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        // Muestra la confirmación para eliminar
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var autor = _autorService.ObtenerAutorPorId(id);

            if (autor == null)
            {
                return NotFound();
            }

            return View(autor);
        }

        // Elimina definitivamente al autor
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _autorService.EliminarAutor(id);

            return RedirectToAction(nameof(Index));
        }
    }
}