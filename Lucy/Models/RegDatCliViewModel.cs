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
    public class RegDatCliViewModel
    {
        [Key]
        public long RegistroId { get; set; }

        //[Key]
        //public long PersonaId { get; set; }

        [Required]
        [Display(Name = "Fecha")]
        public string RegistroFchHora { get; set; } = DateTime.Now.ToString();

        //[RequiredIf("DatCliColesterol", null)]
        [Display(Name = "Altura (centimetros)")]
        [Range(30, 300, ErrorMessage = "El valor debe estar entre {1} y {2}")]
        public Nullable<short> DatCliAltura { get; set; }

        //[RequiredIf("DatCliAltura", null)]
        [Display(Name = "Colesterol total (mg/dL)")]
        [Range(50, 400, ErrorMessage = "El valor debe estar entre {1} y {2}")]
        public Nullable<short> DatCliColesterol { get; set; }
    }
}