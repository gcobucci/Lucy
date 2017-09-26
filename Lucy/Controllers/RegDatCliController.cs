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
    [RoutePrefix("registros/datos_generales")]
    public class RegDatCliController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        [Route("index")]
        public ActionResult Index()
        {
            //long idPer = Fachada.Functions.get_idPer(Request.Cookies[FormsAuthentication.FormsCookieName]);
            long idPer = 1;

            List<ModelCL.Registro> registrosDatCli = db.Registro.Where(r => r.DatCli != null && (r.Persona.PersonaId == idPer)).OrderByDescending(r => r.RegistroFchHora).ToList();

            return View(registrosDatCli);
        }

        [Route("create")]
        public ActionResult Create()
        {
            RegDatCliViewModel newRegDatCli = new RegDatCliViewModel();
            return View(newRegDatCli);
        }

        [HttpPost]
        [Route("create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RegDatCliViewModel datos)
        {
            if (ModelState.IsValid)
            {
                if (datos.DatCliAltura == null && datos.DatCliColesterol == null)
                {
                    ViewBag.ErrorMessage = "No puede dejar los dos campos de valor en blanco.";
                    return View(datos);
                }

                //long idPer = Fachada.Functions.get_idPer(Request.Cookies[FormsAuthentication.FormsCookieName]);
                long idPer = 1;

                DateTime f = Convert.ToDateTime(datos.RegistroFchHora);
                ModelCL.Registro regDatCliEx = db.Registro.Where(r => r.DatCli != null && r.Persona.PersonaId == idPer && r.RegistroFchHora == f).FirstOrDefault();

                if (regDatCliEx != null)
                {
                    ViewBag.ErrorMessage = "Ya existe un valor de datos generales registrado en esta fecha. Puede modificarlo si lo desea.";
                    return View(datos);
                }

                ModelCL.Persona Persona = db.Persona.Find(idPer);

                ModelCL.Registro regDatCli = new ModelCL.Registro();
                regDatCli.RegistroFchHora = Convert.ToDateTime(datos.RegistroFchHora);

                ModelCL.DatCli datCli = new ModelCL.DatCli();

                datCli.DatCliAltura = datos.DatCliAltura;
                datCli.DatCliColesterol = datos.DatCliColesterol;
                regDatCli.DatCli = datCli;

                Persona.Registro.Add(regDatCli);

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(datos);
        }

        [Route("edit")]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModelCL.Registro regDatCli = db.Registro.Where(r => r.RegistroId == id).FirstOrDefault();
            if (regDatCli.DatCli == null)
            {
                return HttpNotFound();
            }

            RegDatCliViewModel vmRegDatCli = new RegDatCliViewModel();
            vmRegDatCli.RegistroId = regDatCli.RegistroId;
            //vmRegDatCli.PersonaId = regDatCli.PersonaId;
            vmRegDatCli.RegistroFchHora = regDatCli.RegistroFchHora.ToString();
            vmRegDatCli.DatCliAltura = regDatCli.DatCli.DatCliAltura;
            vmRegDatCli.DatCliColesterol = regDatCli.DatCli.DatCliColesterol;

            return View(vmRegDatCli);
        }

        [HttpPost]
        [Route("edit")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RegDatCliViewModel datos)
        {
            if (ModelState.IsValid)
            {
                if (datos.DatCliAltura == null && datos.DatCliColesterol == null)
                {
                    ViewBag.ErrorMessage = "No puede dejar los dos campos de valor en blanco.";
                    return View(datos);
                }

                ModelCL.Registro regDatCli = db.Registro.Where(r => r.RegistroId == datos.RegistroId).FirstOrDefault();

                DateTime f = Convert.ToDateTime(datos.RegistroFchHora);
                if (regDatCli.RegistroFchHora != f)
                {                    
                    ModelCL.Registro regDatCliEx = db.Registro.Where(r => r.DatCli != null && r.Persona.PersonaId == regDatCli.PersonaId && r.RegistroFchHora == f).FirstOrDefault();

                    if (regDatCliEx != null)
                    {
                        ViewBag.ErrorMessage = "Ya existe un valor de datos generales registrado en esta fecha. Puede modificarlo si lo desea.";
                        return View(datos);
                    }
                }

                regDatCli.RegistroFchHora = f;
                regDatCli.DatCli.DatCliAltura = datos.DatCliAltura;
                regDatCli.DatCli.DatCliColesterol = datos.DatCliColesterol;

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(datos);
        }

        [Route("delete")]
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModelCL.Registro regDatCli = db.Registro.Where(r => r.RegistroId == id).FirstOrDefault();
            if (regDatCli.DatCli == null)
            {
                return HttpNotFound();
            }
            return View(regDatCli);
        }

        [Route("delete")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ModelCL.Registro regDatCli = db.Registro.Where(r => r.RegistroId == id).FirstOrDefault();
            db.Registro.Remove(regDatCli);
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
