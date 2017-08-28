using Foolproof;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Backend.Models
{
    public class RecetasViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Título")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Este campo debe tener un largo mínimo de {2} caracteres y un máximo de {3}.")]
        public string ContenidoTitulo { get; set; }

        [Required]
        [Display(Name = "Descripción")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Este campo debe tener un largo mínimo de {2} caracteres y un máximo de {3}.")]
        public string RecetaDescripcion { get; set; }

        [Required]
        [Display(Name = "Cuerpo")]
        [StringLength(255, MinimumLength = 2, ErrorMessage = "Este campo debe tener un largo mínimo de {2} caracteres y un máximo de {3}.")]
        public string ContenidoCuerpo { get; set; }

        [Display(Name = "Calorías")]
        public Nullable<double> RecetaCalorias { get; set; }

        [Display(Name = "Hidratos")]
        public Nullable<double> RecetaHidratos { get; set; }

        [Display(Name = "Sodio")]
        public Nullable<double> RecetaSodio { get; set; }

        [Display(Name = "Gluten")]
        public Nullable<double> RecetaGluten { get; set; }
    }
}