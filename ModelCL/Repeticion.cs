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
    
    public partial class Repeticion
    {
        public long RepeticionId { get; set; }
        public long RecordatorioId { get; set; }
        public long PersonaId { get; set; }
        public System.DateTime RepeticionFchHora { get; set; }
        public short RepeticionNum { get; set; }
    
        public virtual Recordatorio Recordatorio { get; set; }
    }
}
