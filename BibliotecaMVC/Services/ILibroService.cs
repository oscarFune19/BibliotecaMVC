using BibliotecaMVC.Models;

namespace BibliotecaMVC.Services
{
    public interface ILibroService
    {
        IReadOnlyList<Libro> ObtenerTodos();
        Libro? ObtenerPorId(int id);
        Libro Agregar(Libro libro);
        bool Actualizar(int id, Libro libro);
        bool Eliminar(int id);
    }
}
