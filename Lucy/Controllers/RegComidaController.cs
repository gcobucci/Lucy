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
    [RoutePrefix("registros/comida")]
    public class RegComidaController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        [Route("listado")]
        public ActionResult Index()
        {
            long idPer = Convert.ToInt64(Request.Cookies["cookiePer"]["PerId"]);

            List<ModelCL.Registro> registrosComida = db.Registro.Where(r => r.Comida != null && (r.Persona.PersonaId == idPer)).OrderByDescending(r => r.RegistroFchHora).ToList();

            return View(registrosComida);
        }

        [Route("ver")]
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
                        
            ModelCL.Registro regComida = db.Registro.Where(r => r.RegistroId == id).FirstOrDefault();
            if (regComida.Comida == null)
            {
                return HttpNotFound();
            }

            return View(regComida);
        }

        [Route("crear")]
        public ActionResult Create()
        {
            RegComidaViewModel newRegComida = new RegComidaViewModel();


            List<Fachada.ViewModelSelectListChk> lComidas = new List<Fachada.ViewModelSelectListChk>()
            {
                new Fachada.ViewModelSelectListChk { Id = "Desayuno", Valor = "Desayuno" },
                new Fachada.ViewModelSelectListChk { Id = "Almuerzo", Valor = "Almuerzo" },
                new Fachada.ViewModelSelectListChk { Id = "Merienda", Valor = "Merienda" },
                new Fachada.ViewModelSelectListChk { Id = "Cena", Valor = "Cena" },
                new Fachada.ViewModelSelectListChk { Id = "Ingesta", Valor = "Ingesta" },
            };
            ViewBag.lComidas = new SelectList(lComidas, "Id", "Valor");


            int idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);

            List<ModelCL.Alimento> lAlimentos = db.Alimento.Where(a => a.Usuario == null || a.Usuario.UsuarioId == idUsu).ToList();
            newRegComida.Alimentos = new List<ComidaAlimentoViewModel>();
            foreach (ModelCL.Alimento ali in lAlimentos)
            {
                newRegComida.Alimentos.Add(new ComidaAlimentoViewModel { AlimentoId = ali.AlimentoId, AlimentoNombre = ali.AlimentoNombre, AlimentoPorcion = ali.AlimentoPorcion, AlimentoCalorias = ali.AlimentoCalorias, AlimentoCarbohidratos = ali.AlimentoCarbohidratos, AlimentoAzucar = ali.AlimentoAzucar, AlimentoGrasa = ali.AlimentoGrasa, AlimentoSodio = ali.AlimentoSodio, AlimentoGluten = ali.AlimentoGluten });
            }

            return View(newRegComida);
        }

        [HttpPost]
        [Route("crear")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RegComidaViewModel datos)
        {
            List<Fachada.ViewModelSelectListChk> lComidas = new List<Fachada.ViewModelSelectListChk>()
            {
                new Fachada.ViewModelSelectListChk { Id = "Desayuno", Valor = "Desayuno" },
                new Fachada.ViewModelSelectListChk { Id = "Almuerzo", Valor = "Almuerzo" },
                new Fachada.ViewModelSelectListChk { Id = "Merienda", Valor = "Merienda" },
                new Fachada.ViewModelSelectListChk { Id = "Cena", Valor = "Cena" },
                new Fachada.ViewModelSelectListChk { Id = "Ingesta", Valor = "Ingesta" },
            };
            ViewBag.lComidas = new SelectList(lComidas, "Id", "Valor");            

            //var errors = ModelState
            //.Where(x => x.Value.Errors.Count > 0)
            //.Select(x => new { x.Key, x.Value.Errors })
            //.ToArray();

            if (ModelState.IsValid)
            {
                long idPer = Convert.ToInt64(Request.Cookies["cookiePer"]["PerId"]);

                DateTime f = Convert.ToDateTime(datos.RegistroFchHora);

                if (datos.ComidaComida != "Ingesta")
                {
                    ModelCL.Registro regComidaEx = db.Registro.Where(r => r.Comida != null && r.Persona.PersonaId == idPer && r.RegistroFchHora/*.Date*/ == f/*.Date*/).FirstOrDefault();

                    if (regComidaEx != null)
                    {
                        ViewBag.ErrorMessage = "Ya existe un/a " + datos.ComidaComida + " registrado/a en esta fecha. Puede modificarlo si lo desea.";
                        return View(datos);
                    }
                }
                
                ModelCL.Persona Persona = db.Persona.Find(idPer);

                ModelCL.Registro regComida = new ModelCL.Registro();
                regComida.RegistroFchHora = Convert.ToDateTime(datos.RegistroFchHora);

                ModelCL.Comida comida = new ModelCL.Comida();

                comida.ComidaPlatillo = datos.ComidaPlatillo;
                comida.ComidaComida = datos.ComidaComida;

                foreach (ComidaAlimentoViewModel a in datos.Alimentos)
                {
                    if (a.RelComAliCantidad != 0)
                    {
                        comida.RelComAli.Add(new ModelCL.RelComAli { Alimento = db.Alimento.Find(a.AlimentoId), ReComAliCantidad = a.RelComAliCantidad });
                    }
                }

                comida.ComidaCalorias = datos.ComidaCalorias;
                comida.ComidaCarbohidratos = datos.ComidaCarbohidratos;
                comida.ComidaAzucar = datos.ComidaAzucar;
                comida.ComidaGrasa = datos.ComidaGrasa;
                comida.ComidaSodio = datos.ComidaSodio;
                comida.ComidaGluten = datos.ComidaGluten;
                                
                regComida.Comida  = comida;
                Persona.Registro.Add(regComida);
                
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

            ModelCL.Registro regComida = db.Registro.Where(r => r.RegistroId == id).FirstOrDefault();
            if (regComida.Comida == null)
            {
                return HttpNotFound();
            }


            RegComidaViewModel vmRegComida = new RegComidaViewModel();


            List<Fachada.ViewModelSelectListChk> lComidas = new List<Fachada.ViewModelSelectListChk>()
            {
                new Fachada.ViewModelSelectListChk { Id = "Desayuno", Valor = "Desayuno" },
                new Fachada.ViewModelSelectListChk { Id = "Almuerzo", Valor = "Almuerzo" },
                new Fachada.ViewModelSelectListChk { Id = "Merienda", Valor = "Merienda" },
                new Fachada.ViewModelSelectListChk { Id = "Cena", Valor = "Cena" },
                new Fachada.ViewModelSelectListChk { Id = "Ingesta", Valor = "Ingesta" },
            };
            ViewBag.lComidas = new SelectList(lComidas, "Id", "Valor");


            int idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);

            List<ModelCL.Alimento> lAlimentos = db.Alimento.Where(a => a.Usuario == null || a.Usuario.UsuarioId == idUsu).ToList();
            vmRegComida.Alimentos = new List<ComidaAlimentoViewModel>();
            foreach (ModelCL.Alimento ali in lAlimentos)
            {
                vmRegComida.Alimentos.Add(new ComidaAlimentoViewModel { AlimentoId = ali.AlimentoId, AlimentoNombre = ali.AlimentoNombre, AlimentoPorcion = ali.AlimentoPorcion, AlimentoCalorias = ali.AlimentoCalorias, AlimentoCarbohidratos = ali.AlimentoCarbohidratos, AlimentoAzucar = ali.AlimentoAzucar, AlimentoGrasa = ali.AlimentoGrasa, AlimentoSodio = ali.AlimentoSodio, AlimentoGluten = ali.AlimentoGluten });
            }


            vmRegComida.RegistroId = regComida.RegistroId;
            //vmRegComida.PersonaId = regComida.PersonaId;
            vmRegComida.RegistroFchHora = regComida.RegistroFchHora.ToString();
            vmRegComida.ComidaPlatillo = regComida.Comida.ComidaPlatillo;
            vmRegComida.ComidaComida = regComida.Comida.ComidaComida;
            //vmRegComida.Alimentos = regComida.Comida.RelComAli.ToList();
            foreach (ModelCL.RelComAli rca in regComida.Comida.RelComAli.ToList())
            {
                ComidaAlimentoViewModel cavm = vmRegComida.Alimentos.Where(a => a.AlimentoId == rca.AlimentoId).FirstOrDefault();
                cavm.RelComAliCantidad = rca.ReComAliCantidad;
            }
            vmRegComida.ComidaCalorias = regComida.Comida.ComidaCalorias;
            vmRegComida.ComidaCarbohidratos = regComida.Comida.ComidaCarbohidratos;
            vmRegComida.ComidaAzucar = regComida.Comida.ComidaAzucar;
            vmRegComida.ComidaGrasa = regComida.Comida.ComidaGrasa;
            vmRegComida.ComidaSodio = regComida.Comida.ComidaSodio;
            vmRegComida.ComidaGluten = regComida.Comida.ComidaGluten;
                      
            return View(vmRegComida);
        }

        [HttpPost]
        [Route("editar")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RegComidaViewModel datos)
        {
            List<Fachada.ViewModelSelectListChk> lComidas = new List<Fachada.ViewModelSelectListChk>()
            {
                new Fachada.ViewModelSelectListChk { Id = "Desayuno", Valor = "Desayuno" },
                new Fachada.ViewModelSelectListChk { Id = "Almuerzo", Valor = "Almuerzo" },
                new Fachada.ViewModelSelectListChk { Id = "Merienda", Valor = "Merienda" },
                new Fachada.ViewModelSelectListChk { Id = "Cena", Valor = "Cena" },
                new Fachada.ViewModelSelectListChk { Id = "Ingesta", Valor = "Ingesta" },
            };
            ViewBag.lComidas = new SelectList(lComidas, "Id", "Valor");
            
            
            if (ModelState.IsValid)
            {
                ModelCL.Registro regComida = db.Registro.Where(r => r.RegistroId == datos.RegistroId).FirstOrDefault();

                DateTime f = Convert.ToDateTime(datos.RegistroFchHora);
                if (datos.ComidaComida != "Ingesta")
                {
                    if (regComida.RegistroFchHora != f)
                    {
                        ModelCL.Registro regComidaEx = db.Registro.Where(r => r.Comida != null && r.Persona.PersonaId == regComida.PersonaId && r.RegistroFchHora == f).FirstOrDefault();

                        if (regComidaEx != null)
                        {
                            ViewBag.ErrorMessage = "Ya existe un/a " + datos.ComidaComida + " registrado/a en esta fecha. Puede modificarlo si lo desea.";
                            return View(datos);
                        }
                    }
                }                

                regComida.RegistroFchHora = f;
                regComida.Comida.ComidaPlatillo = datos.ComidaPlatillo;
                regComida.Comida.ComidaComida = datos.ComidaComida;

                List<ModelCL.RelComAli> bkRelComAli = regComida.Comida.RelComAli.ToList();
                foreach (ModelCL.RelComAli oldRelComAli in bkRelComAli)
                {
                    regComida.Comida.RelComAli.Remove(oldRelComAli);
                }

                foreach (ComidaAlimentoViewModel a in datos.Alimentos)
                {
                    if (a.RelComAliCantidad != 0)
                    {
                        regComida.Comida.RelComAli.Add(new ModelCL.RelComAli { Alimento = db.Alimento.Find(a.AlimentoId), ReComAliCantidad = a.RelComAliCantidad });
                    }
                }

                regComida.Comida.ComidaCalorias = datos.ComidaCalorias;
                regComida.Comida.ComidaCarbohidratos = datos.ComidaCarbohidratos;
                regComida.Comida.ComidaAzucar = datos.ComidaAzucar;
                regComida.Comida.ComidaGrasa = datos.ComidaGrasa;
                regComida.Comida.ComidaSodio = datos.ComidaSodio;
                regComida.Comida.ComidaGluten = datos.ComidaGluten;

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
            ModelCL.Registro regComida = db.Registro.Where(r => r.RegistroId == id).FirstOrDefault();
            if (regComida.Comida == null)
            {
                return HttpNotFound();
            }

            return View(regComida);
        }

        [Route("eliminar")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ModelCL.Registro regComida = db.Registro.Where(r => r.RegistroId == id).FirstOrDefault();
            db.Registro.Remove(regComida);
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
