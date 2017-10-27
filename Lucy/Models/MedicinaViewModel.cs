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
    public class MedicinaViewModel
    {
        [Key]
        public long MedicinaId { get; set; }

        [Required]
        [Display(Name = "Nombre")]
        public string MedicinaNombre { get; set; }

        [Display(Name = "Descripción")]
        public string MedicinaDesc { get; set; }

        [Required]
        [Display(Name = "Tipo")]
        public string MedicinaTipo { get; set; }

        [Required]
        [Display(Name = "Medicina general")]
        public bool MedicinaGeneral { get; set; }

        /////Rutinas/////
        public List<Fachada.ViewModelCheckBox> Enfermedades { get; set; }
    }
}