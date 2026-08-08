namespace BibliotecaMVC.Models
{
    public class Libro
    {
        public int ID { get; set; }

        public string Titulo { get; set; } = string.Empty;

        public string Autor { get; set; } = string.Empty;

        public string Categoria { get; set; } = string.Empty;

        public int AnioPublicacion { get; set; }

        public bool Disponible { get; set; }

        public string Imagen { get; set; } = string.Empty;
    }
}