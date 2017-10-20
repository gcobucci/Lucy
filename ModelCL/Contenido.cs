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
    using System.ComponentModel.DataAnnotations;

    public partial class Contenido
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Contenido()
        {
            this.Comentario = new HashSet<Comentario>();
            this.Multimedia = new HashSet<Multimedia>();
            this.UsuariosFav = new HashSet<Usuario>();
        }
    
        public long ContenidoId { get; set; }
        public string ContenidoTitulo { get; set; }
        public string ContenidoDescripcion { get; set; }
        [DataType(DataType.MultilineText)]
        public string ContenidoCuerpo { get; set; }
        public System.DateTime ContenidoFchHora { get; set; }
        public int ContenidoCantVisitas { get; set; }
    
        public virtual Articulo Articulo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comentario> Comentario { get; set; }
        public virtual Dieta Dieta { get; set; }
        public virtual Ejercicio Ejercicio { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Multimedia> Multimedia { get; set; }
        public virtual Programa Programa { get; set; }
        public virtual Receta Receta { get; set; }
        public virtual Rutina Rutina { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Usuario> UsuariosFav { get; set; }
        public virtual Usuario UsuarioAutor { get; set; }
    }
}
