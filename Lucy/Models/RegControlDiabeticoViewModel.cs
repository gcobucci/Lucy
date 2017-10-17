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
    public class RegControlDiabeticoViewModel
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
        [Display(Name = "Fecha y hora")]
        public string RegistroFchHora { get; set; } = DateTime.Now.ToString();


        [Required]
        [Display(Name = "Resultado del control")]
        [Range(typeof(double), "0,1", "10", ErrorMessage = "El valor debe estar entre {1} y {2}.")]
        public double ControlValor { get; set; }


        //Registro de Comida//
        [Display(Name = "Registrar comida")]
        public bool RegistrarComida { get; set; }
                
        [RequiredIf("RegistrarComida", true)]
        [Display(Name = "Nombre del platillo/descriptivo")]
        public string ComidaPlatillo { get; set; }

        [RequiredIf("RegistrarComida", true)]
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


        //Resultados//
        [Display(Name = "Cantidad de insulina estimada por corrección del control")]
        public string ResultadoInsulinaPorCorreccion { get; set; }

        [Display(Name = "Cantidad de insulina estimada por cantidad de hidratos a ingerir")]
        public string ResultadoInsulinaPorHidratos { get; set; }

        public string ResultadoTotalMensaje { get; set; }

        [Range(typeof(double), "0,1", "20", ErrorMessage = "El valor debe estar entre {1} y {2}.")]
        public Nullable<double> ResultadoTotalInsulinaCorreccion { get; set; } //La presentacion siempre va a ser en unidades a travez de inyección en este caso
    }
}
