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

        [Route("index")]
        public ActionResult Index()
        {
            //long idPer = Fachada.Functions.get_idPer(Request.Cookies[FormsAuthentication.FormsCookieName]);
            long idPer = 1;

            List<ModelCL.Registro> registrosAgua = db.Registro.Where(r => r.Agua != null && (r.Persona.PersonaId == idPer)).OrderByDescending(r => r.RegistroFchHora).ToList();

            return View(registrosAgua);
        }

        [Route("create")]
        public ActionResult Create()
        {
            RegAguaViewModel newRegAgua = new RegAguaViewModel();
            //ViewBag.Confirmacion = false;
            return View(newRegAgua);
        }

        [HttpPost]
        [Route("create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RegAguaViewModel datos, bool confirmacion = false)
        {
            if (ModelState.IsValid)
            {
                //long idPer = Fachada.Functions.get_idPer(Request.Cookies[FormsAuthentication.FormsCookieName]);
                long idPer = 1;

                DateTime f = Convert.ToDateTime(datos.RegistroFchHora);
                ModelCL.Registro regAguaEx = db.Registro.Where(r => r.Agua != null && r.Persona.PersonaId == idPer && r.RegistroFchHora == f).FirstOrDefault();

                if (regAguaEx == null)
                {
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
                    if (confirmacion == true)
                    {
                        double totalAgua = regAguaEx.Agua.AguaCantidad + datos.AguaCantidad;
                        if ((totalAgua) > 20)
                        {
                            ViewBag.ErrorMessage = "Consideramos que no es posible tomar mas de 20 litros de agua por día y la suma del valor ya registrado en esta fecha (" + regAguaEx.Agua.AguaCantidad + ") y el nuevo valor (" + datos.AguaCantidad + ") es " + totalAgua + ".";
                            return View(datos);
                        }

                        regAguaEx.Agua.AguaCantidad += datos.AguaCantidad;
                    }                   
                    else
                    {
                        //ViewBag.Confirmacion = false;
                        ViewBag.ConfirmationMessage = "La fecha ingresada ya tiene un valor registrado (" + regAguaEx.Agua.AguaCantidad + "). Si decide continuar se añadirá la cantidad de agua que esta registrando a la cantidad ya registrada. ¿Desea continuar?";
                        return View(datos);
                    }
                }

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
        [Route("edit")]
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
                                ViewBag.ErrorMessage = "Consideramos que no es posible tomar mas de 20 litros de agua por día y la suma del valor ya registrado en esta fecha (" + regAguaEx.Agua.AguaCantidad + ") y el nuevo valor (" + datos.AguaCantidad + ") es " + totalAgua + ".";
                                return View(datos);
                            }

                            regAguaEx.Agua.AguaCantidad += datos.AguaCantidad;
                        }
                        else
                        {
                            ViewBag.ConfirmationMessage = "La fecha ingresada ya tiene un valor registrado (" + regAguaEx.Agua.AguaCantidad + "). Si decide continuar se añadirá la cantidad de agua que esta registrando-modificando a la cantidad ya registrada y se borrará este registro. ¿Desea continuar?";
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

        [Route("delete")]
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

        [Route("delete")]
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
