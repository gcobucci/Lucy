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
    public class ComidaAlimentoViewModel
    {
        [Key]
        public long AlimentoId { get; set; }

        [Display(Name = "Alimento")]
        public string AlimentoNombre { get; set; }
        [Display(Name = "Imagen")]
        public string AlimentoImagen { get; set; }
        [Display(Name = "Porción")]
        public string AlimentoPorcion { get; set; }
        [Display(Name = "Calorías")]
        public Nullable<short> AlimentoCalorias { get; set; }
        [Display(Name = "Carbohidratos")]
        public Nullable<short> AlimentoCarbohidratos { get; set; }
        [Display(Name = "Azucares")]
        public Nullable<double> AlimentoAzucar { get; set; }
        [Display(Name = "Grasas")]
        public Nullable<double> AlimentoGrasa { get; set; }
        [Display(Name = "Sodio")]
        public Nullable<double> AlimentoSodio { get; set; }
        [Display(Name = "Gluten")]
        public Nullable<bool> AlimentoGluten { get; set; }

        [Display(Name = "Cantidad")]
        [Range(typeof(double), "0", "1000", ErrorMessage = "El valor debe ser mayor a {1}")]
        public double RelComAliCantidad { get; set; } = 0;
    }
}