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
    public class RegMedicacionViewModel
    {
        [Key]
        public long RegistroId { get; set; }

        [Key]
        public long PersonaId { get; set; }

        [Required]
        //[DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [Display(Name = "Fecha")]
        public string RegistroFchHora { get; set; } = DateTime.Now.ToString();

        public Nullable<long> EnfermedadId { get; set; }

        public string EnfermedadNombre { get; set; }

        [Required]
        [Display(Name = "Medicina")]
        public long MedicinaId { get; set; }

        public string MedicinaNombre { get; set; }

        [Required]
        [Display(Name = "Presentacion de la medicina")]
        public short PresentacionId { get; set; }

        [Required]
        [Display(Name = "Cantidad")]
        [Range(typeof(double), "1", "100000", ErrorMessage = "El valor debe ser mayor a {1}")]
        public double MedicacionCantidad { get; set; }
    }
}