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
    [RoutePrefix("registros/control")]
    public class RegControlController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        [Route("listado")]
        public ActionResult Index()
        {
            long idPer = Convert.ToInt64(Request.Cookies["cookiePer"]["PerId"]);

            List<ModelCL.Registro> regControl = db.Registro.Where(r => r.Control != null && r.Persona.PersonaId == idPer).OrderByDescending(r => r.RegistroFchHora).ToList();

            return View(regControl);
        }

        [Route("crear")]
        public ActionResult Create()
        {
            long idPer = Convert.ToInt64(Request.Cookies["cookiePer"]["PerId"]);

            ModelCL.Persona persona = db.Persona.Find(idPer);
            List<ModelCL.RelPerEnf> lrpe = persona.RelPerEnf.ToList();

            List<ModelCL.Enfermedad> lEnfermedades = new List<ModelCL.Enfermedad>();

            foreach (var rpe in lrpe)
            {
                lEnfermedades.Add(rpe.Enfermedad);
            }

            ModelCL.Enfermedad enfermedad = new ModelCL.Enfermedad();
            enfermedad.EnfermedadId = 0;
            enfermedad.EnfermedadNombre = ">> Mostrar todos <<";

            lEnfermedades.Add(enfermedad);
            ViewBag.lEnfermedades = new SelectList(lEnfermedades/*.OrderByDescending(e => e.EnfermedadId)*/, "EnfermedadId", "EnfermedadNombre");
            return View();
        }

        [Route("_crear")]
        public PartialViewResult _Create(long idEnf)
        {
            //long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);

            long idPer = Convert.ToInt64(Request.Cookies["cookiePer"]["PerId"]);


            RegControlViewModel newRegControl = new RegControlViewModel();
            newRegControl.PersonaId = idPer;
            newRegControl.EnfermedadId = idEnf;

            if (idEnf == 0)
            {
                List<ModelCL.Valor> lValores = db.Valor.ToList();
                ViewBag.lValores = new SelectList(lValores, "ValorId", "ValorNombre");
            }
            else
            {
                List<ModelCL.Valor> lValores = db.Valor.Where(v => v.Enfermedad.Where(e => e.EnfermedadId == newRegControl.EnfermedadId).FirstOrDefault() != null).ToList();
                ViewBag.lValores = new SelectList(lValores, "ValorId", "ValorNombre");
            }
                        
            return PartialView(newRegControl);
        }

        [HttpPost]
        [Route("_crear")]
        [ValidateAntiForgeryToken]
        public ActionResult _Create(RegControlViewModel datos)
        {
            if (ModelState.IsValid)
            {
                ModelCL.Persona Persona = db.Persona.Find(datos.PersonaId);

                ModelCL.Registro regControl = new ModelCL.Registro();
                regControl.RegistroFchHora = Convert.ToDateTime(datos.RegistroFchHora);

                ModelCL.Control control = new ModelCL.Control();

                control.ValorId = datos.ValorId;
                control.ControlValor = datos.ControlValor;

                regControl.Control = control;

                Persona.Registro.Add(regControl);

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            if (datos.EnfermedadId == 0)
            {
                List<ModelCL.Valor> lValores = db.Valor.ToList();
                ViewBag.lValores = new SelectList(lValores, "ValorId", "ValorNombre");
            }
            else
            {
                List<ModelCL.Valor> lValores = db.Valor.Where(v => v.Enfermedad.Where(e => e.EnfermedadId == datos.EnfermedadId).FirstOrDefault() != null).ToList();
                ViewBag.lValores = new SelectList(lValores, "ValorId", "ValorNombre");
            }

            return View(datos);
        }

        [Route("editar")]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModelCL.Registro regControl = db.Registro.Where(r => r.RegistroId == id).FirstOrDefault();
            if (regControl.Control == null)
            {
                return HttpNotFound();
            }

            RegControlViewModel vmRegControl = new RegControlViewModel();
            vmRegControl.RegistroId = regControl.RegistroId;
            vmRegControl.PersonaId = regControl.PersonaId;
            vmRegControl.ValorId = regControl.Control.ValorId;
            vmRegControl.ValorNombre = regControl.Control.Valor.ValorNombre;
            vmRegControl.RegistroFchHora = regControl.RegistroFchHora.ToString();
            vmRegControl.ControlValor = regControl.Control.ControlValor;

            return View(vmRegControl);
        }

        [HttpPost]
        [Route("editar")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RegControlViewModel datos)
        {            
            if (ModelState.IsValid)
            {
                ModelCL.Registro regControl = db.Registro.Where(r => r.RegistroId == datos.RegistroId).FirstOrDefault();

                regControl.RegistroFchHora = Convert.ToDateTime(datos.RegistroFchHora);
                regControl.Control.ControlValor = datos.ControlValor;

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(datos);
        }

        [Route("eliminar")]
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModelCL.Registro regControl = db.Registro.Where(r => r.RegistroId == id).FirstOrDefault();
            if (regControl.Control == null)
            {
                return HttpNotFound();
            }
            return View(regControl);
        }

        [Route("eliminar")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ModelCL.Registro regControl = db.Registro.Where(r => r.RegistroId == id).FirstOrDefault();
            db.Registro.Remove(regControl);
            db.SaveChanges();
            return RedirectToAction("Index");
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
