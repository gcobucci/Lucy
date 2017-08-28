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
    public class MultimediaController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        // GET: Multimedia
        public ActionResult Index()
        {
            var multimedia = db.Multimedia.Include(m => m.Contenido);
            return View(multimedia.ToList());
        }

        // GET: Multimedia/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Multimedia multimedia = db.Multimedia.Find(id);
            if (multimedia == null)
            {
                return HttpNotFound();
            }
            return View(multimedia);
        }

        // GET: Multimedia/Create
        public ActionResult Create()
        {
            ViewBag.ContenidoId = new SelectList(db.Contenido, "ContenidoId", "ContenidoTitulo");
            return View();
        }

        // POST: Multimedia/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MultimediaId,ContenidoId,MultimediaTipo,MultimediaImage")] Multimedia multimedia)
        {
            if (ModelState.IsValid)
            {
                db.Multimedia.Add(multimedia);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ContenidoId = new SelectList(db.Contenido, "ContenidoId", "ContenidoTitulo", multimedia.ContenidoId);
            return View(multimedia);
        }

        // GET: Multimedia/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Multimedia multimedia = db.Multimedia.Find(id);
            if (multimedia == null)
            {
                return HttpNotFound();
            }
            ViewBag.ContenidoId = new SelectList(db.Contenido, "ContenidoId", "ContenidoTitulo", multimedia.ContenidoId);
            return View(multimedia);
        }

        // POST: Multimedia/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MultimediaId,ContenidoId,MultimediaTipo,MultimediaImage")] Multimedia multimedia)
        {
            if (ModelState.IsValid)
            {
                db.Entry(multimedia).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ContenidoId = new SelectList(db.Contenido, "ContenidoId", "ContenidoTitulo", multimedia.ContenidoId);
            return View(multimedia);
        }

        // GET: Multimedia/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Multimedia multimedia = db.Multimedia.Find(id);
            if (multimedia == null)
            {
                return HttpNotFound();
            }
            return View(multimedia);
        }

        // POST: Multimedia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Multimedia multimedia = db.Multimedia.Find(id);
            db.Multimedia.Remove(multimedia);
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
