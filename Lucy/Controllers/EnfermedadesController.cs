using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelCL;
using System.Web.Security;
using System.Net;
using System.IO;
using Lucy.Models;

namespace Lucy.Controllers
{
    [RoutePrefix("enfermedades")]
    public class EnfermedadesController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        [Authorize]
        [Route("crear")]
        public ActionResult Create()
        {
            EnfermedadViewModel newEnfermedad = new EnfermedadViewModel();
            
            return View(newEnfermedad);
        }

        [Authorize]
        [HttpPost]
        [Route("crear")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EnfermedadViewModel datos)
        {
            if (ModelState.IsValid)
            {
                ModelCL.Enfermedad newEnf = new ModelCL.Enfermedad();
                newEnf.EnfermedadNombre = datos.EnfermedadNombre;
                newEnf.EnfermedadDesc = datos.EnfermedadDesc;

                long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);
                newEnf.Usuario = db.Usuario.Find(idUsu);
                
                db.Enfermedad.Add(newEnf);
                db.SaveChanges();
                return RedirectToAction("Datos_Clinicos", "Personas");
            }

            ViewBag.ErrorMessage = "Error inesperado";
            return View(datos);
        }

        [Authorize]
        [Route("editar")]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);
            ModelCL.Enfermedad oldEnfermedad = db.Enfermedad.Find(id);
            if (oldEnfermedad == null || oldEnfermedad.Usuario.UsuarioId != idUsu)
            {
                return HttpNotFound();
            }

            EnfermedadViewModel enfermedad = new EnfermedadViewModel();

            enfermedad.EnfermedadId = oldEnfermedad.EnfermedadId;
            enfermedad.EnfermedadNombre = oldEnfermedad.EnfermedadNombre;
            enfermedad.EnfermedadDesc = oldEnfermedad.EnfermedadDesc;

            return View(enfermedad);
        }

        [Authorize]
        [HttpPost]
        [Route("editar")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EnfermedadViewModel datos)
        {
            if (ModelState.IsValid)
            {
                ModelCL.Enfermedad enfermedad = db.Enfermedad.Find(datos.EnfermedadId);

                enfermedad.EnfermedadNombre = datos.EnfermedadNombre;
                enfermedad.EnfermedadDesc = datos.EnfermedadDesc;
                                
                db.SaveChanges();
                return RedirectToAction("Datos_Clinicos", "Personas");
            }

            ViewBag.ErrorMessage = "Error inesperado";
            return View(datos);
        }

        [Authorize]
        [Route("eliminar")]
        //[ValidateAntiForgeryToken]
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);
            ModelCL.Enfermedad enfermedad = db.Enfermedad.Where(r => r.EnfermedadId == id).FirstOrDefault();
            if (enfermedad == null || enfermedad.Usuario.UsuarioId != idUsu)
            {
                return HttpNotFound();
            }

            db.Enfermedad.Remove(enfermedad);
            db.SaveChanges();

            return RedirectToAction("Datos_Clinicos", "Personas");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}