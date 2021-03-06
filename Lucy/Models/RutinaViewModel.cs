﻿using Foolproof;
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
    public class RutinaViewModel
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

        /////Ejercicios/////
        public List<Fachada.ViewModelCheckBox> Ejercicios { get; set; }
    }
}