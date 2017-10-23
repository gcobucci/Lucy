using Foolproof;
using ModelCL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Backend.Models
{
    public class EmailViewModel
    {
        //[Key]
        //public int Id { get; set; }

        [Required]
        [Display(Name = "Filtrar destinatarios por enfermedad")]
        public long EnfermedadId { get; set; }

        [Required]
        [Display(Name = "Respetar a los usuarios que no desean recibir emails (solo desmarcar si es importante)")]
        public bool RespetarDecisión { get; set; } = true;

        [Required]
        [Display(Name = "Asunto")]
        public string Asunto { get; set; }

        [Required]
        [Display(Name = "Mensaje")]
        [DataType(DataType.MultilineText)]
        public string Mensaje { get; set; }
    }
}