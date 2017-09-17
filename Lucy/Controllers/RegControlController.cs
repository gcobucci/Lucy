using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelCL;
using System.Web.Security;

namespace Lucy.Controllers
{
    [Authorize]
    [RoutePrefix("registros/controles")]
    public class RegControlController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        [Route("index")]
        public ActionResult Index()
        {
            //long idPer = Fachada.Functions.get_idPer(Request.Cookies[FormsAuthentication.FormsCookieName]);
            long idPer = 1;

            List<ModelCL.Registro> registrosControl = db.Registro.Where(r => r.Control != null && (r.Persona.PersonaId == idPer)).ToList();

            return View(registrosControl);
        }

        //[Route("details")]
        //public ActionResult Details(long? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Control control = db.Control.Find(id);
        //    if (control == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(control);
        //}

        [Route("create")]
        public ActionResult Create()
        {
            ViewBag.ControlId = new SelectList(db.Registro, "RegistroId", "RegistroId");
            ViewBag.EnfermedadId = new SelectList(db.RelEnfVal, "EnfermedadId", "EnfermedadId");
            return View();
        }

        [HttpPost]
        [Route("create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ControlId,PersonaId,EnfermedadId,ValorId,ControlValor")] Control control)
        {
            if (ModelState.IsValid)
            {
                db.Control.Add(control);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ControlId = new SelectList(db.Registro, "RegistroId", "RegistroId", control.ControlId);
            ViewBag.EnfermedadId = new SelectList(db.RelEnfVal, "EnfermedadId", "EnfermedadId", control.EnfermedadId);
            return View(control);
        }

        [Route("edit")]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Control control = db.Control.Find(id);
            if (control == null)
            {
                return HttpNotFound();
            }
            ViewBag.ControlId = new SelectList(db.Registro, "RegistroId", "RegistroId", control.ControlId);
            ViewBag.EnfermedadId = new SelectList(db.RelEnfVal, "EnfermedadId", "EnfermedadId", control.EnfermedadId);
            return View(control);
        }

        [HttpPost]
        [Route("edit")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ControlId,PersonaId,EnfermedadId,ValorId,ControlValor")] Control control)
        {
            if (ModelState.IsValid)
            {
                db.Entry(control).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ControlId = new SelectList(db.Registro, "RegistroId", "RegistroId", control.ControlId);
            ViewBag.EnfermedadId = new SelectList(db.RelEnfVal, "EnfermedadId", "EnfermedadId", control.EnfermedadId);
            return View(control);
        }

        [Route("delete")]
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Control control = db.Control.Find(id);
            if (control == null)
            {
                return HttpNotFound();
            }
            return View(control);
        }

        [Route("delete")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Control control = db.Control.Find(id);
            db.Control.Remove(control);
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
