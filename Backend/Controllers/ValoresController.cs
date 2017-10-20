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
    [RoutePrefix("valores")]
    public class ValoresController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();
        
        [Route("listado")]
        public ActionResult Index()
        {
            List<ModelCL.Valor> valores = db.Valor.ToList();

            return View(valores);
        }

        [Route("ver")]
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModelCL.Valor valor = db.Valor.Find(id);
            if (valor == null)
            {
                return HttpNotFound();
            }
            return View(valor);
        }

        [Route("crear")]
        public ActionResult Create()
        {
            List<ModelCL.Enfermedad> lEnfermedades = db.Enfermedad.ToList();
            ViewBag.lEnfermedades = lEnfermedades;

            return View();
        }

        [HttpPost]
        [Route("crear")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ModelCL.Valor valor, int[] enfermedades)
        {
            if (ModelState.IsValid)
            {
                foreach (var e in enfermedades)
                {
                    valor.Enfermedad.Add(db.Enfermedad.Find(e));
                }

                db.Valor.Add(valor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            List<ModelCL.Enfermedad> lEnfermedades = db.Enfermedad.ToList();
            ViewBag.lEnfermedades = lEnfermedades;

            return View(valor);
        }

        [Route("editar")]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ModelCL.Valor valor = db.Valor.Find(id);
            if (valor == null)
            {
                return HttpNotFound();
            }

            List<ModelCL.Enfermedad> lEnfermedades = db.Enfermedad.ToList();
            ViewBag.lEnfermedades = lEnfermedades;

            return View(valor);
        }

        [HttpPost]
        [Route("editar")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ModelCL.Valor valor, int[] enfermedades)
        {
            if (ModelState.IsValid)
            {
                ModelCL.Valor val = db.Valor.Find(valor.ValorId);

                val.ValorNombre = valor.ValorNombre;
                val.ValorDesc = valor.ValorDesc;
                val.ValorGeneral = valor.ValorGeneral;
                val.ValorMedida = valor.ValorMedida;
                val.ValorBajoMinimo = valor.ValorBajoMinimo;
                val.ValorNormalMinimo = valor.ValorNormalMinimo;
                val.ValorNormalMaximo = valor.ValorNormalMaximo;
                val.ValorAltoMaximo = valor.ValorAltoMaximo;
                val.ValorMsgMuyBajo = valor.ValorMsgMuyBajo;
                val.ValorMsgBajo = valor.ValorMsgBajo;
                val.ValorMsgNormal = valor.ValorMsgNormal;
                val.ValorMsgAlto = valor.ValorMsgAlto;
                val.ValorMsgMuyAlto = valor.ValorMsgMuyAlto;

                List<ModelCL.Enfermedad> bkEnfermedades = val.Enfermedad.ToList();
                foreach (ModelCL.Enfermedad oldEnfermedad in bkEnfermedades)
                {
                    val.Enfermedad.Remove(oldEnfermedad);
                }

                foreach (var e in enfermedades)
                {
                    val.Enfermedad.Add(db.Enfermedad.Find(e));
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            List<ModelCL.Enfermedad> lEnfermedades = db.Enfermedad.ToList();
            ViewBag.lEnfermedades = lEnfermedades;

            return View(valor);
        }

        [Route("eliminar")]
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ModelCL.Valor valor = db.Valor.Find(id);
            if (valor == null)
            {
                return HttpNotFound();
            }
            return View(valor);
        }

        [HttpPost, ActionName("Delete")]
        [Route("eliminar")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ModelCL.Valor valor = db.Valor.Find(id);

            db.Valor.Remove(valor);
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
