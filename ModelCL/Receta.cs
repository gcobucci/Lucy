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
    
    public partial class Receta
    {
        public long RecetaId { get; set; }
        public Nullable<double> RecetaCalorias { get; set; }
        public Nullable<double> RecetaHidratos { get; set; }
        public Nullable<bool> RecetaSodio { get; set; }
        public Nullable<bool> RecetaGluten { get; set; }
    
        public virtual Contenido Contenido { get; set; }
    }
}
