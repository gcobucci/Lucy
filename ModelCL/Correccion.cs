//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ModelCL
{
    using System;
    using System.Collections.Generic;
    
    public partial class Correccion
    {
        public long CorreccionId { get; set; }
        public long PersonaId { get; set; }
        public long MedicinaId { get; set; }
    
        public virtual Medicacion Medicacion { get; set; }
        public virtual Comida Comida { get; set; }
    }
}
