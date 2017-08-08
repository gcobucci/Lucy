﻿using Foolproof;
using ModelCL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lucy.Models.Personas
{
    public class DatCliViewModel
    {
        [Key]
        public int Id { get; set; }


        /////Datos Personales/////
        [Required]
        [Display(Name = "Nombre")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Este campo debe tener un largo mínimo de {2} caracteres.")]
        public string PersonaNombre { get; set; }

        [Required]
        [Display(Name = "Apellido")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Este campo debe tener un largo mínimo de {2} caracteres.")]
        public string PersonaApellido { get; set; }

        [Required]
        [Display(Name = "Fecha de nacimiento")]
        public String PersonaFchNac { get; set; } = DateTime.Now.ToString();

        [Required]
        [Display(Name = "Sexo")]
        public short SexoId { get; set; }


        /////Datos clinicos/////
        [Display(Name = "Peso")]
        //Ver como restringir el float
        public Nullable<double> DatCliPeso { get; set; }

        [Display(Name = "Altura")]
        public Nullable<double> DatCliAltura { get; set; }


        /////Enfermedades/////
        public List<ViewModelCheckBox> Enfermedades { get; set; }

        [Display(Name = "Tipo")]
        public string DiabetesTipo { get; set; }

        //Tipo 1//
        [RequiredIf("DiabetesTipo", "1")]
        [Range(typeof(double), "0,4", "1", ErrorMessage = "El valor debe estar entre {1} y {2}")]
        [Display(Name = "Valor normal mínimo de glicemia")]
        public double DiabetesGlicemiaBaja { get; set; }

        [RequiredIf("DiabetesTipo", "1")]
        [Range(typeof(double), "1", "1,8", ErrorMessage = "El valor debe estar entre {1} y {2}")]
        [Display(Name = "Valor normal máximo de glicemia")]
        public double DiabetesGlicemiaAlta { get; set; }

        [RequiredIf("DiabetesTipo", "1")]
        [Range(5, 30, ErrorMessage = "El campo valor debe estar entre {1} y {2}")]
        [Display(Name = "Hidratos por unidad de insulina")]
        public short DiabetesHidratosPorUniInsu { get; set; }

        [RequiredIf("DiabetesTipo", "1")]
        [Display(Name = "Insulina retardada")]
        public long InsulinaRetardadaId { get; set; }

        [RequiredIf("DiabetesTipo", "1")]
        [Display(Name = "Insulina de corrección")]
        public long InsulinaCorreccionId { get; set; }


        /////Medicina/////
    }

    public class ViewModelCheckBox
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
        public bool Checked { get; set; } = false;
    }

    public class ViewModelSelectList
    {
        public short Id { get; set; }
        public string Valor { get; set; }
    }
}