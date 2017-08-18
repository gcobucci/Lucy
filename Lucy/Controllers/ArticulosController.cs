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
        // GET: Articulos
        [Authorize]
        [HttpGet]
        public ActionResult Articulos()
        {
            List<ModelCL.Articulo> Articulos = null;

            using (AgustinaEntities db = new AgustinaEntities())
            {

                Articulos = db.Articulo.ToList();

            }

            return View(Articulos);
        }
    }
}