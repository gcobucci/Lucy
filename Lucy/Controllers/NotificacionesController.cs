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
using System.Web.Security;

namespace Lucy.Controllers
{
    [Authorize]
    [RoutePrefix("notificaciones")]
    public class NotificacionesController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        [Route("listado")]
        public PartialViewResult _Index()
        {
            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);

            List<ModelCL.Notificacion> notificaciones = db.Notificacion.Where(n => n.UsuarioId == idUsu).OrderByDescending(r => r.NotificacionFchHora).ToList();

            foreach (ModelCL.Notificacion not in notificaciones)
            {
                not.NotificacionVista = true;
            }
            db.SaveChanges();

            return PartialView(notificaciones);
        }

        [Route("eliminar")]
        //[ValidateAntiForgeryToken]
        public ActionResult Delete(long? id/*, string url*/)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ModelCL.Notificacion notificacion = db.Notificacion.Where(r => r.NotificacionId == id).FirstOrDefault();
            if (notificacion == null)
            {
                return HttpNotFound();
            }

            db.Notificacion.Remove(notificacion);
            db.SaveChanges();

            return null;
            //return Redirect(url);
        }
    }
}
