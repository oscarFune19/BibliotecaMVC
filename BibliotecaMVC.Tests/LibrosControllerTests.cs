using BibliotecaMVC.Controllers;
using BibliotecaMVC.Models;
using BibliotecaMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace BibliotecaMVC.Tests
{
    public class LibrosControllerTests
    {
        private readonly Mock<ILibroService> _servicioMock;
        private readonly Mock<ILogger<LibrosController>> _loggerMock;

        public LibrosControllerTests()
        {
            _servicioMock = new Mock<ILibroService>();
            _loggerMock = new Mock<ILogger<LibrosController>>();
        }

        private LibrosController CrearControlador()
        {
            return new LibrosController(
                _servicioMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public void Index_ConLibros_RetornaVistaConDatos()
        {
            var libros = new List<Libro>
            {
                new()
                {
                    ID = 1,
                    Titulo = "Libro uno",
                    Autor = "Autor uno",
                    Categoria = "Novela",
                    AnioPublicacion = 2024,
                    Disponible = true
                },
                new()
                {
                    ID = 2,
                    Titulo = "Libro dos",
                    Autor = "Autor dos",
                    Categoria = "Historia",
                    AnioPublicacion = 2025,
                    Disponible = false
                }
            };

            _servicioMock
                .Setup(s => s.ObtenerTodos())
                .Returns(libros);

            var controlador = CrearControlador();

            var resultado = controlador.Index();

            var vista = Assert.IsType<ViewResult>(resultado);
            var modelo = Assert.IsAssignableFrom<IReadOnlyList<Libro>>(vista.Model);

            Assert.NotNull(modelo);
            Assert.Equal(2, modelo.Count);
            Assert.Equal("Libro uno", modelo[0].Titulo);
            Assert.True(modelo[0].Disponible);

            _servicioMock.Verify(
                s => s.ObtenerTodos(),
                Times.Once);
        }

        [Fact]
        public void Index_SinLibros_RetornaColeccionVacia()
        {
            _servicioMock
                .Setup(s => s.ObtenerTodos())
                .Returns(Array.Empty<Libro>());

            var controlador = CrearControlador();

            var resultado = controlador.Index();

            var vista = Assert.IsType<ViewResult>(resultado);
            var modelo = Assert.IsAssignableFrom<IReadOnlyList<Libro>>(vista.Model);

            Assert.Empty(modelo);

            _servicioMock.Verify(
                s => s.ObtenerTodos(),
                Times.Once);
        }

        [Fact]
        public void Details_LibroExiste_RetornaVistaConLibro()
        {
            var libro = new Libro
            {
                ID = 7,
                Titulo = "Libro encontrado",
                Autor = "Autor",
                Categoria = "Ciencia",
                AnioPublicacion = 2023,
                Disponible = true
            };

            _servicioMock
                .Setup(s => s.ObtenerPorId(7))
                .Returns(libro);

            var controlador = CrearControlador();

            var resultado = controlador.Details(7);

            var vista = Assert.IsType<ViewResult>(resultado);
            var modelo = Assert.IsType<Libro>(vista.Model);

            Assert.NotNull(modelo);
            Assert.Equal(7, modelo.ID);
            Assert.Equal("Libro encontrado", modelo.Titulo);
        }

        [Fact]
        public void Details_LibroNoExiste_RetornaNotFound()
        {
            _servicioMock
                .Setup(s => s.ObtenerPorId(99))
                .Returns((Libro?)null);

            var controlador = CrearControlador();

            var resultado = controlador.Details(99);

            Assert.IsType<NotFoundResult>(resultado);
        }

        [Fact]
        public void Create_Get_RetornaLibroConValoresIniciales()
        {
            var controlador = CrearControlador();

            var resultado = controlador.Create();

            var vista = Assert.IsType<ViewResult>(resultado);
            var modelo = Assert.IsType<Libro>(vista.Model);

            Assert.True(modelo.Disponible);
            Assert.Equal("/images/libro-default.jpg", modelo.Imagen);
        }

        [Fact]
        public void Create_Post_DatosValidos_AgregaYRedirige()
        {
            var libro = new Libro
            {
                Titulo = "Nuevo libro",
                Autor = "Nuevo autor",
                Categoria = "Tecnología",
                AnioPublicacion = 2026,
                Disponible = true
            };

            _servicioMock
                .Setup(s => s.Agregar(libro))
                .Returns(libro);

            var controlador = CrearControlador();

            var resultado = controlador.Create(libro);

            var redireccion =
                Assert.IsType<RedirectToActionResult>(resultado);

            Assert.Equal(nameof(LibrosController.Index), redireccion.ActionName);

            _servicioMock.Verify(
                s => s.Agregar(libro),
                Times.Once);
        }

        [Fact]
        public void Create_Post_ModeloInvalido_NoAgregaLibro()
        {
            var libro = new Libro();

            var controlador = CrearControlador();
            controlador.ModelState.AddModelError(
                "Titulo",
                "El título es obligatorio.");

            var resultado = controlador.Create(libro);

            var vista = Assert.IsType<ViewResult>(resultado);

            Assert.Equal(libro, vista.Model);
            Assert.False(controlador.ModelState.IsValid);

            _servicioMock.Verify(
                s => s.Agregar(It.IsAny<Libro>()),
                Times.Never);
        }

        [Fact]
        public void Edit_Get_LibroNoExiste_RetornaNotFound()
        {
            _servicioMock
                .Setup(s => s.ObtenerPorId(50))
                .Returns((Libro?)null);

            var controlador = CrearControlador();

            var resultado = controlador.Edit(50);

            Assert.IsType<NotFoundResult>(resultado);
        }

        [Fact]
        public void Edit_Post_LibroExiste_ActualizaYRedirige()
        {
            var libro = new Libro
            {
                ID = 3,
                Titulo = "Libro editado",
                Autor = "Autor",
                Categoria = "Ensayo",
                AnioPublicacion = 2026,
                Disponible = true
            };

            _servicioMock
                .Setup(s => s.Actualizar(3, libro))
                .Returns(true);

            var controlador = CrearControlador();

            var resultado = controlador.Edit(3, libro);

            var redireccion =
                Assert.IsType<RedirectToActionResult>(resultado);

            Assert.Equal(nameof(LibrosController.Index), redireccion.ActionName);

            _servicioMock.Verify(
                s => s.Actualizar(3, libro),
                Times.Once);
        }

        [Fact]
        public void Edit_Post_LibroNoExiste_RetornaNotFound()
        {
            var libro = new Libro
            {
                ID = 100,
                Titulo = "No existe",
                Autor = "Autor",
                Categoria = "Otro",
                AnioPublicacion = 2026
            };

            _servicioMock
                .Setup(s => s.Actualizar(100, libro))
                .Returns(false);

            var controlador = CrearControlador();

            var resultado = controlador.Edit(100, libro);

            Assert.IsType<NotFoundResult>(resultado);
        }

        [Fact]
        public void Delete_Get_LibroExiste_RetornaVista()
        {
            var libro = new Libro
            {
                ID = 8,
                Titulo = "Libro a eliminar",
                Autor = "Autor",
                Categoria = "Novela",
                AnioPublicacion = 2020
            };

            _servicioMock
                .Setup(s => s.ObtenerPorId(8))
                .Returns(libro);

            var controlador = CrearControlador();

            var resultado = controlador.Delete(8);

            var vista = Assert.IsType<ViewResult>(resultado);
            var modelo = Assert.IsType<Libro>(vista.Model);

            Assert.Equal(8, modelo.ID);
        }

        [Fact]
        public void DeleteConfirmed_LibroExiste_EliminaYRedirige()
        {
            _servicioMock
                .Setup(s => s.Eliminar(8))
                .Returns(true);

            var controlador = CrearControlador();

            var resultado = controlador.DeleteConfirmed(8);

            var redireccion =
                Assert.IsType<RedirectToActionResult>(resultado);

            Assert.Equal(nameof(LibrosController.Index), redireccion.ActionName);

            _servicioMock.Verify(
                s => s.Eliminar(8),
                Times.Once);
        }

        [Fact]
        public void DeleteConfirmed_LibroNoExiste_RetornaNotFound()
        {
            _servicioMock
                .Setup(s => s.Eliminar(404))
                .Returns(false);

            var controlador = CrearControlador();

            var resultado = controlador.DeleteConfirmed(404);

            Assert.IsType<NotFoundResult>(resultado);

            _servicioMock.Verify(
                s => s.Eliminar(404),
                Times.Once);
        }
    }
}
