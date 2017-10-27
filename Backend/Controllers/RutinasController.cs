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
    [RoutePrefix("rutinas")]
    public class RutinasController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        [Route("listado")]
        public ActionResult Index()
        {
            var rutinas = db.Contenido.Where(c => c.Rutina != null && c.UsuarioAutor == null).ToList();

            return View(rutinas);
        }

        [Route("ver")]
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

        [Route("crear")]
        public ActionResult Create()
        {
            List<ModelCL.Ejercicio> lEjercicios = db.Ejercicio.Where(e => e.Contenido.UsuarioAutor == null).ToList();
            ViewBag.lEjercicios = lEjercicios;

            return View();
        }

        [HttpPost]
        [Route("crear")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ModelCL.Contenido contenido, int[] ejercicios)
        {
            if (ModelState.IsValid)
            {
                contenido.Rutina = new ModelCL.Rutina();

                if (ejercicios != null)
                {
                    foreach (var e in ejercicios)
                    {
                        contenido.Rutina.Ejercicio.Add(db.Ejercicio.Find(e));
                    }
                }

                db.Contenido.Add(contenido);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            List<ModelCL.Ejercicio> lEjercicios = db.Ejercicio.Where(e => e.Contenido.UsuarioAutor == null).ToList();
            ViewBag.lEjercicios = lEjercicios;

            return View(contenido);
        }

        [Route("editar")]
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

            List<ModelCL.Ejercicio> lEjercicios = db.Ejercicio.Where(e => e.Contenido.UsuarioAutor == null).ToList();
            ViewBag.lEjercicios = lEjercicios;

            return View(contRutina);
        }

        [HttpPost]
        [Route("editar")]
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
                
                if (ejercicios != null)
                {
                    foreach (var e in ejercicios)
                    {
                        oldContenido.Rutina.Ejercicio.Add(db.Ejercicio.Find(e));
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            List<ModelCL.Ejercicio> lEjercicios = db.Ejercicio.Where(e => e.Contenido.UsuarioAutor == null).ToList();
            ViewBag.lEjercicios = lEjercicios;

            return View(contenido);
        }

        [Route("eliminar")]
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
        [Route("eliminar")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ModelCL.Contenido contRutina = db.Contenido.Find(id);

            List<ModelCL.Programa> bkProgramas = contRutina.Rutina.Programa.ToList();
            foreach (ModelCL.Programa oldPrograma in bkProgramas)
            {
                contRutina.Rutina.Programa.Remove(oldPrograma);
            }

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
