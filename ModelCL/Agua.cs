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
    
    public partial class Agua
    {
        public long AguaId { get; set; }
        public long PersonaId { get; set; }
        public double AguaCantidad { get; set; }
    
        public virtual Registro Registro { get; set; }
    }
}
