using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelCL;
using Lucy.Models;
using System.Web.Security;

namespace Lucy.Controllers
{
    [Authorize]
    [RoutePrefix("registros/actividad")]
    public class RegActividadController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        [Route("index")]
        public ActionResult Index()
        {
            //long idPer = Fachada.Functions.get_idPer(Request.Cookies[FormsAuthentication.FormsCookieName]);
            long idPer = 1;

            List<ModelCL.Registro> registrosActividad = db.Registro.Where(r => r.Actividad != null && r.Persona.PersonaId == idPer).OrderByDescending(r => r.RegistroFchHora).ToList();

            return View(registrosActividad);
        }

        [Route("create")]
        public ActionResult Create()
        {
            RegActividadViewModel newRegActividad = new RegActividadViewModel();


            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);

            List<ModelCL.Contenido> lContEje = db.Contenido.Where(e => e.Ejercicio != null && (e.UsuarioAutor == null || e.UsuarioAutor.UsuarioId == idUsu)).ToList();
            ViewBag.lContEje = new SelectList(lContEje, "ContenidoId", "ContenidoTitulo");

            return View(newRegActividad);
        }

        [HttpPost]
        [Route("create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RegActividadViewModel datos)
        {
            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);

            List<ModelCL.Contenido> lContEje = db.Contenido.Where(e => e.Ejercicio != null && (e.UsuarioAutor == null || e.UsuarioAutor.UsuarioId == idUsu)).ToList();
            ViewBag.lContEje = new SelectList(lContEje, "ContenidoId", "ContenidoTitulo");

            if (ModelState.IsValid)
            {
                //long idPer = Fachada.Functions.get_idPer(Request.Cookies[FormsAuthentication.FormsCookieName]);
                long idPer = 1;

                DateTime f = Convert.ToDateTime(datos.RegistroFchHora);
                List<ModelCL.Registro> regActividadEx = db.Registro.Where(r => r.Actividad != null && r.Persona.PersonaId == idPer && r.RegistroFchHora == f).ToList();

                if (regActividadEx.Count() != 0)
                {
                    short totalTiempo = 0;
                    foreach (ModelCL.Registro r in regActividadEx)
                    {
                        totalTiempo += r.Actividad.ActividadTiempo;
                    }

                    totalTiempo += datos.ActividadTiempo;

                    if (totalTiempo > 720)
                    {
                        ViewBag.ErrorMessage = "El total de tiempo de este ejercicio y los anteriores registrados para esta fecha es de " + totalTiempo + ". Consideramos (de forma exagerada) que una persona no puede hacer ejercicio por mas de 12 horas (720 minutos) al día.";
                        return View(datos);
                    }
                }

                ModelCL.Persona Persona = db.Persona.Find(idPer);

                ModelCL.Registro regActividad = new ModelCL.Registro();
                regActividad.RegistroFchHora = f;

                ModelCL.Actividad actividad = new ModelCL.Actividad();

                actividad.EjercicioId = datos.EjercicioId;
                actividad.ActividadTiempo = datos.ActividadTiempo;

                regActividad.Actividad = actividad;

                Persona.Registro.Add(regActividad);

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(datos);
        }

        [Route("edit")]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModelCL.Registro regActividad = db.Registro.Where(r => r.RegistroId == id).FirstOrDefault();
            if (regActividad.Actividad == null)
            {
                return HttpNotFound();
            }

            RegActividadViewModel vmRegActividad = new RegActividadViewModel();
            vmRegActividad.RegistroId = regActividad.RegistroId;
            vmRegActividad.RegistroFchHora = regActividad.RegistroFchHora.ToString();
            vmRegActividad.EjercicioId = regActividad.Actividad.EjercicioId;
            vmRegActividad.ActividadTiempo = regActividad.Actividad.ActividadTiempo;


            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);

            List<ModelCL.Contenido> lContEje = db.Contenido.Where(e => e.Ejercicio != null && (e.UsuarioAutor == null || e.UsuarioAutor.UsuarioId == idUsu)).ToList();
            ViewBag.lContEje = new SelectList(lContEje, "ContenidoId", "ContenidoTitulo");

            return View(vmRegActividad);
        }

        [HttpPost]
        [Route("edit")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RegActividadViewModel datos)
        {
            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);

            List<ModelCL.Contenido> lContEje = db.Contenido.Where(e => e.Ejercicio != null && (e.UsuarioAutor == null || e.UsuarioAutor.UsuarioId == idUsu)).ToList();
            ViewBag.lContEje = new SelectList(lContEje, "ContenidoId", "ContenidoTitulo");


            if (ModelState.IsValid)
            {
                ModelCL.Registro regActividad = db.Registro.Where(r => r.RegistroId == datos.RegistroId).FirstOrDefault();

                DateTime f = Convert.ToDateTime(datos.RegistroFchHora);
                List<ModelCL.Registro> regActividadEx = db.Registro.Where(r => r.Actividad != null && r.Persona.PersonaId == regActividad.PersonaId && r.RegistroFchHora == f).ToList();

                if (regActividadEx.Count() != 0)
                {
                    short totalTiempo = 0;
                    foreach (ModelCL.Registro r in regActividadEx)
                    {
                        totalTiempo += r.Actividad.ActividadTiempo;
                    }

                    totalTiempo += datos.ActividadTiempo;

                    if (totalTiempo > 720)
                    {
                        ViewBag.ErrorMessage = "El total de tiempo de este ejercicio y los anteriores registrados para esta fecha es de " + totalTiempo + ". Consideramos (de forma exagerada) que una persona no puede hacer ejercicio por mas de 12 horas (720 minutos) al día.";
                        return View(datos);
                    }
                }

                regActividad.RegistroFchHora = f;
                regActividad.Actividad.EjercicioId = datos.EjercicioId;
                regActividad.Actividad.ActividadTiempo = datos.ActividadTiempo;

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(datos);
        }

        [Route("delete")]
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModelCL.Registro regActividad = db.Registro.Where(r => r.RegistroId == id).FirstOrDefault();
            if (regActividad.Actividad == null)
            {
                return HttpNotFound();
            }
            return View(regActividad);
        }

        [Route("delete")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ModelCL.Registro regActividad = db.Registro.Where(r => r.RegistroId == id).FirstOrDefault();
            db.Registro.Remove(regActividad);
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
