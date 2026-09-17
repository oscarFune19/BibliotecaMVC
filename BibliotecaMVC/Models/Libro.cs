using System.ComponentModel.DataAnnotations;

namespace BibliotecaMVC.Models
{
    public class Libro
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(150)]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El autor es obligatorio.")]
        [StringLength(120)]
        public string Autor { get; set; } = string.Empty;

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        [StringLength(100)]
        public string Categoria { get; set; } = string.Empty;

        [Display(Name = "Año de publicación")]
        [Range(1, 2100, ErrorMessage = "Ingrese un año válido.")]
        public int AnioPublicacion { get; set; }

        public bool Disponible { get; set; }

        [StringLength(250)]
        public string Imagen { get; set; } = string.Empty;
    }
}