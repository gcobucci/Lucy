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
    
    public partial class Medicina
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Medicina()
        {
            this.Medicacion = new HashSet<Medicacion>();
            this.RelMedRelPerEnf = new HashSet<RelMedRelPerEnf>();
            this.RelMedVal = new HashSet<RelMedVal>();
            this.Enfermedad = new HashSet<Enfermedad>();
            this.Recordatorio = new HashSet<Recordatorio>();
        }
    
        public long MedicinaId { get; set; }
        public string MedicinaNombre { get; set; }
        public bool MedicinaGeneral { get; set; }
        public string MedicinaTipo { get; set; }
        public string MedicinaDesc { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Medicacion> Medicacion { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RelMedRelPerEnf> RelMedRelPerEnf { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RelMedVal> RelMedVal { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Enfermedad> Enfermedad { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Recordatorio> Recordatorio { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}
