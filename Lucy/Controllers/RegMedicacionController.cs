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
    [RoutePrefix("registros/medicacion")]
    public class RegMedicacionController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        [Route("listado")]
        public ActionResult Index()
        {
            long idPer = Convert.ToInt64(Request.Cookies["cookiePer"]["PerId"]);

            List<ModelCL.Registro> registrosMed = db.Registro.Where(r => r.Medicacion != null && r.Persona.PersonaId == idPer).OrderByDescending(r => r.RegistroFchHora).ToList();

            return View(registrosMed);
        }

        [Route("ver")]
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            long idPer = Convert.ToInt64(Request.Cookies["cookiePer"]["PerId"]);
            ModelCL.Registro regMedicacion = db.Registro.Where(r => r.RegistroId == id).FirstOrDefault();
            if (regMedicacion == null || regMedicacion.Medicacion == null || regMedicacion.Persona.PersonaId != idPer)
            {
                return HttpNotFound();
            }
            return View(regMedicacion);
        }

        [Route("crear")]
        public ActionResult Create(/*string ad = null*/)
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
            enfermedad.EnfermedadNombre = ">> Ninguna <<";

            lEnfermedades.Add(enfermedad);
            ViewBag.lEnfermedades = new SelectList(lEnfermedades/*.OrderByDescending(e => e.EnfermedadId)*/, "EnfermedadId", "EnfermedadNombre");



            return View();
        }

        [Route("_crear")]
        public PartialViewResult _Create(long idEnf)
        {
            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);

            long idPer = Convert.ToInt64(Request.Cookies["cookiePer"]["PerId"]);


            RegMedicacionViewModel newRegMed = new RegMedicacionViewModel();
            newRegMed.PersonaId = idPer;
            newRegMed.EnfermedadId = idEnf;


            if (newRegMed.EnfermedadId == 0)
            {
                List<ModelCL.Medicina> lMedicinas = db.Medicina.Where(m => (m.Usuario == null || m.Usuario.UsuarioId == idUsu) && m.MedicinaGeneral == true).ToList();
                ViewBag.lMedicinas = new SelectList(lMedicinas, "MedicinaId", "MedicinaNombre");
            }
            else
            {
                List<ModelCL.Medicina> lMedicinas = db.Medicina.Where(m => m.RelMedRelPerEnf.Where(rmrpe => rmrpe.EnfermedadId == newRegMed.EnfermedadId && rmrpe.PersonaId == newRegMed.PersonaId).FirstOrDefault() != null).ToList();
                ViewBag.lMedicinas = new SelectList(lMedicinas, "MedicinaId", "MedicinaNombre");
            }
            
            List<ModelCL.Presentacion> lPresentacionesMed = db.Presentacion.ToList();
            ViewBag.lPresentacionesMed = new SelectList(lPresentacionesMed, "PresentacionId", "PresentacionNombre");

            return PartialView(newRegMed);
        }

        [HttpPost]
        [Route("_crear")]
        [ValidateAntiForgeryToken]
        public ActionResult _Create(RegMedicacionViewModel datos)
        {
            if (ModelState.IsValid)
            {
                ModelCL.Persona persona = db.Persona.Find(datos.PersonaId);

                ModelCL.Registro regMedicacion = new ModelCL.Registro();
                regMedicacion.RegistroFchHora = Convert.ToDateTime(datos.RegistroFchHora);

                ModelCL.Medicacion medicacion = new ModelCL.Medicacion();

                if (datos.EnfermedadId != 0)
                {
                    medicacion.EnfermedadId = datos.EnfermedadId;
                }
                
                medicacion.MedicinaId = datos.MedicinaId;
                medicacion.PresentacionId = datos.PresentacionId;
                medicacion.MedicacionCantidad = datos.MedicacionCantidad;
                
                regMedicacion.Medicacion = medicacion;

                persona.Registro.Add(regMedicacion);

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);

            if (datos.EnfermedadId == 0)
            {
                List<ModelCL.Medicina> lMedicinas = db.Medicina.Where(m => (m.Usuario == null || m.Usuario.UsuarioId == idUsu) && m.MedicinaGeneral == true).ToList();
                ViewBag.lMedicinas = new SelectList(lMedicinas, "MedicinaId", "MedicinaNombre");
            }
            else
            {
                List<ModelCL.Medicina> lMedicinas = db.Medicina.Where(m => m.RelMedRelPerEnf.Where(rmrpe => rmrpe.EnfermedadId == datos.EnfermedadId && rmrpe.PersonaId == datos.PersonaId).FirstOrDefault() != null).ToList();
                ViewBag.lMedicinas = new SelectList(lMedicinas, "MedicinaId", "MedicinaNombre");
            }

            List<ModelCL.Presentacion> lPresentacionesMed = db.Presentacion.ToList();
            ViewBag.lPresentacionesMed = new SelectList(lPresentacionesMed, "PresentacionId", "PresentacionNombre");


            TempData["ErrorMessagePartial"] = "Error inesperado";

            return RedirectToAction("Create");
        }

        [Route("editar")]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            long idPer = Convert.ToInt64(Request.Cookies["cookiePer"]["PerId"]);
            ModelCL.Registro regMedicacion = db.Registro.Where(r => r.RegistroId == id).FirstOrDefault();
            if (regMedicacion == null || regMedicacion.Medicacion == null || regMedicacion.Persona.PersonaId != idPer)
            {
                return HttpNotFound();
            }

            RegMedicacionViewModel vmRegMedicacion = new RegMedicacionViewModel();
            vmRegMedicacion.RegistroId = regMedicacion.RegistroId;
            vmRegMedicacion.PersonaId = regMedicacion.PersonaId;
            vmRegMedicacion.EnfermedadId = regMedicacion.Medicacion.EnfermedadId;
            vmRegMedicacion.MedicinaId = regMedicacion.Medicacion.MedicinaId;

            if (regMedicacion.Medicacion.Enfermedad == null)
            {
                vmRegMedicacion.EnfermedadNombre = "-";
            }
            else
            {
                vmRegMedicacion.EnfermedadNombre = regMedicacion.Medicacion.Enfermedad.EnfermedadNombre;
            }
            vmRegMedicacion.MedicinaNombre = regMedicacion.Medicacion.Medicina.MedicinaNombre;

            vmRegMedicacion.RegistroFchHora = regMedicacion.RegistroFchHora.ToString();
            vmRegMedicacion.PresentacionId = regMedicacion.Medicacion.PresentacionId;
            vmRegMedicacion.MedicacionCantidad = regMedicacion.Medicacion.MedicacionCantidad;


            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);

            if (vmRegMedicacion.EnfermedadId == null)
            {
                List<ModelCL.Medicina> lMedicinas = db.Medicina.Where(m => (m.Usuario == null || m.Usuario.UsuarioId == idUsu) && m.MedicinaGeneral == true).ToList();
                ViewBag.lMedicinas = new SelectList(lMedicinas, "MedicinaId", "MedicinaNombre");
            }
            else
            {
                List<ModelCL.Medicina> lMedicinas = db.Medicina.Where(m => m.RelMedRelPerEnf.Where(rmrpe => rmrpe.EnfermedadId == vmRegMedicacion.EnfermedadId && rmrpe.PersonaId == vmRegMedicacion.PersonaId).FirstOrDefault() != null).ToList();
                ViewBag.lMedicinas = new SelectList(lMedicinas, "MedicinaId", "MedicinaNombre");
            }

            List<ModelCL.Presentacion> lPresentacionesMed = db.Presentacion.ToList();
            ViewBag.lPresentacionesMed = new SelectList(lPresentacionesMed, "PresentacionId", "PresentacionNombre");

            return View(vmRegMedicacion);
        }

        [HttpPost]
        [Route("editar")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RegMedicacionViewModel datos)
        {            
            if (ModelState.IsValid)
            {
                ModelCL.Registro regMedicacion = db.Registro.Where(r => r.RegistroId == datos.RegistroId).FirstOrDefault();

                regMedicacion.RegistroFchHora = Convert.ToDateTime(datos.RegistroFchHora);
                regMedicacion.Medicacion.PresentacionId = datos.PresentacionId;
                regMedicacion.Medicacion.MedicacionCantidad = datos.MedicacionCantidad;

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);

            if (datos.EnfermedadId == null)
            {
                List<ModelCL.Medicina> lMedicinas = db.Medicina.Where(m => (m.Usuario == null || m.Usuario.UsuarioId == idUsu) && m.MedicinaGeneral == true).ToList();
                ViewBag.lMedicinas = new SelectList(lMedicinas, "MedicinaId", "MedicinaNombre");
            }
            else
            {
                List<ModelCL.Medicina> lMedicinas = db.Medicina.Where(m => m.RelMedRelPerEnf.Where(rmrpe => rmrpe.EnfermedadId == datos.EnfermedadId && rmrpe.PersonaId == datos.PersonaId).FirstOrDefault() != null).ToList();
                ViewBag.lMedicinas = new SelectList(lMedicinas, "MedicinaId", "MedicinaNombre");
            }

            List<ModelCL.Presentacion> lPresentacionesMed = db.Presentacion.ToList();
            ViewBag.lPresentacionesMed = new SelectList(lPresentacionesMed, "PresentacionId", "PresentacionNombre");

            return View(datos);
        }

        [Route("eliminar")]
        //[ValidateAntiForgeryToken]
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            long idPer = Convert.ToInt64(Request.Cookies["cookiePer"]["PerId"]);
            ModelCL.Registro registro = db.Registro.Where(r => r.RegistroId == id && r.Medicacion != null).FirstOrDefault();
            if (registro == null || registro.Medicacion == null || registro.Persona.PersonaId != idPer)
            {
                return HttpNotFound();
            }

            db.Registro.Remove(registro);
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
