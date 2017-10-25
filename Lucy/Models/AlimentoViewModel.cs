using Foolproof;
using ModelCL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lucy.Models
{
    public class AlimentoViewModel
    {
        [Key]
        public long AlimentoId { get; set; }

        [Required]
        [Display(Name = "Nombre")]
        public string AlimentoNombre { get; set; }

        [Display(Name = "Imagen")]
        public string AlimentoImagen { get; set; }

        [Required]
        [Display(Name = "Porción")]
        public string AlimentoPorcion { get; set; }

        [Display(Name = "Calorías")]
        [Range(0, 2000, ErrorMessage = "El valor debe estar entre {1} y {2}.")]
        public Nullable<short> AlimentoCalorias { get; set; }

        [Display(Name = "Carbohidratos")]
        [Range(0, 2000, ErrorMessage = "El valor debe estar entre {1} y {2}.")]
        public Nullable<short> AlimentoCarbohidratos { get; set; }

        [Display(Name = "Grasas")]
        [Range(typeof(double), "0", "2000", ErrorMessage = "El valor debe estar entre {1} y {2}.")]
        public Nullable<double> AlimentoGrasa { get; set; }

        [Display(Name = "Azucares")]
        [Range(typeof(double), "0", "2000", ErrorMessage = "El valor debe estar entre {1} y {2}.")]
        public Nullable<double> AlimentoAzucar { get; set; }

        [Display(Name = "Sodio")]
        [Range(typeof(double), "0", "2000", ErrorMessage = "El valor debe estar entre {1} y {2}.")]
        public Nullable<double> AlimentoSodio { get; set; }

        [Display(Name = "Gluten")]
        public Nullable<bool> AlimentoGluten { get; set; }
    }
}