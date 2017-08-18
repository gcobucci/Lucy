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
    
    public partial class Comentario
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Comentario()
        {
            this.ComentarioHijo = new HashSet<Comentario>();
            this.UsuariosFav = new HashSet<Usuario>();
        }
    
        public long ComentarioId { get; set; }
        public long UsuarioId { get; set; }
        public long ContenidoId { get; set; }
        public string ComentarioCuerpo { get; set; }
        public System.DateTime ComentarioFchHora { get; set; }
    
        public virtual Contenido Contenido { get; set; }
        public virtual Usuario UsuarioAutor { get; set; }
        public virtual Comentario ComentarioPadre { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comentario> ComentarioHijo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Usuario> UsuariosFav { get; set; }
    }
}
