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
    
    public partial class Comida
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Comida()
        {
            this.RelComAli = new HashSet<RelComAli>();
        }
    
        public long ComidaId { get; set; }
        public long PersonaId { get; set; }
        public string ComidaPlatillo { get; set; }
        public string ComidaComida { get; set; }
        public Nullable<short> ComidaCalorias { get; set; }
        public Nullable<short> ComidaCarbohidratos { get; set; }
        public Nullable<double> ComidaAzucar { get; set; }
        public Nullable<double> ComidaGrasa { get; set; }
        public Nullable<double> ComidaSodio { get; set; }
        public Nullable<bool> ComidaGluten { get; set; }
    
        public virtual Registro Registro { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RelComAli> RelComAli { get; set; }
    }
}
