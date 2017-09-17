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
    [RoutePrefix("dosis")]
    public class DosisController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        [Route("index")]
        public ActionResult Index()
        {
            List<ModelCL.Dosis> dosis = db.Dosis.ToList();

            return View(dosis);
        }

        [Route("create")]
        public ActionResult Create(long? idMed = null, long? idVal = null)
        {
            ModelCL.Dosis dosis = new ModelCL.Dosis();

            dosis.RelMedVal = new ModelCL.RelMedVal();

            if (idMed != null)
            {
                dosis.MedicinaId = Convert.ToInt64(idMed); 
            }

            if (idVal != null)
            {
                dosis.ValorId = Convert.ToInt64(idVal);
            }

            List<ModelCL.Medicina> lMedicinas = db.Medicina.Where(e => e.Usuario == null).ToList();
            ViewBag.lMedicinas = new SelectList(lMedicinas, "MedicinaId", "MedicinaNombre");

            List<ModelCL.Valor> lValores = db.Valor.ToList();
            ViewBag.lValores = new SelectList(lValores, "ValorId", "ValorNombre");

            return View(dosis);
        }

        [HttpPost]
        [Route("create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ModelCL.Dosis datos)
        {
            if (ModelState.IsValid)
            {
                ModelCL.Dosis dosis = new ModelCL.Dosis();

                dosis.DosisTipo = datos.DosisTipo;
                dosis.DosisCantidadMin = datos.DosisCantidadMin;
                dosis.DosisMedida = datos.DosisMedida;
                dosis.DosisEfecto = datos.DosisEfecto;


                ModelCL.RelMedVal oldRMV = db.RelMedVal.Where(r => r.MedicinaId == datos.MedicinaId && r.ValorId == datos.ValorId).FirstOrDefault();

                if (oldRMV != null)
                {
                    oldRMV.Dosis.Add(dosis);
                }
                else
                {
                    ModelCL.RelMedVal newRMV = new ModelCL.RelMedVal();

                    newRMV.MedicinaId = datos.MedicinaId;
                    newRMV.ValorId = datos.ValorId;

                    db.RelMedVal.Add(newRMV);

                    newRMV.Dosis.Add(dosis);
                }

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

            ModelCL.Dosis dosis = db.Dosis.SingleOrDefault(d => d.DosisId == id);
            if (dosis == null)
            {
                return HttpNotFound();
            }

            List<ModelCL.Medicina> lMedicinas = db.Medicina.Where(e => e.Usuario == null).ToList();
            ViewBag.lMedicinas = new SelectList(lMedicinas, "MedicinaId", "MedicinaNombre");

            List<ModelCL.Valor> lValores = db.Valor.ToList();
            ViewBag.lValores = new SelectList(lValores, "ValorId", "ValorNombre");

            return View(dosis);
        }

        [HttpPost]
        [Route("edit")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ModelCL.Dosis dosis)
        {
            if (ModelState.IsValid)
            {
                //ModelCL.Dosis d = db.Dosis.SingleOrDefault(dd => dd.DosisId == dosis.DosisId);

                //d.DosisTipo = dosis.DosisTipo;
                //d.DosisMedida = dosis.DosisMedida;
                //d.DosisCantidadMin = dosis.DosisCantidadMin;
                //d.DosisEfecto = dosis.DosisEfecto;

                db.Entry(dosis).State = EntityState.Modified; //No anda no se porque

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dosis);
        }

        [Route("delete")]
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ModelCL.Dosis dosis = db.Dosis.SingleOrDefault(d => d.DosisId == id);
            if (dosis == null)
            {
                return HttpNotFound();
            }
            return View(dosis);
        }

        [HttpPost, ActionName("Delete")]
        [Route("delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ModelCL.Dosis dosis = db.Dosis.SingleOrDefault(d => d.DosisId == id);

            ModelCL.RelMedVal rmv = dosis.RelMedVal;

            db.Dosis.Remove(dosis);

            if (rmv.Dosis.Count() == 0)
            {
                db.RelMedVal.Remove(rmv);
            }

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
