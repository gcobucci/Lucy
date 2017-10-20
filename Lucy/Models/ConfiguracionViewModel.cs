using ModelCL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lucy.Models
{
    //public class ConfiguracionDatosGeneralesViewModel
    //{
    //    //[Key]
    //    //public int Id { get; set; }

    //    [Required]
    //    [Display(Name = "País de residencia")]
    //    public string UsuarioPais { get; set; }
    //}



    //[MetadataType(typeof(ConfiguracionContraseñaViewModel))]
    //public partial class UsurioConfiguracionPC
    //{
    //    public string UsuarioPassConfirmacion { get; set; }
    //}

    //public class ConfiguracionContraseñaViewModel
    //{
    //    //[Key]
    //    //public int Id { get; set; }

    //    [Required]
    //    [DataType(DataType.Password)]
    //    [StringLength(20, MinimumLength = 6, ErrorMessage = "Este campo debe tener un largo mínimo de {2} caracteres.")]
    //    [Display(Name = "Contraseña anterior")]
    //    public string UsuarioOldPass { get; set; }

    //    [Required]
    //    [DataType(DataType.Password)]
    //    [StringLength(20, MinimumLength = 6, ErrorMessage = "Este campo debe tener un largo mínimo de {2} caracteres.")]
    //    [Display(Name = "Nueva contraseña")]
    //    public string UsuarioPass { get; set; }

    //    [Required]
    //    [DataType(DataType.Password)]
    //    [StringLength(20, MinimumLength = 6, ErrorMessage = "Este campo debe tener un largo mínimo de {2} caracteres.")]
    //    [Compare("UsuarioPass", ErrorMessage = "Las contraseñas no coinciden")]
    //    [Display(Name = "Confirmar contraseña")]
    //    public string UsuarioPassConfirmacion { get; set; }
    //}



    //public class ConfiguracionEmailViewModel
    //{
    //    //[Key]
    //    //public int Id { get; set; }

    //    [Required]
    //    [EmailAddress]
    //    [StringLength(60)]
    //    [Display(Name = "Nuevo email")]
    //    public string UsuarioEmail { get; set; }

    //    [Required]
    //    [DataType(DataType.Password)]
    //    [StringLength(20, MinimumLength = 6, ErrorMessage = "Este campo debe tener un largo mínimo de {2} caracteres.")]
    //    [Display(Name = "Contraseña (para verificar que es usted)")]
    //    public string UsuarioPass { get; set; }
    //}
    
    
    
    public class ConfiguracionPremiumViewModel
    {
        //[Key]
        //public int Id { get; set; }

        

    }



    public class ConfiguracionNotificacionesViewModel
    {
        //[Key]
        //public int Id { get; set; }

        
        [Display(Name = "Recibir emails de la aplicación")]
        public bool UsuarioRecibirEmails { get; set; }
    }
}