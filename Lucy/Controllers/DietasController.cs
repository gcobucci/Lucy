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
using Lucy.Models;

namespace Lucy.Controllers
{
    [Authorize]
    [RoutePrefix("dietas")]
    public class DietasController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        [Route("listado")]
        public ActionResult Index()
        {
            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);

            var dietas = db.Contenido.Where(c => c.Dieta != null && (c.UsuarioAutor == null || c.UsuarioAutor.UsuarioId == idUsu)).OrderBy(c => c.ContenidoTitulo).ToList();

            long idPer = Convert.ToInt64(Request.Cookies["cookiePer"]["PerId"]);
            ViewBag.idPer = idPer;

            return View(dietas);
        }

        [Route("ver")]
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);
            ViewBag.idUsu = idUsu;

            long idPer = Convert.ToInt64(Request.Cookies["cookiePer"]["PerId"]);
            ViewBag.idPer = idPer;

            ModelCL.Contenido contDieta = db.Contenido.Find(id);
            if (contDieta == null || contDieta.Dieta == null || (contDieta.UsuarioAutor != null && contDieta.UsuarioAutor.UsuarioId != idUsu))
            {
                return HttpNotFound();
            }

            contDieta.ContenidoCantVisitas += 1;
            db.SaveChanges();

            return View(contDieta);
        }

        [Route("crear")]
        public ActionResult Create()
        {
            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);

            if (Fachada.Functions.es_premium(idUsu) == false)
            {
                TempData["PermisoDenegado"] = true;
                return RedirectToAction("Index", "Home");
            }


            DietaViewModel newDieta = new DietaViewModel();

            return View(newDieta);
        }

        [HttpPost]
        [Route("crear")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DietaViewModel datos)
        {
            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);
            
            if (Fachada.Functions.es_premium(idUsu) == false)
            {
                TempData["PermisoDenegado"] = true;
                return RedirectToAction("Index", "Home");
            }
            
            if (ModelState.IsValid)
            {
                ModelCL.Contenido newContDie = new ModelCL.Contenido();
                newContDie.ContenidoTitulo = datos.ContenidoTitulo;
                newContDie.ContenidoDescripcion = datos.ContenidoDescripcion;
                newContDie.ContenidoCuerpo = datos.ContenidoCuerpo;

                ModelCL.Dieta dieta = new ModelCL.Dieta();
                dieta.DietaDesayunoCalorias = datos.DietaDesayunoCalorias;
                dieta.DietaDesayunoDescripcion = datos.DietaDesayunoDescripcion;
                dieta.DietaAlmuerzoCalorias = datos.DietaAlmuerzoCalorias;
                dieta.DietaAlmuerzoDescripcion = datos.DietaAlmuerzoDescripcion;
                dieta.DietaMeriendaCalorias = datos.DietaMeriendaCalorias;
                dieta.DietaMeriendaDescripcion = datos.DietaMeriendaDescripcion;
                dieta.DietaCenaCalorias = datos.DietaCenaCalorias;
                dieta.DietaCenaDescripcion = datos.DietaCenaDescripcion;
                dieta.DietaIngestasCalorias = datos.DietaIngestasCalorias;
                dieta.DietaIngestasDescripcion = datos.DietaIngestasDescripcion;

                newContDie.Dieta = dieta;

                newContDie.UsuarioAutor = db.Usuario.Find(idUsu);


                db.Contenido.Add(newContDie);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ErrorMessage = "Error inesperado";
            return View(datos);
        }

        [Route("editar")]
        public ActionResult Edit(long? id)
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

            ModelCL.Contenido oldContDieta = db.Contenido.Find(id);
            if (oldContDieta == null || oldContDieta.Dieta == null)
            {
                return HttpNotFound();
            }

            DietaViewModel dieta = new DietaViewModel();

            dieta.ContenidoId = oldContDieta.ContenidoId;
            dieta.ContenidoTitulo = oldContDieta.ContenidoTitulo;
            dieta.ContenidoDescripcion = oldContDieta.ContenidoDescripcion;
            dieta.ContenidoCuerpo = oldContDieta.ContenidoCuerpo;

            dieta.DietaDesayunoCalorias = oldContDieta.Dieta.DietaDesayunoCalorias;
            dieta.DietaDesayunoDescripcion = oldContDieta.Dieta.DietaDesayunoDescripcion;
            dieta.DietaAlmuerzoCalorias = oldContDieta.Dieta.DietaAlmuerzoCalorias;
            dieta.DietaAlmuerzoDescripcion = oldContDieta.Dieta.DietaAlmuerzoDescripcion;
            dieta.DietaMeriendaCalorias = oldContDieta.Dieta.DietaMeriendaCalorias;
            dieta.DietaMeriendaDescripcion = oldContDieta.Dieta.DietaMeriendaDescripcion;
            dieta.DietaCenaCalorias = oldContDieta.Dieta.DietaCenaCalorias;
            dieta.DietaCenaDescripcion = oldContDieta.Dieta.DietaCenaDescripcion;
            dieta.DietaIngestasCalorias = oldContDieta.Dieta.DietaIngestasCalorias;
            dieta.DietaIngestasDescripcion = oldContDieta.Dieta.DietaIngestasDescripcion;

            return View(dieta);
        }

        [HttpPost]
        [Route("editar")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DietaViewModel datos)
        {
            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);

            if (Fachada.Functions.es_premium(idUsu) == false)
            {
                TempData["PermisoDenegado"] = true;
                return RedirectToAction("Index", "Home");
            }


            if (ModelState.IsValid)
            {
                ModelCL.Contenido contDieta = db.Contenido.Find(datos.ContenidoId);

                contDieta.ContenidoTitulo = datos.ContenidoTitulo;
                contDieta.ContenidoDescripcion = datos.ContenidoDescripcion;
                contDieta.ContenidoCuerpo = datos.ContenidoCuerpo;

                contDieta.Dieta.DietaDesayunoCalorias = datos.DietaDesayunoCalorias;
                contDieta.Dieta.DietaDesayunoDescripcion = datos.DietaDesayunoDescripcion;
                contDieta.Dieta.DietaAlmuerzoCalorias = datos.DietaAlmuerzoCalorias;
                contDieta.Dieta.DietaAlmuerzoDescripcion = datos.DietaAlmuerzoDescripcion;
                contDieta.Dieta.DietaMeriendaCalorias = datos.DietaMeriendaCalorias;
                contDieta.Dieta.DietaMeriendaDescripcion = datos.DietaMeriendaDescripcion;
                contDieta.Dieta.DietaCenaCalorias = datos.DietaCenaCalorias;
                contDieta.Dieta.DietaCenaDescripcion = datos.DietaCenaDescripcion;
                contDieta.Dieta.DietaIngestasCalorias = datos.DietaIngestasCalorias;
                contDieta.Dieta.DietaIngestasDescripcion = datos.DietaIngestasDescripcion;

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ErrorMessage = "Error inesperado";
            return View(datos);
        }

        public ActionResult comenzarDieta(long idDieta)
        {
            long idPer = Convert.ToInt64(Request.Cookies["cookiePer"]["PerId"]);
            ModelCL.Persona Persona = db.Persona.Find(idPer);

            ModelCL.Dieta Dieta = db.Dieta.Find(idDieta);


            Persona.Dieta = Dieta;
            db.SaveChanges();

            return null;
        }

        public ActionResult abandonarDieta()
        {
            long idPer = Convert.ToInt64(Request.Cookies["cookiePer"]["PerId"]);
            ModelCL.Persona Persona = db.Persona.Find(idPer);

            Persona.Dieta = null;

            db.SaveChanges();

            return null;
        }

        [Route("eliminar")]
        //[ValidateAntiForgeryToken]
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModelCL.Contenido contenido = db.Contenido.Where(c => c.ContenidoId == id && c.Dieta != null).FirstOrDefault();

            if (contenido == null)
            {
                return HttpNotFound();
            }

            db.Contenido.Remove(contenido);
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
