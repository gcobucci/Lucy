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
    
    public partial class Alimento
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Alimento()
        {
            this.Comida = new HashSet<Comida>();
        }
    
        public long AlimentoId { get; set; }
        public string AlimentoNombre { get; set; }
        public string AlimentoImagen { get; set; }
        public string AlimentoPorcion { get; set; }
        public Nullable<double> AlimentoCarbohidratos { get; set; }
        public Nullable<double> AlimentoCalorias { get; set; }
        public Nullable<double> AlimentoAzucar { get; set; }
        public Nullable<double> AlimentoGrasa { get; set; }
        public Nullable<double> AlimentoSodio { get; set; }
        public Nullable<bool> AlimentoGluten { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comida> Comida { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}
