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
    [RoutePrefix("enfermedades")]
    public class EnfermedadesController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        [Route("listado")]
        public ActionResult Index()
        {
            List<ModelCL.Enfermedad> enfermedades = db.Enfermedad.Where(e => e.Usuario == null).ToList();

            return View(enfermedades);
        }

        [Route("ver")]
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ModelCL.Enfermedad enfermedad = db.Enfermedad.Find(id);
            if (enfermedad == null)
            {
                return HttpNotFound();
            }
            return View(enfermedad);
        }

        [Route("crear")]
        public ActionResult Create()
        {
            List<ModelCL.Valor> lValores = db.Valor.ToList();
            ViewBag.lValores = lValores;

            List<ModelCL.Medicina> lMedicinas = db.Medicina.Where(m => m.Usuario == null).ToList();
            ViewBag.lMedicinas = lMedicinas;

            return View();
        }

        [HttpPost]
        [Route("crear")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ModelCL.Enfermedad enfermedad, int[] valores, int[] medicinas)
        {
            if (ModelState.IsValid)
            {
                if (valores != null)
                {
                    foreach (var v in valores)
                    {
                        enfermedad.Valor.Add(db.Valor.Find(v));
                    }
                }

                if (medicinas != null)
                {
                    foreach (var m in medicinas)
                    {
                        enfermedad.Medicina.Add(db.Medicina.Find(m));
                    }
                }                

                db.Enfermedad.Add(enfermedad);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            List<ModelCL.Valor> lValores = db.Valor.ToList();
            ViewBag.lValores = lValores;

            List<ModelCL.Medicina> lMedicinas = db.Medicina.Where(m => m.Usuario == null).ToList();
            ViewBag.lMedicinas = lMedicinas;

            return View(enfermedad);
        }

        [Route("editar")]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ModelCL.Enfermedad enfermedad = db.Enfermedad.Find(id);
            if (enfermedad == null)
            {
                return HttpNotFound();
            }

            List<ModelCL.Valor> lValores = db.Valor.ToList();
            ViewBag.lValores = lValores;

            List<ModelCL.Medicina> lMedicinas = db.Medicina.Where(m => m.Usuario == null).ToList();
            ViewBag.lMedicinas = lMedicinas;

            return View(enfermedad);
        }

        [HttpPost]
        [Route("editar")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ModelCL.Enfermedad enfermedad, int[] valores, int[] medicinas)
        {
            if (ModelState.IsValid)
            {
                ModelCL.Enfermedad enf = db.Enfermedad.Find(enfermedad.EnfermedadId);

                enf.EnfermedadNombre = enfermedad.EnfermedadNombre;
                enf.EnfermedadDesc = enfermedad.EnfermedadDesc;


                List<ModelCL.Valor> bkValores = enf.Valor.ToList();
                foreach (ModelCL.Valor oldValor in bkValores)
                {
                    enf.Valor.Remove(oldValor);
                }

                if (valores != null)
                {
                    foreach (var v in valores)
                    {
                        enf.Valor.Add(db.Valor.Find(v));
                    }
                }
                

                List<ModelCL.Medicina> bkMedicinas = enf.Medicina.ToList();
                foreach (ModelCL.Medicina oldMedicina in bkMedicinas)
                {
                    enf.Medicina.Remove(oldMedicina);
                }

                if (medicinas != null)
                {
                    foreach (var m in medicinas)
                    {
                        enf.Medicina.Add(db.Medicina.Find(m));
                    }
                }               

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            List<ModelCL.Valor> lValores = db.Valor.ToList();
            ViewBag.lValores = lValores;

            List<ModelCL.Medicina> lMedicinas = db.Medicina.Where(m => m.Usuario == null).ToList();
            ViewBag.lMedicinas = lMedicinas;

            return View(enfermedad);
        }

        [Route("eliminar")]
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ModelCL.Enfermedad enfermedad = db.Enfermedad.Find(id);
            if (enfermedad == null)
            {
                return HttpNotFound();
            }
            return View(enfermedad);
        }

        [HttpPost, ActionName("Delete")]
        [Route("eliminar")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ModelCL.Enfermedad enfermedad = db.Enfermedad.Find(id);

            db.Enfermedad.Remove(enfermedad);
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
