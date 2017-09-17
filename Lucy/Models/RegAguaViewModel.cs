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
    public class RegAguaViewModel
    {
        [Key]
        public long RegistroId { get; set; }

        //[Key]
        //public long PersonaId { get; set; }

        [Required]
        //[DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [Display(Name = "Fecha")]
        public string RegistroFchHora { get; set; } = DateTime.Now.ToString();

        [Required]
        [Display(Name = "Cantidad (litros)")]
        [Range(typeof(double), "0,1", "20", ErrorMessage = "El valor debe estar entre {1} y {2}")]
        public double AguaCantidad { get; set; }
    }
}