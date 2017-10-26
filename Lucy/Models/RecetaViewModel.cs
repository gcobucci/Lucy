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
    public class RecetaViewModel
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


        [Display(Name = "Calorías")]
        [Range(0, 2000, ErrorMessage = "El valor debe estar entre {1} y {2}.")]
        public Nullable<short> RecetaCalorias { get; set; }

        [Display(Name = "Carbohidratos")]
        [Range(0, 2000, ErrorMessage = "El valor debe estar entre {1} y {2}.")]
        public Nullable<short> RecetaHidratos { get; set; }
                
        [Display(Name = "Sodio")]
        public Nullable<bool> RecetaSodio { get; set; }

        [Display(Name = "Gluten")]
        public Nullable<bool> RecetaGluten { get; set; }


        public Nullable<long> RecetaImagen1Id { get; set; }

        [Display(Name = "Imagen de portada")]
        public string RecetaImagen1Url { get; set; }

        public Nullable<long> RecetaImagen2Id { get; set; }

        [Display(Name = "Imagen de portada")]
        public string RecetaImagen2Url { get; set; }

        public Nullable<long> RecetaImagen3Id { get; set; }

        [Display(Name = "Imagen de portada")]
        public string RecetaImagen3Url { get; set; }

        public Nullable<long> RecetaImagen4Id { get; set; }

        [Display(Name = "Imagen de portada")]
        public string RecetaImagen4Url { get; set; }

        public Nullable<long> RecetaImagen5Id { get; set; }

        [Display(Name = "Imagen de portada")]
        public string RecetaImagen5Url { get; set; }
    }
}