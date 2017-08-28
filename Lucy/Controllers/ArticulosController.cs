using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelCL;
using System.Web.Security;

namespace Lucy.Controllers
{
    public class ArticulosController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        // GET: Articulos
        [Authorize]
        [HttpGet]
        public ActionResult List()
        {
            
            List<ModelCL.Articulo> Articulos = null;
            Articulos = db.Articulo.ToList();
            return View(Articulos);

        }
    }
}
