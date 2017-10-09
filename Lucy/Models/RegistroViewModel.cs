using ModelCL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lucy.Models
{
    [MetadataType(typeof(RegistroViewModel))]
    public partial class UsurioPC
    {
        public string UsuarioPassConfirmacion { get; set; }
    }

    public class RegistroViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "Este campo debe tener un largo mínimo de {2} caracteres.")]
        [Display(Name = "Nombre de usuario")]
        public string UsuarioNombre { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(60)]
        [Display(Name = "Email")]
        public string UsuarioEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Este campo debe tener un largo mínimo de {2} caracteres.")]
        [Display(Name = "Contraseña")]
        public string UsuarioPass { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Este campo debe tener un largo mínimo de {2} caracteres.")]
        [Compare("UsuarioPass", ErrorMessage = "Las contraseñas no coinciden")]
        [Display(Name = "Confirmar contraseña")]
        public string UsuarioPassConfirmacion { get; set; }
                
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

        [Required]
        [Display(Name = "País de residencia")]
        public string UsuarioPais { get; set; }
    }
}