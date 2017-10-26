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
    public class EjercicioViewModel
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
        [Display(Name = "Tipo")]
        public string EjercicioTipo { get; set; }

        [Required]
        [Display(Name = "Categoría")]
        public string EjercicioCategoria{ get; set; }

        [Display(Name = "Calorías por minuto")]
        [Range(0, 2000, ErrorMessage = "El valor debe estar entre {1} y {2}.")]
        public Nullable<short> EjercicioCaloriasPorMinuto { get; set; }


        public Nullable<long> EjercicioImagen1Id { get; set; }

        [Display(Name = "Imagen de portada")]
        public string EjercicioImagen1Url { get; set; }

        public Nullable<long> EjercicioImagen2Id { get; set; }

        [Display(Name = "Imagen de portada")]
        public string EjercicioImagen2Url { get; set; }

        public Nullable<long> EjercicioImagen3Id { get; set; }

        [Display(Name = "Imagen de portada")]
        public string EjercicioImagen3Url { get; set; }

        public Nullable<long> EjercicioImagen4Id { get; set; }

        [Display(Name = "Imagen de portada")]
        public string EjercicioImagen4Url { get; set; }

        public Nullable<long> EjercicioImagen5Id { get; set; }

        [Display(Name = "Imagen de portada")]
        public string EjercicioImagen5Url { get; set; }
    }
}