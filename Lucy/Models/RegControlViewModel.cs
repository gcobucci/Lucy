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
    public class RegControlViewModel
    {
        [Key]
        public long RegistroId { get; set; }

        [Key]
        public long PersonaId { get; set; }

        public Nullable<long> EnfermedadId { get; set; }

        [Required]
        //[DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [Display(Name = "Fecha y hora")]
        public string RegistroFchHora { get; set; } = DateTime.Now.ToString();

        [Required]
        [Display(Name = "Valor")]
        public long ValorId { get; set; }

        public string ValorNombre { get; set; }

        [Required]
        [Display(Name = "Resultado del control")]
        //[Range(typeof(double), "1", "100000", ErrorMessage = "El valor debe ser mayor a {1}")]
        public double ControlValor { get; set; }
    }
}