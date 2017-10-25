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
    public class EnfermedadViewModel
    {
        [Key]
        public long EnfermedadId { get; set; }

        [Required]
        [Display(Name = "Nombre")]
        public string EnfermedadNombre { get; set; }

        [Display(Name = "Descripcion")]
        public string EnfermedadDesc { get; set; }
    }
}