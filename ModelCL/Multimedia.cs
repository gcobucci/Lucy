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
    
    public partial class Multimedia
    {
        public long MultimediaId { get; set; }
        public long ContenidoId { get; set; }
        public string MultimediaUrl { get; set; }
        public string MultimediaTipo { get; set; }
        public short MultimediaOrden { get; set; }
    
        public virtual Contenido Contenido { get; set; }
    }
}
