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
    [RoutePrefix("rutinas")]
    public class RutinasController : Controller
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



            var rutinas = db.Contenido.Where(c => c.Rutina != null && (c.UsuarioAutor == null || c.UsuarioAutor.UsuarioId == idUsu)).ToList();

            return View(rutinas);
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



            ModelCL.Contenido contRutina = db.Contenido.Find(id);
            if (contRutina.Rutina == null || (contRutina.UsuarioAutor != null && contRutina.UsuarioAutor.UsuarioId != idUsu))
            {
                return HttpNotFound();
            }

            contRutina.ContenidoCantVisitas += 1;
            db.SaveChanges();

            return View(contRutina);
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


            RutinaViewModel newRutina = new RutinaViewModel();

            List<ModelCL.Contenido> lContEjercicios = db.Contenido.Where(c => c.Ejercicio != null && (c.UsuarioAutor == null || (c.UsuarioAutor != null && c.UsuarioAutor.UsuarioId == idUsu))).ToList();
            List<Fachada.ViewModelCheckBox> lEje = new List<Fachada.ViewModelCheckBox>();
            foreach (ModelCL.Contenido contEje in lContEjercicios)
            {
                lEje.Add(new Fachada.ViewModelCheckBox() { Id = contEje.ContenidoId, Nombre = contEje.ContenidoTitulo });
            }

            newRutina.Ejercicios = lEje;           

            return View(newRutina);
        }

        [HttpPost]
        [Route("crear")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RutinaViewModel datos)
        {
            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);

            if (Fachada.Functions.es_premium(idUsu) == false)
            {
                TempData["PermisoDenegado"] = true;
                return RedirectToAction("Index", "Home");
            }


            if (ModelState.IsValid)
            {
                if (datos.Ejercicios.Where(e => e.Checked == true).Count() == 0)
                {
                    ViewBag.ErrorMessage = "Debe seleccionar al menos un ejercicio";
                    return View(datos);
                }

                ModelCL.Contenido newContRut = new ModelCL.Contenido();
                newContRut.ContenidoTitulo = datos.ContenidoTitulo;
                newContRut.ContenidoDescripcion = datos.ContenidoDescripcion;
                newContRut.ContenidoCuerpo = datos.ContenidoCuerpo;

                ModelCL.Rutina rutina = new ModelCL.Rutina();

                foreach (var eje in datos.Ejercicios.Where(e => e.Checked == true))
                {
                    rutina.Ejercicio.Add(db.Ejercicio.Find(eje.Id));
                }          

                newContRut.Rutina = rutina;
                newContRut.UsuarioAutor = db.Usuario.Find(idUsu);
                                
                db.Contenido.Add(newContRut);
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

            ModelCL.Contenido oldContRutina = db.Contenido.Find(id);
            if (oldContRutina == null || oldContRutina.Rutina == null)
            {
                return HttpNotFound();
            }


            RutinaViewModel rutina = new RutinaViewModel();

            rutina.ContenidoId = oldContRutina.ContenidoId;
            rutina.ContenidoTitulo = oldContRutina.ContenidoTitulo;
            rutina.ContenidoDescripcion = oldContRutina.ContenidoDescripcion;
            rutina.ContenidoCuerpo = oldContRutina.ContenidoCuerpo;


            List<ModelCL.Contenido> lContEjercicios = db.Contenido.Where(c => c.Ejercicio != null && (c.UsuarioAutor == null || (c.UsuarioAutor != null && c.UsuarioAutor.UsuarioId == idUsu))).ToList();
            List<Fachada.ViewModelCheckBox> lEje = new List<Fachada.ViewModelCheckBox>();
            foreach (ModelCL.Contenido contEje in lContEjercicios)
            {
                lEje.Add(new Fachada.ViewModelCheckBox() { Id = contEje.ContenidoId, Nombre = contEje.ContenidoTitulo });
            }

            foreach (Fachada.ViewModelCheckBox eje in lEje)
            {
                ModelCL.Ejercicio ej = oldContRutina.Rutina.Ejercicio.Where(e => e.EjercicioId == eje.Id).FirstOrDefault();

                if (ej != null)
                {
                    eje.Checked = true;
                }
            }

            rutina.Ejercicios = lEje;

            return View(rutina);
        }

        [HttpPost]
        [Route("editar")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RutinaViewModel datos)
        {
            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);

            if (Fachada.Functions.es_premium(idUsu) == false)
            {
                TempData["PermisoDenegado"] = true;
                return RedirectToAction("Index", "Home");
            }


            if (ModelState.IsValid)
            {
                if (datos.Ejercicios.Where(e => e.Checked == true).Count() == 0)
                {
                    ViewBag.ErrorMessage = "Debe seleccionar al menos un ejercicio";
                    return View(datos);
                }

                ModelCL.Contenido contRutina = db.Contenido.Find(datos.ContenidoId);

                contRutina.ContenidoTitulo = datos.ContenidoTitulo;
                contRutina.ContenidoDescripcion = datos.ContenidoDescripcion;
                contRutina.ContenidoCuerpo = datos.ContenidoCuerpo;

                List<ModelCL.Ejercicio> bkEjercicios = contRutina.Rutina.Ejercicio.ToList();
                foreach (ModelCL.Ejercicio oldEjercicio in bkEjercicios)
                {
                    contRutina.Rutina.Ejercicio.Remove(oldEjercicio);
                }

                foreach (var eje in datos.Ejercicios.Where(e => e.Checked == true))
                {
                    contRutina.Rutina.Ejercicio.Add(db.Ejercicio.Where(e => e.EjercicioId == eje.Id).FirstOrDefault());
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ErrorMessage = "Error inesperado";
            return View(datos);
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
