using Foolproof;
using ModelCL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lucy.Models
{
    public class ComentarioViewModel
    {
        //[Key]
        //public long ComentarioId { get; set; }


        public long ContenidoId { get; set; }

        public Nullable<long> ComentarioPadreId { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string ComentarioCuerpo { get; set; }
    }
}