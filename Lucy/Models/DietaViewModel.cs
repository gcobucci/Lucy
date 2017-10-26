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
    public class DietaViewModel
    {
        [Key]
        public long ContenidoId { get; set; }

        [Required]
        [Display(Name = "Título")]
        public string ContenidoTitulo { get; set; }

        [Display(Name = "Descripción")]
        public string ContenidoDescripcion { get; set; }

        [Required]
        [Display(Name = "Cuerpo")]
        [DataType(DataType.MultilineText)]
        public string ContenidoCuerpo { get; set; }


        [Required]
        [Display(Name = "Calorías en el desayuno")]
        [Range(0, 2000, ErrorMessage = "El valor debe estar entre {1} y {2}.")]
        public short DietaDesayunoCalorias { get; set; }

        [Required]
        [Display(Name = "Descripción del desayuno")]
        public string DietaDesayunoDescripcion { get; set; }


        [Required]
        [Display(Name = "Calorías en el almuerzo")]
        [Range(0, 2000, ErrorMessage = "El valor debe estar entre {1} y {2}.")]
        public short DietaAlmuerzoCalorias { get; set; }

        [Required]
        [Display(Name = "Descripción del almuerzo")]
        public string DietaAlmuerzoDescripcion { get; set; }


        [Required]
        [Display(Name = "Calorías en la merienda")]
        [Range(0, 2000, ErrorMessage = "El valor debe estar entre {1} y {2}.")]
        public short DietaMeriendaCalorias { get; set; }

        [Required]
        [Display(Name = "Descripción de la merienda")]
        public string DietaMeriendaDescripcion { get; set; }


        [Required]
        [Display(Name = "Calorías en la cena")]
        [Range(0, 2000, ErrorMessage = "El valor debe estar entre {1} y {2}.")]
        public short DietaCenaCalorias { get; set; }

        [Required]
        [Display(Name = "Descripción de la cena")]
        public string DietaCenaDescripcion { get; set; }


        [Required]
        [Display(Name = "Calorías en las ingestas")]
        [Range(0, 2000, ErrorMessage = "El valor debe estar entre {1} y {2}.")]
        public short DietaIngestasCalorias { get; set; }

        [Required]
        [Display(Name = "Descripción de las ingestas")]
        public string DietaIngestasDescripcion { get; set; }
    }
}