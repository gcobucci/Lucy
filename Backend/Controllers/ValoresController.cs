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
        
        [Route("index")]
        public ActionResult Index()
        {
            List<ModelCL.Valor> valores = db.Valor.ToList();

            return View(valores);
        }

        [Route("details")]
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

        [Route("create")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Route("create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ModelCL.Valor valor)
        {
            if (ModelState.IsValid)
            {
                db.Valor.Add(valor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(valor);
        }

        [Route("edit")]
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
            return View(valor);
        }

        [HttpPost]
        [Route("edit")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ModelCL.Valor valor)
        {
            if (ModelState.IsValid)
            {
                ModelCL.Valor val = db.Valor.Find(valor.ValorId);

                val.ValorNombre = valor.ValorNombre;
                val.ValorDesc = valor.ValorDesc;
                val.ValorMedida = valor.ValorMedida;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(valor);
        }

        [Route("delete")]
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
        [Route("delete")]
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
