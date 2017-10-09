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
    public class RegComidaViewModel
    {
        //public RegComidaViewModel()
        //{
        //    Alimentos = new List<ComidaAlimentoViewModel>();
        //}

        [Key]
        public long RegistroId { get; set; }

        //[Key]
        //public long PersonaId { get; set; }


        [Required]
        //[DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [Display(Name = "Fecha")]
        public string RegistroFchHora { get; set; } = DateTime.Now.ToString();

        [Required]
        [Display(Name = "Nombre del platillo/descriptivo")]
        public string ComidaPlatillo { get; set; }

        [Required]
        [Display(Name = "Comida")]
        public string ComidaComida { get; set; }

        [Display(Name = "Alimentos")]
        [UIHint("ComidaAlimentoViewModel")]
        public List<ComidaAlimentoViewModel> Alimentos { get; set; } = new List<ComidaAlimentoViewModel>();

        [Display(Name = "Calorías")]
        public Nullable<short> ComidaCalorias { get; set; }

        [Display(Name = "Carbohidratos")]
        public Nullable<short> ComidaCarbohidratos { get; set; }

        [Display(Name = "Azucares")]
        public Nullable<double> ComidaAzucar { get; set; }

        [Display(Name = "Grasas")]
        public Nullable<double> ComidaGrasa { get; set; }

        [Display(Name = "Sodio")]
        public Nullable<double> ComidaSodio { get; set; }

        [Display(Name = "Gluten")]
        public Nullable<bool> ComidaGluten { get; set; }
    }

    public class ComidaAlimentoViewModel
    {
        [Key]
        public long AlimentoId { get; set; }

        [Display(Name = "Alimento")]
        public string AlimentoNombre { get; set; }
        [Display(Name = "Calorías")]
        public Nullable<short> AlimentoCalorias { get; set; }
        [Display(Name = "Carbohidratos")]
        public Nullable<short> AlimentoCarbohidratos { get; set; }
        [Display(Name = "Azucares")]
        public Nullable<double> AlimentoAzucar { get; set; }
        [Display(Name = "Grasas")]
        public Nullable<double> AlimentoGrasa { get; set; }
        [Display(Name = "Sodio")]
        public Nullable<double> AlimentoSodio { get; set; }
        [Display(Name = "Gluten")]
        public Nullable<bool> AlimentoGluten { get; set; }

        [Display(Name = "Cantidad")]
        public double RelComAliCantidad { get; set; } = 0;
    }
}
