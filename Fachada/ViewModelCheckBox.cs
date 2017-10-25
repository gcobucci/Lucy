using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fachada
{
    public class ViewModelCheckBox
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
        public bool Checked { get; set; } = false;

        //Enfermedad
        public Nullable<long> UsuarioId { get; set; } = null;
    }
}
