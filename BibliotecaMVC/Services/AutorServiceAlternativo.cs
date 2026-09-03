using BibliotecaMVC.Models;

namespace BibliotecaMVC.Services
{
    public class AutorServiceAlternativo : IAutorService
    {
        private static readonly List<Autor> autores = new()
        {
            new Autor
            {
                ID = 101,
                Nombre = "Julio",
                Apellido = "Cortázar",
                Nacionalidad = "Argentina",
                FechaNacimiento = new DateTime(1914, 8, 26),
                Activo = true
            },

            new Autor
            {
                ID = 102,
                Nombre = "Virginia",
                Apellido = "Woolf",
                Nacionalidad = "Británica",
                FechaNacimiento = new DateTime(1882, 1, 25),
                Activo = true
            },

            new Autor
            {
                ID = 103,
                Nombre = "Jorge Luis",
                Apellido = "Borges",
                Nacionalidad = "Argentina",
                FechaNacimiento = new DateTime(1899, 8, 24),
                Activo = false
            },

            new Autor
            {
                ID = 104,
                Nombre = "Agatha",
                Apellido = "Christie",
                Nacionalidad = "Británica",
                FechaNacimiento = new DateTime(1890, 9, 15),
                Activo = true
            }
        };

        public List<Autor> ObtenerAutores()
        {
            return autores;
        }

        public Autor? ObtenerAutorPorId(int id)
        {
            return autores.FirstOrDefault(a => a.ID == id);
        }

        public bool ActualizarAutor(Autor autor)
        {
            var autorExistente = ObtenerAutorPorId(autor.ID);

            if (autorExistente == null)
            {
                return false;
            }

            autorExistente.Nombre = autor.Nombre;
            autorExistente.Apellido = autor.Apellido;
            autorExistente.Nacionalidad = autor.Nacionalidad;
            autorExistente.FechaNacimiento = autor.FechaNacimiento;
            autorExistente.Activo = autor.Activo;

            return true;
        }

        public bool EliminarAutor(int id)
        {
            var autor = ObtenerAutorPorId(id);

            if (autor == null)
            {
                return false;
            }

            autores.Remove(autor);
            return true;
        }
    }
}