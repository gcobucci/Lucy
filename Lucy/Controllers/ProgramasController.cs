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



            var programas = db.Contenido.Where(c => c.Programa != null && (c.UsuarioAutor == null || c.UsuarioAutor.UsuarioId == idUsu)).OrderBy(c => c.ContenidoTitulo).ToList();

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
            ViewBag.idUsu = idUsu;


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

        [Route("crear")]
        public ActionResult Create()
        {
            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);

            if (Fachada.Functions.es_premium(idUsu) == false)
            {
                TempData["PermisoDenegado"] = true;
                return RedirectToAction("Index", "Home");
            }


            ProgramaViewModel newPrograma = new ProgramaViewModel();

            List<ModelCL.Contenido> lContRutinas = db.Contenido.Where(c => c.Rutina != null && (c.UsuarioAutor == null || (c.UsuarioAutor != null && c.UsuarioAutor.UsuarioId == idUsu))).ToList();
            List<Fachada.ViewModelCheckBox> lRut = new List<Fachada.ViewModelCheckBox>();
            foreach (ModelCL.Contenido contRut in lContRutinas)
            {
                lRut.Add(new Fachada.ViewModelCheckBox() { Id = contRut.ContenidoId, Nombre = contRut.ContenidoTitulo });
            }

            newPrograma.Rutinas = lRut;

            return View(newPrograma);
        }

        [HttpPost]
        [Route("crear")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProgramaViewModel datos)
        {
            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);

            if (Fachada.Functions.es_premium(idUsu) == false)
            {
                TempData["PermisoDenegado"] = true;
                return RedirectToAction("Index", "Home");
            }


            if (ModelState.IsValid)
            {
                if (datos.Rutinas.Where(e => e.Checked == true).Count() == 0)
                {
                    ViewBag.ErrorMessage = "Debe seleccionar al menos una rutina";
                    return View(datos);
                }

                ModelCL.Contenido newContProg = new ModelCL.Contenido();
                newContProg.ContenidoTitulo = datos.ContenidoTitulo;
                newContProg.ContenidoDescripcion = datos.ContenidoDescripcion;
                newContProg.ContenidoCuerpo = datos.ContenidoCuerpo;

                ModelCL.Programa programa = new ModelCL.Programa();

                foreach (var rut in datos.Rutinas.Where(e => e.Checked == true))
                {
                    programa.Rutina.Add(db.Rutina.Find(rut.Id));
                }

                newContProg.Programa = programa;
                newContProg.UsuarioAutor = db.Usuario.Find(idUsu);

                db.Contenido.Add(newContProg);
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

            ModelCL.Contenido oldContPrograma = db.Contenido.Find(id);
            if (oldContPrograma == null || oldContPrograma.Programa == null)
            {
                return HttpNotFound();
            }


            ProgramaViewModel programa = new ProgramaViewModel();

            programa.ContenidoId = oldContPrograma.ContenidoId;
            programa.ContenidoTitulo = oldContPrograma.ContenidoTitulo;
            programa.ContenidoDescripcion = oldContPrograma.ContenidoDescripcion;
            programa.ContenidoCuerpo = oldContPrograma.ContenidoCuerpo;


            List<ModelCL.Contenido> lContRutinas = db.Contenido.Where(c => c.Rutina != null && (c.UsuarioAutor == null || (c.UsuarioAutor != null && c.UsuarioAutor.UsuarioId == idUsu))).ToList();
            List<Fachada.ViewModelCheckBox> lRut = new List<Fachada.ViewModelCheckBox>();
            foreach (ModelCL.Contenido contRut in lContRutinas)
            {
                lRut.Add(new Fachada.ViewModelCheckBox() { Id = contRut.ContenidoId, Nombre = contRut.ContenidoTitulo });
            }

            foreach (Fachada.ViewModelCheckBox rut in lRut)
            {
                ModelCL.Rutina ru = oldContPrograma.Programa.Rutina.Where(e => e.RutinaId == rut.Id).FirstOrDefault();

                if (ru != null)
                {
                    rut.Checked = true;
                }
            }

            programa.Rutinas = lRut;

            return View(programa);
        }

        [HttpPost]
        [Route("editar")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProgramaViewModel datos)
        {
            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);

            if (Fachada.Functions.es_premium(idUsu) == false)
            {
                TempData["PermisoDenegado"] = true;
                return RedirectToAction("Index", "Home");
            }


            if (ModelState.IsValid)
            {
                if (datos.Rutinas.Where(e => e.Checked == true).Count() == 0)
                {
                    ViewBag.ErrorMessage = "Debe seleccionar al menos una rutina";
                    return View(datos);
                }

                ModelCL.Contenido contPrograma = db.Contenido.Find(datos.ContenidoId);

                contPrograma.ContenidoTitulo = datos.ContenidoTitulo;
                contPrograma.ContenidoDescripcion = datos.ContenidoDescripcion;
                contPrograma.ContenidoCuerpo = datos.ContenidoCuerpo;

                List<ModelCL.Rutina> bkRutinas = contPrograma.Programa.Rutina.ToList();
                foreach (ModelCL.Rutina oldRutina in bkRutinas)
                {
                    contPrograma.Programa.Rutina.Remove(oldRutina);
                }

                foreach (var eje in datos.Rutinas.Where(e => e.Checked == true))
                {
                    contPrograma.Programa.Rutina.Add(db.Rutina.Where(e => e.RutinaId == eje.Id).FirstOrDefault());
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ErrorMessage = "Error inesperado";
            return View(datos);
        }

        [Route("eliminar")]
        //[ValidateAntiForgeryToken]
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModelCL.Contenido contenido = db.Contenido.Where(c => c.ContenidoId == id && c.Programa != null).FirstOrDefault();

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
