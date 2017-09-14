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
    
    public partial class Persona
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Persona()
        {
            this.Datos = new HashSet<Datos>();
            this.Recordatorio = new HashSet<Recordatorio>();
            this.Registro = new HashSet<Registro>();
            this.RelPerEnf = new HashSet<RelPerEnf>();
            this.RelUsuPer = new HashSet<RelUsuPer>();
        }
    
        public long PersonaId { get; set; }
        public string PersonaNombre { get; set; }
        public string PersonaApellido { get; set; }
        public System.DateTime PersonaFchNac { get; set; }
        public System.DateTime PersonaFchIng { get; set; }
        public short SexoId { get; set; }

        public string nombreCompleto { get { return PersonaNombre + " " + PersonaApellido; } }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Datos> Datos { get; set; }
        public virtual Sexo Sexo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Recordatorio> Recordatorio { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Registro> Registro { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RelPerEnf> RelPerEnf { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RelUsuPer> RelUsuPer { get; set; }
        public virtual Dieta Dieta { get; set; }
        public virtual Programa Programa { get; set; }
    }
}
