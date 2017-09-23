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
    public class RegActividadViewModel
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
        [Display(Name = "Actividad/Ejercicio")]
        public long EjercicioId { get; set; }

        [Required]
        [Display(Name = "Tiempo (minutos)")]
        [Range(1, 720, ErrorMessage = "El valor debe estar entre {1} y {2}")]
        public short ActividadTiempo { get; set; }
    }
}