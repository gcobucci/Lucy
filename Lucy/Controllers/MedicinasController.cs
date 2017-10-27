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
    [RoutePrefix("medicinas")]
    public class MedicinasController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        [Authorize]
        [Route("listado")]
        public ActionResult Index()
        {
            List<ModelCL.Medicina> medicinas = null;

            if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
            {
                long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);
                ViewBag.idUsu = idUsu;

                medicinas = db.Medicina.Where(a => a.Usuario == null || a.Usuario.UsuarioId == idUsu).OrderBy(a => a.MedicinaNombre).ToList();
            }
            else
            {
                medicinas = db.Medicina.Where(a => a.Usuario == null).OrderBy(a => a.MedicinaNombre).ToList();
            }

            return View(medicinas);
        }

        [Authorize]
        [Route("crear")]
        public ActionResult Create()
        {
            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);

            MedicinaViewModel newMedicina = new MedicinaViewModel();

            List<ModelCL.Enfermedad> lEnfermedades = db.Enfermedad.Where(e => e.Usuario == null || (e.Usuario != null && e.Usuario.UsuarioId == idUsu)).ToList();
            List<Fachada.ViewModelCheckBox> lEnf = new List<Fachada.ViewModelCheckBox>();
            foreach (ModelCL.Enfermedad enf in lEnfermedades)
            {
                lEnf.Add(new Fachada.ViewModelCheckBox() { Id = enf.EnfermedadId, Nombre = enf.EnfermedadNombre });
            }

            newMedicina.Enfermedades = lEnf;


            List<Fachada.ViewModelSelectListChk> lMedTipos = new List<Fachada.ViewModelSelectListChk>()
            {
                new Fachada.ViewModelSelectListChk { Id = "Activa", Valor = "Activa" },
                new Fachada.ViewModelSelectListChk { Id = "Pasiva", Valor = "Pasiva" },
            };
            ViewBag.lMedTipos = new SelectList(lMedTipos, "Id", "Valor");

            return View(newMedicina);
        }

        [Authorize]
        [HttpPost]
        [Route("crear")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MedicinaViewModel datos)
        {
            List<Fachada.ViewModelSelectListChk> lMedTipos = new List<Fachada.ViewModelSelectListChk>()
            {
                new Fachada.ViewModelSelectListChk { Id = "Activa", Valor = "Activa" },
                new Fachada.ViewModelSelectListChk { Id = "Pasiva", Valor = "Pasiva" },
            };
            ViewBag.lMedTipos = new SelectList(lMedTipos, "Id", "Valor");

            if (ModelState.IsValid)
            {
                ModelCL.Medicina newMed = new ModelCL.Medicina();
                newMed.MedicinaNombre = datos.MedicinaNombre;
                newMed.MedicinaDesc = datos.MedicinaDesc;
                newMed.MedicinaTipo = datos.MedicinaTipo;
                newMed.MedicinaGeneral = datos.MedicinaGeneral;

                if (datos.Enfermedades.Where(e => e.Checked == true).Count() > 0)
                {
                    foreach (var enf in datos.Enfermedades.Where(e => e.Checked == true))
                    {
                        newMed.Enfermedad.Add(db.Enfermedad.Find(enf.Id));
                    }
                }                


                long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);
                newMed.Usuario = db.Usuario.Find(idUsu);
                
                db.Medicina.Add(newMed);
                db.SaveChanges();
                return RedirectToAction("Index", "Medicinas");
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
            ModelCL.Medicina oldMedicina = db.Medicina.Find(id);
            if (oldMedicina == null || oldMedicina.Usuario.UsuarioId != idUsu)
            {
                return HttpNotFound();
            }


            List<Fachada.ViewModelSelectListChk> lMedTipos = new List<Fachada.ViewModelSelectListChk>()
            {
                new Fachada.ViewModelSelectListChk { Id = "Activa", Valor = "Activa" },
                new Fachada.ViewModelSelectListChk { Id = "Pasiva", Valor = "Pasiva" },
            };
            ViewBag.lMedTipos = new SelectList(lMedTipos, "Id", "Valor");



            MedicinaViewModel medicina = new MedicinaViewModel();

            medicina.MedicinaId = oldMedicina.MedicinaId;
            medicina.MedicinaNombre = oldMedicina.MedicinaNombre;
            medicina.MedicinaDesc = oldMedicina.MedicinaDesc;
            medicina.MedicinaTipo = oldMedicina.MedicinaTipo;
            medicina.MedicinaGeneral = oldMedicina.MedicinaGeneral;


            List<ModelCL.Enfermedad> lEnfermedades = db.Enfermedad.Where(e => e.Usuario == null || (e.Usuario != null && e.Usuario.UsuarioId == idUsu)).ToList();
            List<Fachada.ViewModelCheckBox> lEnf = new List<Fachada.ViewModelCheckBox>();
            foreach (ModelCL.Enfermedad enf in lEnfermedades)
            {
                lEnf.Add(new Fachada.ViewModelCheckBox() { Id = enf.EnfermedadId, Nombre = enf.EnfermedadNombre });
            }

            foreach (Fachada.ViewModelCheckBox enf in lEnf)
            {
                ModelCL.Enfermedad en = oldMedicina.Enfermedad.Where(e => e.EnfermedadId == enf.Id).FirstOrDefault();

                if (en != null)
                {
                    enf.Checked = true;
                }
            }

            medicina.Enfermedades = lEnf;


            return View(medicina);
        }

        [Authorize]
        [HttpPost]
        [Route("editar")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MedicinaViewModel datos)
        {
            List<Fachada.ViewModelSelectListChk> lMedTipos = new List<Fachada.ViewModelSelectListChk>()
            {
                new Fachada.ViewModelSelectListChk { Id = "Activa", Valor = "Activa" },
                new Fachada.ViewModelSelectListChk { Id = "Pasiva", Valor = "Pasiva" },
            };
            ViewBag.lMedTipos = new SelectList(lMedTipos, "Id", "Valor");

            if (ModelState.IsValid)
            {
                long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);

                ModelCL.Medicina medicina = db.Medicina.Find(datos.MedicinaId);

                medicina.MedicinaNombre = datos.MedicinaNombre;
                medicina.MedicinaDesc = datos.MedicinaDesc;
                medicina.MedicinaTipo = datos.MedicinaTipo;
                medicina.MedicinaGeneral = datos.MedicinaGeneral;


                List<ModelCL.Enfermedad> bkEnfermedades = medicina.Enfermedad.ToList();
                foreach (ModelCL.Enfermedad oldEnfermedad in bkEnfermedades)
                {
                    medicina.Enfermedad.Remove(oldEnfermedad);
                }

                if (datos.Enfermedades.Where(e => e.Checked == true).Count() > 0)
                {
                    foreach (var enf in datos.Enfermedades.Where(e => e.Checked == true))
                    {
                        medicina.Enfermedad.Add(db.Enfermedad.Where(e => e.EnfermedadId == enf.Id).FirstOrDefault());
                    }
                }           

                db.SaveChanges();
                return RedirectToAction("Index", "Medicinas");
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
            ModelCL.Medicina medicina = db.Medicina.Where(r => r.MedicinaId == id).FirstOrDefault();
            if (medicina == null || medicina.Usuario.UsuarioId != idUsu)
            {
                return HttpNotFound();
            }

            db.Medicina.Remove(medicina);
            db.SaveChanges();

            return RedirectToAction("Index", "Medicinas");
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