using BibliotecaMVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaMVC.Controllers
{
    public class AutoresController : Controller
    {
        public IActionResult Index()
        {
            var autores = new List<Autor>
            {
                new Autor
                {
                    ID = 1,
                    Nombre = "Gabriel",
                    Apellido = "García Márquez",
                    Nacionalidad = "Colombiana",
                    FechaNacimiento = new DateTime(1927, 3, 6),
                    Activo = false
                },

                new Autor
                {
                    ID = 2,
                    Nombre = "Isabel",
                    Apellido = "Allende",
                    Nacionalidad = "Chilena",
                    FechaNacimiento = new DateTime(1942, 8, 2),
                    Activo = true
                },

                new Autor
                {
                    ID = 3,
                    Nombre = "Stephen",
                    Apellido = "King",
                    Nacionalidad = "Estadounidense",
                    FechaNacimiento = new DateTime(1947, 9, 21),
                    Activo = true
                },

                new Autor
                {
                    ID = 4,
                    Nombre = "Jane",
                    Apellido = "Austen",
                    Nacionalidad = "Británica",
                    FechaNacimiento = new DateTime(1775, 12, 16),
                    Activo = false
                },

                new Autor
                {
                    ID = 5,
                    Nombre = "Mario",
                    Apellido = "Vargas Llosa",
                    Nacionalidad = "Peruana",
                    FechaNacimiento = new DateTime(1936, 3, 28),
                    Activo = false
                },

                new Autor
                {
                    ID = 6,
                    Nombre = "J. K.",
                    Apellido = "Rowling",
                    Nacionalidad = "Británica",
                    FechaNacimiento = new DateTime(1965, 7, 31),
                    Activo = true
                }
            };

            return View(autores);
        }
    }
}