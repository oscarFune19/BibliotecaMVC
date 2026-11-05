using BibliotecaMVC.Models;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaMVC.Tests
{
    public class LibroValidacionTests
    {
        [Fact]
        public void Libro_DatosValidos_SuperaValidacion()
        {
            var libro = new Libro
            {
                Titulo = "Libro válido",
                Autor = "Autor válido",
                Categoria = "Novela",
                AnioPublicacion = 2026,
                Disponible = true,
                Imagen = "/images/libro.jpg"
            };

            var resultados = Validar(libro);

            Assert.Empty(resultados);
            Assert.True(resultados.Count == 0);
        }

        [Theory]
        [InlineData("", "Autor", "Novela", 2026)]
        [InlineData("Título", "", "Novela", 2026)]
        [InlineData("Título", "Autor", "", 2026)]
        [InlineData("Título", "Autor", "Novela", 0)]
        [InlineData("Título", "Autor", "Novela", 2200)]
        public void Libro_DatosInvalidos_GeneraErroresDeValidacion(
            string titulo,
            string autor,
            string categoria,
            int anio)
        {
            var libro = new Libro
            {
                Titulo = titulo,
                Autor = autor,
                Categoria = categoria,
                AnioPublicacion = anio,
                Disponible = true
            };

            var resultados = Validar(libro);

            Assert.False(resultados.Count == 0);
            Assert.NotEmpty(resultados);
            Assert.NotNull(resultados[0].ErrorMessage);
        }

        private static List<ValidationResult> Validar(Libro libro)
        {
            var resultados = new List<ValidationResult>();
            var contexto = new ValidationContext(libro);

            Validator.TryValidateObject(
                libro,
                contexto,
                resultados,
                validateAllProperties: true);

            return resultados;
        }
    }
}
