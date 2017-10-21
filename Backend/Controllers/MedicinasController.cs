using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelCL;
using System.IO;

namespace Backend.Controllers
{
    [RoutePrefix("medicinas")]
    public class MedicinasController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        [Route("listado")]
        public ActionResult Index()
        {
            List<ModelCL.Medicina> medicinas = db.Medicina.ToList();

            return View(medicinas);
        }

        [Route("ver")]
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModelCL.Medicina medicina = db.Medicina.Find(id);
            if (medicina == null)
            {
                return HttpNotFound();
            }
            return View(medicina);
        }

        [Route("crear")]
        public ActionResult Create()
        {
            List<ModelCL.Enfermedad> lEnfermedades = db.Enfermedad.ToList();
            ViewBag.lEnfermedades = lEnfermedades;

            List<Fachada.ViewModelSelectListChk> lMedTipos = new List<Fachada.ViewModelSelectListChk>()
            {
                new Fachada.ViewModelSelectListChk { Id = "Activa", Valor = "Activa" },
                new Fachada.ViewModelSelectListChk { Id = "Pasiva", Valor = "Pasiva" },
            };
            ViewBag.lMedTipos = new SelectList(lMedTipos, "Id", "Valor");

            return View();
        }

        [HttpPost]
        [Route("crear")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ModelCL.Medicina medicina, int[] enfermedades)
        {
            if (ModelState.IsValid)
            {
                if (enfermedades != null)
                {
                    foreach (var e in enfermedades)
                    {
                        medicina.Enfermedad.Add(db.Enfermedad.Find(e));
                    }
                }
                

                db.Medicina.Add(medicina);

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            List<ModelCL.Enfermedad> lEnfermedades = db.Enfermedad.ToList();
            ViewBag.lEnfermedades = lEnfermedades;

            List<Fachada.ViewModelSelectListChk> lMedTipos = new List<Fachada.ViewModelSelectListChk>()
            {
                new Fachada.ViewModelSelectListChk { Id = "Activa", Valor = "Activa" },
                new Fachada.ViewModelSelectListChk { Id = "Pasiva", Valor = "Pasiva" },
            };
            ViewBag.lMedTipos = new SelectList(lMedTipos, "Id", "Valor");

            return View(medicina);
        }

        [Route("editar")]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ModelCL.Medicina medicina = db.Medicina.Find(id);
            if (medicina == null)
            {
                return HttpNotFound();
            }

            List<ModelCL.Enfermedad> lEnfermedades = db.Enfermedad.ToList();
            ViewBag.lEnfermedades = lEnfermedades;

            List<Fachada.ViewModelSelectListChk> lMedTipos = new List<Fachada.ViewModelSelectListChk>()
            {
                new Fachada.ViewModelSelectListChk { Id = "Activa", Valor = "Activa" },
                new Fachada.ViewModelSelectListChk { Id = "Pasiva", Valor = "Pasiva" },
            };
            ViewBag.lMedTipos = new SelectList(lMedTipos, "Id", "Valor");

            return View(medicina);
        }

        [HttpPost]
        [Route("editar")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ModelCL.Medicina medicina, int[] enfermedades)
        {
            if (ModelState.IsValid)
            {
                ModelCL.Medicina med = db.Medicina.Find(medicina.MedicinaId);

                med.MedicinaNombre = medicina.MedicinaNombre;
                med.MedicinaTipo = medicina.MedicinaTipo;
                med.MedicinaDesc = medicina.MedicinaDesc;

                List<ModelCL.Enfermedad> bkEnfermedades = med.Enfermedad.ToList();
                foreach (ModelCL.Enfermedad oldEnfermedad in bkEnfermedades)
                {
                    med.Enfermedad.Remove(oldEnfermedad);
                }

                if (enfermedades != null)
                {
                    foreach (var e in enfermedades)
                    {
                        med.Enfermedad.Add(db.Enfermedad.Find(e));
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            List<ModelCL.Enfermedad> lEnfermedades = db.Enfermedad.ToList();
            ViewBag.lEnfermedades = lEnfermedades;

            List<Fachada.ViewModelSelectListChk> lMedTipos = new List<Fachada.ViewModelSelectListChk>()
            {
                new Fachada.ViewModelSelectListChk { Id = "Activa", Valor = "Activa" },
                new Fachada.ViewModelSelectListChk { Id = "Pasiva", Valor = "Pasiva" },
            };
            ViewBag.lMedTipos = new SelectList(lMedTipos, "Id", "Valor");

            return View(medicina);
        }

        [Route("eliminar")]
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ModelCL.Medicina medicina = db.Medicina.Find(id);
            if (medicina == null)
            {
                return HttpNotFound();
            }
            return View(medicina);
        }

        [HttpPost, ActionName("Delete")]
        [Route("eliminar")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ModelCL.Medicina medicina = db.Medicina.Find(id);

            db.Medicina.Remove(medicina);
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
