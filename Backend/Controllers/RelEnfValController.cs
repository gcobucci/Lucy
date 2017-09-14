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
    [RoutePrefix("relenfval")]
    public class RelEnfValController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        [Route("index")]
        public ActionResult Index()
        {
            List<ModelCL.RelEnfVal> relenfval = db.RelEnfVal.ToList();

            return View(relenfval);
        }

        //[Route("details")]
        //public ActionResult Details(long? idEnf, long? idVal)
        //{
        //    if (idEnf == null || idVal == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    ModelCL.RelEnfVal relenfval = db.RelEnfVal.Where(r => r.EnfermedadId == idEnf && r.ValorId == idVal).FirstOrDefault();
        //    if (relenfval == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(relenfval);
        //}

        [Route("create")]
        public ActionResult Create(long? idEnf = null, long? idVal = null)
        {
            ModelCL.RelEnfVal relenfval = new ModelCL.RelEnfVal();

            if (idEnf != null)
            {
                relenfval.EnfermedadId = Convert.ToInt64(idEnf);
            }

            if (idVal != null)
            {
                relenfval.ValorId = Convert.ToInt64(idVal);
            }

            List<ModelCL.Enfermedad> lEnfermedades = db.Enfermedad.Where(e => e.Usuario == null).ToList();
            ViewBag.lEnfermedades = new SelectList(lEnfermedades, "EnfermedadId", "EnfermedadNombre");

            List<ModelCL.Valor> lValores = db.Valor.ToList();
            ViewBag.lValores = new SelectList(lValores, "ValorId", "ValorNombre");

            return View(relenfval);
        }

        [HttpPost]
        [Route("create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ModelCL.RelEnfVal relenfval)
        {
            if (ModelState.IsValid)
            {
                db.RelEnfVal.Add(relenfval);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(relenfval);
        }

        [Route("edit")]
        public ActionResult Edit(long? idEnf, long? idVal)
        {
            if (idEnf == null || idVal == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ModelCL.RelEnfVal relenfval = db.RelEnfVal.Where(r => r.EnfermedadId == idEnf && r.ValorId == idVal).FirstOrDefault();
            if (relenfval == null)
            {
                return HttpNotFound();
            }

            List<ModelCL.Enfermedad> lEnfermedades = db.Enfermedad.Where(e => e.Usuario == null).ToList();
            ViewBag.lEnfermedades = new SelectList(lEnfermedades, "EnfermedadId", "EnfermedadNombre");

            List<ModelCL.Valor> lValores = db.Valor.ToList();
            ViewBag.lValores = new SelectList(lValores, "ValorId", "ValorNombre");

            return View(relenfval);
        }

        [HttpPost]
        [Route("edit")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ModelCL.RelEnfVal relenfval)
        {
            if (ModelState.IsValid)
            {
                ModelCL.RelEnfVal rev = db.RelEnfVal.Where(r => r.EnfermedadId == relenfval.EnfermedadId && r.ValorId == relenfval.ValorId).FirstOrDefault();

                rev.RelEnfValMinimo = relenfval.RelEnfValMinimo;
                rev.RelEnfValMaximo = relenfval.RelEnfValMaximo;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(relenfval);
        }

        [Route("delete")]
        public ActionResult Delete(long? idEnf, long? idVal)
        {
            if (idEnf == null || idVal == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ModelCL.RelEnfVal relenfval = db.RelEnfVal.Where(r => r.EnfermedadId == idEnf && r.ValorId == idVal).FirstOrDefault();
            if (relenfval == null)
            {
                return HttpNotFound();
            }
            return View(relenfval);
        }

        [HttpPost, ActionName("Delete")]
        [Route("delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long idEnf, long idVal)
        {
            ModelCL.RelEnfVal relenfval = db.RelEnfVal.Where(r => r.EnfermedadId == idEnf && r.ValorId == idVal).FirstOrDefault();

            db.RelEnfVal.Remove(relenfval);
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
