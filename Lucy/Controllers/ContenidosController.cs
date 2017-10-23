using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelCL;
using Lucy.Models;

namespace Lucy.Controllers
{
    [Authorize]
    [RoutePrefix("contenidos")]
    public class ContenidosController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();               

        [Route("eliminar")]
        //[ValidateAntiForgeryToken]
        public ActionResult Delete(long? id, string url)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModelCL.Contenido contenido = db.Contenido.Where(r => r.ContenidoId == id).FirstOrDefault();

            if (contenido == null)
            {
                return HttpNotFound();
            }

            db.Contenido.Remove(contenido);
            db.SaveChanges();
            return Redirect(url);
        }
    }
}
