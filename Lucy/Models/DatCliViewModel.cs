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
        public string PersonaFchNac { get; set; } = DateTime.Now.ToString();

        [Required]
        [Display(Name = "Sexo")]
        public short SexoId { get; set; }


        /////DatCli/////
        [Display(Name = "Nivel de actividad física")]
        public string DatCliNivelActividad { get; set; }

        /////Peso/////
        [Display(Name = "Peso (Kg)")]
        [Range(typeof(double), "1", "700", ErrorMessage = "El valor debe estar entre {1} y {2}")]
        public Nullable<double> PesoValor { get; set; }

        /////DatCli/////
        [Display(Name = "Altura (cm)")]
        [Range(30, 300, ErrorMessage = "El valor debe estar entre {1} y {2}")]
        public Nullable<short> DatCliAltura { get; set; }


        /////Enfermedades/////
        public List<Fachada.ViewModelCheckBox> Enfermedades { get; set; }

        [Range(5, 30, ErrorMessage = "El campo valor debe estar entre {1} y {2}")]
        [Display(Name = "Hidratos por unidad de insulina")]
        public Nullable<short> DiabetesHidratosPorUniInsu { get; set; }

        [Display(Name = "Insulina retardada")]
        public Nullable<long> InsulinaRetardadaId { get; set; }

        [Display(Name = "Insulina de corrección")]
        public Nullable<long> InsulinaCorreccionId { get; set; }


        /////Medicina/////
        public List<Fachada.ViewModelCheckBox> Medicinas { get; set; }
    }
}