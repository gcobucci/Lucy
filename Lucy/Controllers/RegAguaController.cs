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

namespace Lucy.Controllers
{
    [Authorize]
    [RoutePrefix("registros/agua")]
    public class RegAguaController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        [Route("listado")]
        public ActionResult Index()
        {
            long idPer = Convert.ToInt64(Request.Cookies["cookiePer"]["PerId"]);

            List<ModelCL.Registro> registrosAgua = db.Registro.Where(r => r.Agua != null && (r.Persona.PersonaId == idPer)).OrderByDescending(r => r.RegistroFchHora).ToList();

            return View(registrosAgua);
        }

        [Route("crear")]
        public ActionResult Create()
        {
            RegAguaViewModel newRegAgua = new RegAguaViewModel();
            //ViewBag.Confirmacion = false;
            return View(newRegAgua);
        }

        [HttpPost]
        [Route("crear")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RegAguaViewModel datos)
        {
            if (ModelState.IsValid)
            {
                long idPer = Convert.ToInt64(Request.Cookies["cookiePer"]["PerId"]);

                DateTime f = Convert.ToDateTime(datos.RegistroFchHora);
                ModelCL.Registro regAguaEx = db.Registro.Where(r => r.Agua != null && r.Persona.PersonaId == idPer && r.RegistroFchHora == f).FirstOrDefault();

                if (regAguaEx == null)
                {
                    if (datos.confirmacion == false)
                    {
                        if ((datos.AguaCantidad) > 20)
                        {
                            ViewBag.ConfirmationMessage = "No es saludable tomar tanta agua por día, ¿Quieres registrar " + datos.AguaCantidad + " litro(s) de todas formas? ";
                            return View(datos);
                        }
                    }

                    ModelCL.Persona Persona = db.Persona.Find(idPer);

                    ModelCL.Registro regAgua = new ModelCL.Registro();
                    regAgua.RegistroFchHora = Convert.ToDateTime(datos.RegistroFchHora);

                    ModelCL.Agua agua = new ModelCL.Agua();

                    agua.AguaCantidad = datos.AguaCantidad;
                    regAgua.Agua = agua;

                    Persona.Registro.Add(regAgua);
                }
                else
                {
                    if (datos.confirmacion == true)
                    {                        
                        regAguaEx.Agua.AguaCantidad += datos.AguaCantidad;
                    }
                    else
                    {
                        double totalAgua = regAguaEx.Agua.AguaCantidad + datos.AguaCantidad;
                        if ((totalAgua) > 20)
                        {
                            ViewBag.ConfirmationMessage = "No es saludable tomar tanta agua por día, ya has registrado " + regAguaEx.Agua.AguaCantidad + " litros de agua en esta fecha ¿Quieres registrar " + datos.AguaCantidad + " litro(s) más de todas formas? ";
                            return View(datos);
                        }
                        else
                        {
                            ViewBag.ConfirmationMessage = "Ya tienes " + regAguaEx.Agua.AguaCantidad + " litros de agua registrados en esta fecha. ¿Deseas registrar " + datos.AguaCantidad + " litro(s) más?";
                            return View(datos);
                        }                        
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(datos);
        }

        [Route("editar")]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModelCL.Registro regAgua = db.Registro.Where(r => r.RegistroId == id).FirstOrDefault();
            if (regAgua.Agua == null)
            {
                return HttpNotFound();
            }

            RegAguaViewModel vmRegAgua = new RegAguaViewModel();
            vmRegAgua.RegistroId = regAgua.RegistroId;
            //vmRegAgua.PersonaId = regAgua.PersonaId;
            vmRegAgua.RegistroFchHora = regAgua.RegistroFchHora.ToString();
            vmRegAgua.AguaCantidad = regAgua.Agua.AguaCantidad;

            return View(vmRegAgua);
        }

        [HttpPost]
        [Route("editar")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RegAguaViewModel datos, bool confirmacion = false)
        {
            if (ModelState.IsValid)
            {
                ModelCL.Registro regAgua = db.Registro.Where(r => r.RegistroId == datos.RegistroId).FirstOrDefault();

                DateTime f = Convert.ToDateTime(datos.RegistroFchHora);
                if (regAgua.RegistroFchHora != f)
                {
                    ModelCL.Registro regAguaEx = db.Registro.Where(r => r.Agua != null && r.Persona.PersonaId == regAgua.PersonaId && r.RegistroFchHora == f).FirstOrDefault();

                    if (regAguaEx != null)
                    {
                        if (confirmacion == true)
                        {
                            db.Registro.Remove(regAgua);

                            double totalAgua = regAguaEx.Agua.AguaCantidad + datos.AguaCantidad;
                            if ((totalAgua) > 20)
                            {
                                ViewBag.ConfirmationMessage = "No es saludable tomar tanta agua por día, ya has registrado " + regAguaEx.Agua.AguaCantidad + " litros de agua en esta fecha ¿Quieres registrar " + datos.AguaCantidad + " litro(s) más de todas formas? ";
                                return View(datos);
                            }

                            regAguaEx.Agua.AguaCantidad += datos.AguaCantidad;
                        }
                        else
                        {
                            ViewBag.ConfirmationMessage = "Ya tienes " + regAguaEx.Agua.AguaCantidad + " litros de agua registrados en esta fecha. ¿Deseas registrar " + datos.AguaCantidad + " litro(s) más?";
                            return View(datos);
                        }              
                    }
                }
                else
                {
                    regAgua.RegistroFchHora = f;
                    regAgua.Agua.AguaCantidad = datos.AguaCantidad;
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(datos);
        }

        [Route("eliminar")]
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModelCL.Registro regAgua = db.Registro.Where(r => r.RegistroId == id).FirstOrDefault();
            if (regAgua.Agua == null)
            {
                return HttpNotFound();
            }
            return View(regAgua);
        }

        [Route("eliminar")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ModelCL.Registro regAgua = db.Registro.Where(r => r.RegistroId == id).FirstOrDefault();
            db.Registro.Remove(regAgua);
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
