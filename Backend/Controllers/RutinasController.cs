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
    [RoutePrefix("rutinas")]
    public class RutinasController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        [Route("index")]
        public ActionResult Index()
        {
            var rutinas = db.Contenido.Where(c => c.Rutina != null).ToList();

            return View(rutinas);
        }

        [Route("details")]
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModelCL.Contenido contRutina = db.Contenido.Find(id);
            if (contRutina.Rutina == null)
            {
                return HttpNotFound();
            }
            return View(contRutina);
        }

        [Route("create")]
        public ActionResult Create()
        {
            List<ModelCL.Ejercicio> lEjercicios = db.Ejercicio.ToList();
            ViewBag.lEjercicios = lEjercicios;

            return View();
        }

        [HttpPost]
        [Route("create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ModelCL.Contenido contenido, int[] ejercicios)
        {
            if (ModelState.IsValid)
            {
                contenido.Rutina = new ModelCL.Rutina();

                foreach (var e in ejercicios)
                {
                    contenido.Rutina.Ejercicio.Add(db.Ejercicio.Find(e));
                }

                db.Contenido.Add(contenido);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(contenido);
        }

        [Route("edit")]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ModelCL.Contenido contRutina = db.Contenido.Find(id);
            if (contRutina.Rutina == null)
            {
                return HttpNotFound();
            }

            List<ModelCL.Ejercicio> lEjercicios = db.Ejercicio.ToList();
            ViewBag.lEjercicios = lEjercicios;

            return View(contRutina);
        }

        [HttpPost]
        [Route("edit")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ModelCL.Contenido contenido, int[] ejercicios)
        {
            if (ModelState.IsValid)
            {
                ModelCL.Contenido oldContenido = db.Contenido.Find(contenido.ContenidoId);
                oldContenido.ContenidoTitulo = contenido.ContenidoTitulo;
                oldContenido.ContenidoDescripcion = contenido.ContenidoDescripcion;
                oldContenido.ContenidoCuerpo = contenido.ContenidoCuerpo;


                List<ModelCL.Ejercicio> bkEjercicios = oldContenido.Rutina.Ejercicio.ToList();
                foreach (ModelCL.Ejercicio oldEjercicio in bkEjercicios)
                {
                    oldContenido.Rutina.Ejercicio.Remove(oldEjercicio);
                }


                foreach (var e in ejercicios)
                {
                    oldContenido.Rutina.Ejercicio.Add(db.Ejercicio.Find(e));
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(contenido);
        }

        [Route("delete")]
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModelCL.Contenido contRutina = db.Contenido.Find(id);
            if (contRutina.Rutina == null)
            {
                return HttpNotFound();
            }
            return View(contRutina);
        }

        [HttpPost, ActionName("Delete")]
        [Route("delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ModelCL.Contenido contRutina = db.Contenido.Find(id);

            db.Contenido.Remove(contRutina);
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
