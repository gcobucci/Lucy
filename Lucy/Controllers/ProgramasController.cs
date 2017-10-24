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
using System.Web.Security;

namespace Backend.Controllers
{
    [Authorize]
    [RoutePrefix("programas")]
    public class ProgramasController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        [Route("listado")]
        public ActionResult Index()
        {
            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);


            if (Fachada.Functions.es_premium(idUsu) == false)
            {
                TempData["PermisoDenegado"] = true;
                return RedirectToAction("Index", "Home");
            }



            var programas = db.Contenido.Where(c => c.Programa != null && (c.UsuarioAutor == null || c.UsuarioAutor.UsuarioId == idUsu)).ToList();

            return View(programas);
        }

        [Route("ver")]
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);


            if (Fachada.Functions.es_premium(idUsu) == false)
            {
                TempData["PermisoDenegado"] = true;
                return RedirectToAction("Index", "Home");
            }



            ModelCL.Contenido contPrograma = db.Contenido.Find(id);
            if (contPrograma.Programa == null || (contPrograma.UsuarioAutor != null && contPrograma.UsuarioAutor.UsuarioId != idUsu))
            {
                return HttpNotFound();
            }

            contPrograma.ContenidoCantVisitas += 1;
            db.SaveChanges();

            return View(contPrograma);
        }

        //[Route("crear")]
        //public ActionResult Create()
        //{
        //    List<ModelCL.Rutina> lRutinas = db.Rutina.ToList();
        //    ViewBag.lRutinas = lRutinas;

        //    return View();
        //}

        //[HttpPost]
        //[Route("crear")]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(ModelCL.Contenido contenido, int[] rutinas)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        contenido.Programa = new ModelCL.Programa();

        //        foreach (var r in rutinas)
        //        {
        //            contenido.Programa.Rutina.Add(db.Rutina.Find(r));
        //        }

        //        db.Contenido.Add(contenido);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(contenido);
        //}

        //[Route("editar")]
        //public ActionResult Edit(long? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    ModelCL.Contenido contPrograma = db.Contenido.Find(id);
        //    if (contPrograma.Programa == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    List<ModelCL.Rutina> lRutinas = db.Rutina.ToList();
        //    ViewBag.lRutinas = lRutinas;

        //    return View(contPrograma);
        //}

        //[HttpPost]
        //[Route("editar")]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(ModelCL.Contenido contenido, int[] rutinas)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        ModelCL.Contenido oldContenido = db.Contenido.Find(contenido.ContenidoId);
        //        oldContenido.ContenidoTitulo = contenido.ContenidoTitulo;
        //        oldContenido.ContenidoDescripcion = contenido.ContenidoDescripcion;
        //        oldContenido.ContenidoCuerpo = contenido.ContenidoCuerpo;


        //        List<ModelCL.Rutina> bkRutinas = oldContenido.Programa.Rutina.ToList();
        //        foreach (ModelCL.Rutina oldRutina in bkRutinas)
        //        {
        //            oldContenido.Programa.Rutina.Remove(oldRutina);
        //        }


        //        foreach (var r in rutinas)
        //        {
        //            oldContenido.Programa.Rutina.Add(db.Rutina.Find(r));
        //        }

        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(contenido);
        //}

        //[Route("eliminar")]
        //public ActionResult Delete(long? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    ModelCL.Contenido contPrograma = db.Contenido.Find(id);
        //    if (contPrograma.Programa == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(contPrograma);
        //}

        //[HttpPost, ActionName("Delete")]
        //[Route("eliminar")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(long id)
        //{
        //    ModelCL.Contenido contPrograma = db.Contenido.Find(id);

        //    db.Contenido.Remove(contPrograma);
        //    db.SaveChanges();

        //    return RedirectToAction("Index");
        //}

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
