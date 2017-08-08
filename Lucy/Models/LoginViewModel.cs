using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lucy.Models
{
    public class LoginViewModel
    {
        //[Key]
        //public int Id { get; set; }

        [Required]
        [StringLength(60)]
        [Display(Name = "Usuario o email")]
        public string LoginUsuario { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(20)]
        [Display(Name = "Contraseña")]
        public string LoginPass { get; set; }

        [Display(Name = "Recordarme")]
        public bool Recordarme { get; set; }
    }
}