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
    public class CalcTMBViewModel
    {
        [Key]
        public int Id { get; set; }


        [Display(Name = "Peso (Kg)")]
        [Range(typeof(double), "1", "700", ErrorMessage = "El valor debe estar entre {1} y {2}")]
        public double Peso { get; set; }

        [Display(Name = "Altura (centimetros)")]
        [Range(30, 300, ErrorMessage = "El valor debe estar entre {1} y {2}")]
        public short Altura { get; set; }

        [Display(Name = "Edad")]
        [Range(3, 150, ErrorMessage = "El valor debe estar entre {1} y {2}")]
        public short Edad { get; set; }

        [Required]
        [Display(Name = "Sexo")]
        public short Sexo { get; set; }

        [Required]
        [Display(Name = "Nivel de actividad física")]
        public string NivelActividad { get; set; }
    }
}