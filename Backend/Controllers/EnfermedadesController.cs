﻿using System;
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

        [Route("index")]
        public ActionResult Index()
        {
            List<ModelCL.Enfermedad> enfermedades = db.Enfermedad.ToList();

            return View(enfermedades);
        }

        [Route("details")]
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

        [Route("create")]
        public ActionResult Create()
        {
            List<ModelCL.Valor> lValores = db.Valor.ToList();
            ViewBag.lValores = lValores;

            List<ModelCL.Medicina> lMedicinas = db.Medicina.ToList();
            ViewBag.lMedicinas = lMedicinas;

            return View();
        }

        [HttpPost]
        [Route("create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ModelCL.Enfermedad enfermedad, int[] valores, int[] medicinas)
        {
            if (ModelState.IsValid)
            {
                foreach (var v in valores)
                {
                    enfermedad.Valor.Add(db.Valor.Find(v));
                }

                foreach (var m in medicinas)
                {
                    enfermedad.Medicina.Add(db.Medicina.Find(m));
                }

                db.Enfermedad.Add(enfermedad);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            List<ModelCL.Valor> lValores = db.Valor.ToList();
            ViewBag.lValores = lValores;

            List<ModelCL.Medicina> lMedicinas = db.Medicina.ToList();
            ViewBag.lMedicinas = lMedicinas;

            return View(enfermedad);
        }

        [Route("edit")]
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

            List<ModelCL.Medicina> lMedicinas = db.Medicina.ToList();
            ViewBag.lMedicinas = lMedicinas;

            return View(enfermedad);
        }

        [HttpPost]
        [Route("edit")]
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

                foreach (var v in valores)
                {
                    enf.Valor.Add(db.Valor.Find(v));
                }

                List<ModelCL.Medicina> bkMedicinas = enf.Medicina.ToList();
                foreach (ModelCL.Medicina oldMedicina in bkMedicinas)
                {
                    enf.Medicina.Remove(oldMedicina);
                }

                foreach (var m in medicinas)
                {
                    enf.Medicina.Add(db.Medicina.Find(m));
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            List<ModelCL.Valor> lValores = db.Valor.ToList();
            ViewBag.lValores = lValores;

            List<ModelCL.Medicina> lMedicinas = db.Medicina.ToList();
            ViewBag.lMedicinas = lMedicinas;

            return View(enfermedad);
        }

        [Route("delete")]
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
        [Route("delete")]
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
