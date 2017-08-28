using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelCL;

namespace Backend.Controllers
{
    [RoutePrefix("bk-recetas2")]
    public class RecetasController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        // GET: Recetas
        [Route("index")]
        public ActionResult Index()
        {
            var receta = db.Receta.Include(r => r.Contenido);
            return View(receta.ToList());
        }

        // GET: Recetas/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Receta receta = db.Receta.Find(id);
            if (receta == null)
            {
                return HttpNotFound();
            }
            return View(receta);
        }

        // GET: Recetas/Create
        public ActionResult Create()
        {
            ViewBag.RecetaId = new SelectList(db.Contenido, "ContenidoId", "ContenidoTitulo");
            return View();
        }

        // POST: Recetas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RecetaId,RecetaDescripcion,RecetaCalorias,RecetaHidratos,RecetaSodio,RecetaGluten")] Receta receta)
        {
            if (ModelState.IsValid)
            {
                db.Receta.Add(receta);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RecetaId = new SelectList(db.Contenido, "ContenidoId", "ContenidoTitulo", receta.RecetaId);
            return View(receta);
        }

        // GET: Recetas/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Receta receta = db.Receta.Find(id);
            if (receta == null)
            {
                return HttpNotFound();
            }
            ViewBag.RecetaId = new SelectList(db.Contenido, "ContenidoId", "ContenidoTitulo", receta.RecetaId);
            return View(receta);
        }

        // POST: Recetas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RecetaId,RecetaDescripcion,RecetaCalorias,RecetaHidratos,RecetaSodio,RecetaGluten")] Receta receta)
        {
            if (ModelState.IsValid)
            {
                db.Entry(receta).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RecetaId = new SelectList(db.Contenido, "ContenidoId", "ContenidoTitulo", receta.RecetaId);
            return View(receta);
        }

        // GET: Recetas/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Receta receta = db.Receta.Find(id);
            if (receta == null)
            {
                return HttpNotFound();
            }
            return View(receta);
        }

        // POST: Recetas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Receta receta = db.Receta.Find(id);
            db.Receta.Remove(receta);
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
