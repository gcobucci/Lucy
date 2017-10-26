using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelCL;
using System.Web.Security;
using System.Data;
using System.Data.Entity;
using System.Net;
using System.IO;

namespace Lucy.Controllers
{
    [RoutePrefix("articulos")]
    public class ArticulosController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        [Route("listado")]
        public ActionResult Index()
        {
            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);
            ViewBag.idUsu = idUsu;

            var articulos = db.Contenido.Where(c => c.Articulo != null).ToList();

            return View(articulos);
        }

        [Route("ver")]
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModelCL.Contenido contArticulo = db.Contenido.Find(id);
            if (contArticulo.Articulo == null)
            {
                return HttpNotFound();
            }

            contArticulo.ContenidoCantVisitas += 1;
            db.SaveChanges();

            return View(contArticulo);
        }
    }
}
