﻿using System;
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
    [RoutePrefix("registros/control_diabetico")]
    public class RegControlDiabeticoController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        [Route("listado")]
        public ActionResult Index()
        {
            long idPer = Convert.ToInt64(Request.Cookies["cookiePer"]["PerId"]);

            if (Fachada.Functions.es_diabetico_tipo_1(idPer) == false && Fachada.Functions.fue_diabetico_tipo_1(idPer) == false)
            {
                ModelCL.Persona Persona = db.Persona.Find(idPer);

                TempData["NoCumpleRequisitos"] = "La persona -" + Persona.nombreCompleto + "- no tiene la enfermedad -Diabetes tipo 1- y por lo tanto no puede acceder a este registro.";
                return RedirectToAction("Index", "Home");
            }

            List<ModelCL.Registro> regControlDiabetico = 
                db.Registro.Where(r => ((r.Medicacion != null && (r.Medicacion.Medicina.MedicinaTipo == "Pasiva" && 
                r.Medicacion.Enfermedad.EnfermedadNombre == "Diabetes tipo 1")) || (r.Control != null && (r.Control.Valor.ValorNombre == "Glucosa"))) 
                && r.Persona.PersonaId == idPer).OrderByDescending(r => r.RegistroFchHora).ToList();                       

            return View(regControlDiabetico);
        }

        [Route("ver")]
        public ActionResult Details(long? id)
        {
            long idPer = Convert.ToInt64(Request.Cookies["cookiePer"]["PerId"]);

            if (Fachada.Functions.es_diabetico_tipo_1(idPer) == false && Fachada.Functions.fue_diabetico_tipo_1(idPer) == false)
            {
                ModelCL.Persona Persona = db.Persona.Find(idPer);

                TempData["NoCumpleRequisitos"] = "La persona " + Persona.nombreCompleto + " no tiene la enfermedad Diabetes tipo 1 y por lo tanto no puede acceder a este registro.";
                return RedirectToAction("Index", "Home");
            }


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
                        
            ModelCL.Registro regControlDiabetico = db.Registro.Where(r => r.RegistroId == id).FirstOrDefault();
            if (!(regControlDiabetico.Control != null && (regControlDiabetico.Control.Valor.ValorNombre == "Glucosa")))
            {
                return HttpNotFound();
            }

            return View(regControlDiabetico);
        }

        [Route("crear")]
        public ActionResult Create()
        {
            long idPer = Convert.ToInt64(Request.Cookies["cookiePer"]["PerId"]);

            if (Fachada.Functions.es_diabetico_tipo_1(idPer) == false)
            {
                ModelCL.Persona Persona = db.Persona.Find(idPer);

                TempData["NoCumpleRequisitos"] = "La persona -" + Persona.nombreCompleto + "- no tiene la enfermedad -Diabetes tipo 1- y por lo tanto no puede acceder a este registro.";
                return RedirectToAction("Index", "Home");
            }


            RegControlDiabeticoViewModel newRegControlDiabetico = new RegControlDiabeticoViewModel();


            //Registro de Comida//
            List<Fachada.ViewModelSelectListChk> lComidas = new List<Fachada.ViewModelSelectListChk>()
            {
                new Fachada.ViewModelSelectListChk { Id = "Desayuno", Valor = "Desayuno" },
                new Fachada.ViewModelSelectListChk { Id = "Almuerzo", Valor = "Almuerzo" },
                new Fachada.ViewModelSelectListChk { Id = "Merienda", Valor = "Merienda" },
                new Fachada.ViewModelSelectListChk { Id = "Cena", Valor = "Cena" },
                new Fachada.ViewModelSelectListChk { Id = "Ingesta", Valor = "Ingesta" },
            };
            ViewBag.lComidas = new SelectList(lComidas, "Id", "Valor");


            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);

            List<ModelCL.Alimento> lAlimentos = db.Alimento.Where(a => a.Usuario == null || a.Usuario.UsuarioId == idUsu).ToList();
            newRegControlDiabetico.Alimentos = new List<ComidaAlimentoViewModel>();
            foreach (ModelCL.Alimento ali in lAlimentos)
            {
                newRegControlDiabetico.Alimentos.Add(new ComidaAlimentoViewModel { AlimentoId = ali.AlimentoId, AlimentoNombre = ali.AlimentoNombre, AlimentoImagen = ali.AlimentoImagen, AlimentoPorcion = ali.AlimentoPorcion, AlimentoCalorias = ali.AlimentoCalorias, AlimentoCarbohidratos = ali.AlimentoCarbohidratos, AlimentoAzucar = ali.AlimentoAzucar, AlimentoGrasa = ali.AlimentoGrasa, AlimentoSodio = ali.AlimentoSodio, AlimentoGluten = ali.AlimentoGluten });
            }

            return View(newRegControlDiabetico);
        }

        [HttpPost]
        [Route("crear")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RegControlDiabeticoViewModel datos)
        {
            //Registro de Comida//
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

                ModelCL.Persona Persona = db.Persona.Find(idPer);

                DateTime f = Convert.ToDateTime(datos.RegistroFchHora);                              
                                

                ModelCL.Registro regControl = new ModelCL.Registro();
                regControl.RegistroFchHora = f;

                ModelCL.Control control = new ModelCL.Control();

                control.Valor = db.Valor.Where(v => v.ValorNombre == "Glucosa").FirstOrDefault();
                control.ControlValor = datos.ControlValor;

                regControl.Control = control;


                if (datos.RegistrarComida == true)
                {
                    if (datos.Alimentos.Where(a => a.RelComAliCantidad != 0).Count() == 0)
                    {
                        ViewBag.ErrorMessage = "Si va a registrar una comida debe seleccionar al menos un alimento.";

                        return View(datos);
                    }

                    if (datos.ComidaComida != "Ingesta")
                    {
                        ModelCL.Registro regComidaEx = db.Registro.Where(r => r.Comida != null && r.Comida.ComidaComida == datos.ComidaComida && r.Persona.PersonaId == idPer && r.RegistroFchHora/*.Date*/ == f/*.Date*/).FirstOrDefault();

                        if (regComidaEx != null)
                        {
                            ViewBag.ErrorMessage = "Ya existe un/a " + datos.ComidaComida + " registrado/a en esta fecha. Puede modificarlo si lo desea.";
                            return View(datos);
                        }
                    }
                        
                    ModelCL.Comida comida = new ModelCL.Comida();

                    comida.ComidaPlatillo = datos.ComidaPlatillo;
                    comida.ComidaComida = datos.ComidaComida;

                    foreach (ComidaAlimentoViewModel a in datos.Alimentos)
                    {
                        if (a.RelComAliCantidad != 0)
                        {
                            comida.RelComAli.Add(new ModelCL.RelComAli { Alimento = db.Alimento.Find(a.AlimentoId), RelComAliCantidad = a.RelComAliCantidad });
                        }
                    }

                    comida.ComidaCalorias = datos.ComidaCalorias;
                    comida.ComidaCarbohidratos = datos.ComidaCarbohidratos;
                    comida.ComidaAzucar = datos.ComidaAzucar;
                    comida.ComidaGrasa = datos.ComidaGrasa;
                    comida.ComidaSodio = datos.ComidaSodio;
                    comida.ComidaGluten = datos.ComidaGluten;

                    regControl.Comida = comida;
                }

                //Resultado//
                if (datos.ResultadoTotalInsulinaCorreccion != null)
                {
                    ModelCL.Medicacion medicacion = new ModelCL.Medicacion();

                    medicacion.Enfermedad = db.Enfermedad.Where(e => e.EnfermedadNombre == "Diabetes tipo 1").FirstOrDefault();
                    medicacion.Medicina = Persona.RelPerEnf.Where(rpe => rpe.Enfermedad.EnfermedadNombre == "Diabetes tipo 1").FirstOrDefault().RelMedRelPerEnf.Where(rmrpe => rmrpe.Medicina.MedicinaTipo == "Activa").FirstOrDefault().Medicina;
                    medicacion.PresentacionId = db.Presentacion.Where(p => p.PresentacionNombre == "Inyección (unidades)").FirstOrDefault().PresentacionId;
                    medicacion.MedicacionCantidad = Convert.ToDouble(datos.ResultadoTotalInsulinaCorreccion);

                    regControl.Medicacion = medicacion;
                }


                if (datos.RegistrarComida == true)
                {

                    ModelCL.Dieta dietaPer = Persona.Dieta;
                    if (dietaPer != null)
                    {
                        Nullable<bool> sePaso = null;

                        if (datos.ComidaComida == "Desayuno")
                        {
                            if (datos.ComidaCalorias > dietaPer.DietaDesayunoCalorias)
                            {
                                sePaso = true;
                            }
                            else
                            {
                                sePaso = false;
                            }


                            if (sePaso == true)
                            {
                                TempData["PostMessage"] = "Según tu dieta te pasaste de las calorías especificadas para esta comida, pero no te preocupes esta bien darse un gustito!";
                            }
                            else if (sePaso == false)
                            {
                                TempData["PostMessage"] = "Excelente, las calorías de esta comida concuerdan con tu dieta.";
                            }
                            else
                            {
                                TempData["PostMessage"] = "Error inesperado.";
                            }
                        }
                        else if (datos.ComidaComida == "Almuerzo")
                        {
                            if (datos.ComidaCalorias > dietaPer.DietaAlmuerzoCalorias)
                            {
                                sePaso = true;
                            }
                            else
                            {
                                sePaso = false;
                            }


                            if (sePaso == true)
                            {
                                TempData["PostMessage"] = "Según tu dieta te pasaste de las calorías especificadas para esta comida, pero no te preocupes esta bien darse un gustito!";
                            }
                            else if (sePaso == false)
                            {
                                TempData["PostMessage"] = "Excelente, las calorías de esta comida concuerdan con tu dieta.";
                            }
                            else
                            {
                                TempData["PostMessage"] = "Error inesperado.";
                            }
                        }
                        else if (datos.ComidaComida == "Merienda")
                        {
                            if (datos.ComidaCalorias > dietaPer.DietaMeriendaCalorias)
                            {
                                sePaso = true;
                            }
                            else
                            {
                                sePaso = false;
                            }


                            if (sePaso == true)
                            {
                                TempData["PostMessage"] = "Según tu dieta te pasaste de las calorías especificadas para esta comida, pero no te preocupes esta bien darse un gustito!";
                            }
                            else if (sePaso == false)
                            {
                                TempData["PostMessage"] = "Excelente, las calorías de esta comida concuerdan con tu dieta.";
                            }
                            else
                            {
                                TempData["PostMessage"] = "Error inesperado.";
                            }
                        }
                        else if (datos.ComidaComida == "Cena")
                        {
                            if (datos.ComidaCalorias > dietaPer.DietaCenaCalorias)
                            {
                                sePaso = true;
                            }
                            else
                            {
                                sePaso = false;
                            }


                            if (sePaso == true)
                            {
                                TempData["PostMessage"] = "Según tu dieta te pasaste de las calorías especificadas para esta comida, pero no te preocupes esta bien darse un gustito!";
                            }
                            else if (sePaso == false)
                            {
                                TempData["PostMessage"] = "Excelente, las calorías de esta comida concuerdan con tu dieta.";
                            }
                            else
                            {
                                TempData["PostMessage"] = "Error inesperado.";
                            }
                        }
                        else //Ingestas
                        {
                            List<ModelCL.Registro> regIngestasEx = db.Registro.Where(r => r.Comida != null && r.Comida.ComidaComida == datos.ComidaComida && r.Persona.PersonaId == idPer && r.RegistroFchHora/*.Date*/ == f/*.Date*/).ToList();

                            if (regIngestasEx.Count() > 0)
                            {
                                short caloriasIngestas = 0;

                                foreach (var i in regIngestasEx)
                                {
                                    caloriasIngestas += Convert.ToInt16(i.Comida.ComidaCalorias);
                                }

                                caloriasIngestas += Convert.ToInt16(datos.ComidaCalorias);


                                if (caloriasIngestas > dietaPer.DietaIngestasCalorias)
                                {
                                    TempData["PostMessage"] = "Según tu dieta te pasaste de las calorías especificadas para las ingestas, pero no te preocupes esta bien darse un gustito!";
                                }
                                else
                                {
                                    int caloriasFaltantes = dietaPer.DietaIngestasCalorias - caloriasIngestas;

                                    TempData["PostMessage"] = "Excelente, hasta ahora las calorías de tus ingestas concuerdan con tu dieta. Aún puedes consumir " + caloriasFaltantes + " calorías más este día.";
                                }
                            }
                            else
                            {
                                short caloriasIngestas = Convert.ToInt16(datos.ComidaCalorias);


                                if (caloriasIngestas > dietaPer.DietaIngestasCalorias)
                                {
                                    TempData["PostMessage"] = "Según tu dieta te pasaste de las calorías especificadas para las ingestas, pero no te preocupes esta bien darse un gustito!";
                                }
                                else
                                {
                                    int caloriasFaltantes = dietaPer.DietaIngestasCalorias - caloriasIngestas;

                                    TempData["PostMessage"] = "Excelente, hasta ahora las calorías de tus ingestas concuerdan con tu dieta. Aún puedes consumir " + caloriasFaltantes + " calorías más este día.";
                                }
                            }
                        }
                    }
                }


                Persona.Registro.Add(regControl);                            
                
                db.SaveChanges();                

                return RedirectToAction("Index");
            }

            return View(datos);
        }

        [Route("editar")]
        public ActionResult Edit(long? id)
        {
            long idPer = Convert.ToInt64(Request.Cookies["cookiePer"]["PerId"]);

            if (Fachada.Functions.es_diabetico_tipo_1(idPer) == false)
            {
                ModelCL.Persona Persona = db.Persona.Find(idPer);

                TempData["NoCumpleRequisitos"] = "La persona -" + Persona.nombreCompleto + "- no tiene la enfermedad -Diabetes tipo 1- y por lo tanto no puede acceder a este registro.";
                return RedirectToAction("Index", "Home");
            }


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ModelCL.Registro regControlDiabetico = db.Registro.Where(r => r.RegistroId == id).FirstOrDefault();
            if (!(regControlDiabetico.Control != null && (regControlDiabetico.Control.Valor.ValorNombre == "Glucosa")))
            {
                return HttpNotFound();
            }


            RegControlDiabeticoViewModel vmRegControlDiabetico = new RegControlDiabeticoViewModel();


            List<Fachada.ViewModelSelectListChk> lComidas = new List<Fachada.ViewModelSelectListChk>()
            {
                new Fachada.ViewModelSelectListChk { Id = "Desayuno", Valor = "Desayuno" },
                new Fachada.ViewModelSelectListChk { Id = "Almuerzo", Valor = "Almuerzo" },
                new Fachada.ViewModelSelectListChk { Id = "Merienda", Valor = "Merienda" },
                new Fachada.ViewModelSelectListChk { Id = "Cena", Valor = "Cena" },
                new Fachada.ViewModelSelectListChk { Id = "Ingesta", Valor = "Ingesta" },
            };
            ViewBag.lComidas = new SelectList(lComidas, "Id", "Valor");


            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);

            List<ModelCL.Alimento> lAlimentos = db.Alimento.Where(a => a.Usuario == null || a.Usuario.UsuarioId == idUsu).ToList();
            vmRegControlDiabetico.Alimentos = new List<ComidaAlimentoViewModel>();
            foreach (ModelCL.Alimento ali in lAlimentos)
            {
                vmRegControlDiabetico.Alimentos.Add(new ComidaAlimentoViewModel { AlimentoId = ali.AlimentoId, AlimentoNombre = ali.AlimentoNombre, AlimentoImagen = ali.AlimentoImagen, AlimentoPorcion = ali.AlimentoPorcion, AlimentoCalorias = ali.AlimentoCalorias, AlimentoCarbohidratos = ali.AlimentoCarbohidratos, AlimentoAzucar = ali.AlimentoAzucar, AlimentoGrasa = ali.AlimentoGrasa, AlimentoSodio = ali.AlimentoSodio, AlimentoGluten = ali.AlimentoGluten });
            }


            vmRegControlDiabetico.RegistroId = regControlDiabetico.RegistroId;
            //vmRegComida.PersonaId = regComida.PersonaId;
            vmRegControlDiabetico.RegistroFchHora = regControlDiabetico.RegistroFchHora.ToString();

            vmRegControlDiabetico.ControlValor = regControlDiabetico.Control.ControlValor;

            if (regControlDiabetico.Comida != null)
            {
                vmRegControlDiabetico.RegistrarComida = true;

                vmRegControlDiabetico.ComidaPlatillo = regControlDiabetico.Comida.ComidaPlatillo;
                vmRegControlDiabetico.ComidaComida = regControlDiabetico.Comida.ComidaComida;
                //vmRegComida.Alimentos = regComida.Comida.RelComAli.ToList();
                foreach (ModelCL.RelComAli rca in regControlDiabetico.Comida.RelComAli.ToList())
                {
                    ComidaAlimentoViewModel cavm = vmRegControlDiabetico.Alimentos.Where(a => a.AlimentoId == rca.AlimentoId).FirstOrDefault();
                    cavm.RelComAliCantidad = rca.RelComAliCantidad;
                }
                vmRegControlDiabetico.ComidaCalorias = regControlDiabetico.Comida.ComidaCalorias;
                vmRegControlDiabetico.ComidaCarbohidratos = regControlDiabetico.Comida.ComidaCarbohidratos;
                vmRegControlDiabetico.ComidaAzucar = regControlDiabetico.Comida.ComidaAzucar;
                vmRegControlDiabetico.ComidaGrasa = regControlDiabetico.Comida.ComidaGrasa;
                vmRegControlDiabetico.ComidaSodio = regControlDiabetico.Comida.ComidaSodio;
                vmRegControlDiabetico.ComidaGluten = regControlDiabetico.Comida.ComidaGluten;
            }

            //Ver como tratamos en especifico esta parte, es diferente al create ya que aca ya tenes los valores finales
            if (regControlDiabetico.Medicacion != null)
            {
                vmRegControlDiabetico.ResultadoTotalInsulinaCorreccion = regControlDiabetico.Medicacion.MedicacionCantidad;
            }
            else
            {
                //Hay que ver como hacemos lo de los resultados con mensaje
            }

            return View(vmRegControlDiabetico);
        }

        [HttpPost]
        [Route("editar")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RegControlDiabeticoViewModel datos)
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
                ModelCL.Registro regControlDiabetico = db.Registro.Where(r => r.RegistroId == datos.RegistroId).FirstOrDefault();

                DateTime f = Convert.ToDateTime(datos.RegistroFchHora);
                if (datos.ComidaComida != "Ingesta")
                {
                    if (regControlDiabetico.RegistroFchHora != f)
                    {
                        ModelCL.Registro regComidaEx = db.Registro.Where(r => r.Comida != null && r.Comida.ComidaComida == datos.ComidaComida && r.Persona.PersonaId == regControlDiabetico.PersonaId && r.RegistroFchHora == f).FirstOrDefault();

                        if (regComidaEx != null)
                        {
                            ViewBag.ErrorMessage = "Ya existe un/a " + datos.ComidaComida + " registrado/a en esta fecha. Puede modificarlo si lo desea.";
                            return View(datos);
                        }
                    }
                }

                regControlDiabetico.RegistroFchHora = f;

                regControlDiabetico.Control.ControlValor = datos.ControlValor;


                if (datos.RegistrarComida == true)
                {
                    if (datos.Alimentos.Where(a => a.RelComAliCantidad != 0).Count() == 0)
                    {
                        ViewBag.ErrorMessage = "Si va a registrar una comida debe seleccionar al menos un alimento.";

                        return View(datos);
                    }

                    regControlDiabetico.Comida.ComidaPlatillo = datos.ComidaPlatillo;
                    regControlDiabetico.Comida.ComidaComida = datos.ComidaComida;

                    List<ModelCL.RelComAli> bkRelComAli = regControlDiabetico.Comida.RelComAli.ToList();
                    foreach (ModelCL.RelComAli oldRelComAli in bkRelComAli)
                    {
                        regControlDiabetico.Comida.RelComAli.Remove(oldRelComAli);
                    }

                    foreach (ComidaAlimentoViewModel a in datos.Alimentos)
                    {
                        if (a.RelComAliCantidad != 0)
                        {
                            regControlDiabetico.Comida.RelComAli.Add(new ModelCL.RelComAli { Alimento = db.Alimento.Find(a.AlimentoId), RelComAliCantidad = a.RelComAliCantidad });
                        }
                    }

                    regControlDiabetico.Comida.ComidaCalorias = datos.ComidaCalorias;
                    regControlDiabetico.Comida.ComidaCarbohidratos = datos.ComidaCarbohidratos;
                    regControlDiabetico.Comida.ComidaAzucar = datos.ComidaAzucar;
                    regControlDiabetico.Comida.ComidaGrasa = datos.ComidaGrasa;
                    regControlDiabetico.Comida.ComidaSodio = datos.ComidaSodio;
                    regControlDiabetico.Comida.ComidaGluten = datos.ComidaGluten;
                }
                else
                {
                    regControlDiabetico.Comida = null;
                }


                if (datos.ResultadoTotalInsulinaCorreccion != null)
                {
                    regControlDiabetico.Medicacion.MedicacionCantidad = Convert.ToDouble(datos.ResultadoTotalInsulinaCorreccion);
                }
                else
                {
                    regControlDiabetico.Medicacion = null;
                }

                db.SaveChanges();


                if (datos.RegistrarComida == true)
                {
                    long idPer = Convert.ToInt64(Request.Cookies["cookiePer"]["PerId"]);
                    ModelCL.Persona Persona = db.Persona.Find(idPer);

                    ModelCL.Dieta dietaPer = Persona.Dieta;
                    if (dietaPer != null)
                    {
                        Nullable<bool> sePaso = null;

                        if (datos.ComidaComida == "Desayuno")
                        {
                            if (datos.ComidaCalorias > dietaPer.DietaDesayunoCalorias)
                            {
                                sePaso = true;
                            }
                            else
                            {
                                sePaso = false;
                            }


                            if (sePaso == true)
                            {
                                TempData["PostMessage"] = "Según tu dieta te pasaste de las calorías especificadas para esta comida, pero no te preocupes esta bien darse un gustito!";
                            }
                            else if (sePaso == false)
                            {
                                TempData["PostMessage"] = "Excelente, las calorías de esta comida concuerdan con tu dieta.";
                            }
                            else
                            {
                                TempData["PostMessage"] = "Error inesperado.";
                            }
                        }
                        else if (datos.ComidaComida == "Almuerzo")
                        {
                            if (datos.ComidaCalorias > dietaPer.DietaAlmuerzoCalorias)
                            {
                                sePaso = true;
                            }
                            else
                            {
                                sePaso = false;
                            }


                            if (sePaso == true)
                            {
                                TempData["PostMessage"] = "Según tu dieta te pasaste de las calorías especificadas para esta comida, pero no te preocupes esta bien darse un gustito!";
                            }
                            else if (sePaso == false)
                            {
                                TempData["PostMessage"] = "Excelente, las calorías de esta comida concuerdan con tu dieta.";
                            }
                            else
                            {
                                TempData["PostMessage"] = "Error inesperado.";
                            }
                        }
                        else if (datos.ComidaComida == "Merienda")
                        {
                            if (datos.ComidaCalorias > dietaPer.DietaMeriendaCalorias)
                            {
                                sePaso = true;
                            }
                            else
                            {
                                sePaso = false;
                            }


                            if (sePaso == true)
                            {
                                TempData["PostMessage"] = "Según tu dieta te pasaste de las calorías especificadas para esta comida, pero no te preocupes esta bien darse un gustito!";
                            }
                            else if (sePaso == false)
                            {
                                TempData["PostMessage"] = "Excelente, las calorías de esta comida concuerdan con tu dieta.";
                            }
                            else
                            {
                                TempData["PostMessage"] = "Error inesperado.";
                            }
                        }
                        else if (datos.ComidaComida == "Cena")
                        {
                            if (datos.ComidaCalorias > dietaPer.DietaCenaCalorias)
                            {
                                sePaso = true;
                            }
                            else
                            {
                                sePaso = false;
                            }


                            if (sePaso == true)
                            {
                                TempData["PostMessage"] = "Según tu dieta te pasaste de las calorías especificadas para esta comida, pero no te preocupes esta bien darse un gustito!";
                            }
                            else if (sePaso == false)
                            {
                                TempData["PostMessage"] = "Excelente, las calorías de esta comida concuerdan con tu dieta.";
                            }
                            else
                            {
                                TempData["PostMessage"] = "Error inesperado.";
                            }
                        }
                        else //Ingestas
                        {
                            List<ModelCL.Registro> regIngestasEx = db.Registro.Where(r => r.RegistroId != datos.RegistroId && r.Comida != null && r.Comida.ComidaComida == datos.ComidaComida && r.Persona.PersonaId == idPer && r.RegistroFchHora/*.Date*/ == f/*.Date*/).ToList();

                            if (regIngestasEx.Count() > 0)
                            {
                                short caloriasIngestas = 0;

                                foreach (var i in regIngestasEx)
                                {
                                    caloriasIngestas += Convert.ToInt16(i.Comida.ComidaCalorias);
                                }

                                caloriasIngestas += Convert.ToInt16(datos.ComidaCalorias);


                                if (caloriasIngestas > dietaPer.DietaIngestasCalorias)
                                {
                                    TempData["PostMessage"] = "Según tu dieta te pasaste de las calorías especificadas para las ingestas, pero no te preocupes esta bien darse un gustito!";
                                }
                                else
                                {
                                    int caloriasFaltantes = dietaPer.DietaIngestasCalorias - caloriasIngestas;

                                    TempData["PostMessage"] = "Excelente, hasta ahora las calorías de tus ingestas concuerdan con tu dieta. Aún puedes consumir " + caloriasFaltantes + " calorías más este día.";
                                }
                            }
                            else
                            {
                                short caloriasIngestas = Convert.ToInt16(datos.ComidaCalorias);


                                if (caloriasIngestas > dietaPer.DietaIngestasCalorias)
                                {
                                    TempData["PostMessage"] = "Según tu dieta te pasaste de las calorías especificadas para las ingestas, pero no te preocupes esta bien darse un gustito!";
                                }
                                else
                                {
                                    int caloriasFaltantes = dietaPer.DietaIngestasCalorias - caloriasIngestas;

                                    TempData["PostMessage"] = "Excelente, hasta ahora las calorías de tus ingestas concuerdan con tu dieta. Aún puedes consumir " + caloriasFaltantes + " calorías más este día.";
                                }
                            }
                        }
                    }
                }                

                return RedirectToAction("Index");
            }

            return View(datos);
        }

        [Route("calcularResultado")]
        public ActionResult calcularResultado(double valorControl, short? hidratos)
        {
            //Nullable<short> hidratos = datos.ComidaCarbohidratos;
            //double valorControl = datos.ControlValor;

            string resultadoInsulinaMensaje = null;
            double insulinaPorCorreccion = 0;
            double insulinaPorHidratos = 0;
            double totalInsulina = 0;

            long idPer = Convert.ToInt64(Request.Cookies["cookiePer"]["PerId"]);
            ModelCL.Persona Persona = db.Persona.Find(idPer);

            ModelCL.Medicina insuCorreccion = Persona.RelPerEnf.Where(rpe => rpe.Enfermedad.EnfermedadNombre == "Diabetes tipo 1").FirstOrDefault().RelMedRelPerEnf.Where(rmrpe => rmrpe.Medicina.MedicinaTipo == "Activa").FirstOrDefault().Medicina;

            ModelCL.Dosis dosisInsu = insuCorreccion.RelMedVal.Where(rmv => rmv.Valor.ValorNombre == "Glucosa").FirstOrDefault().Dosis.FirstOrDefault();


            ModelCL.Valor valorGlucosa = db.Valor.Where(v => v.ValorNombre == "Glucosa").FirstOrDefault();

            //double puntoMedioNormalGlucosa = valorGlucosa.ValorNormalMinimo + ((valorGlucosa.ValorNormalMaximo - valorGlucosa.ValorNormalMinimo) / 2);



            if (valorControl > valorGlucosa.ValorNormalMaximo)
            {
                double difGlucosa = valorControl - valorGlucosa.ValorNormalMaximo;

                double cantidadDeMinimas = difGlucosa / Math.Abs(dosisInsu.DosisEfecto);

                insulinaPorCorreccion = dosisInsu.DosisCantidadMin * Math.Ceiling(cantidadDeMinimas);
            }


            if (hidratos != null && hidratos != 0)
            {
                short hidratosPorUniInsu = Persona.Datos.Where(d => d.Diabetes != null).OrderByDescending(d => d.DatosFchEnable).FirstOrDefault().Diabetes.DiabetesHidratosPorUniInsu;

                double UnidadesFijasPorHidratos = Convert.ToDouble(hidratos / hidratosPorUniInsu);

                double cantidadDeMinimas = UnidadesFijasPorHidratos / dosisInsu.DosisCantidadMin;

                insulinaPorHidratos = dosisInsu.DosisCantidadMin * Math.Round(cantidadDeMinimas);
            }



            if (insulinaPorCorreccion == 0)
            {
                if (insulinaPorHidratos == 0)
                {
                    if (valorControl >= valorGlucosa.ValorNormalMinimo && valorControl <= valorGlucosa.ValorNormalMaximo)
                    {
                        resultadoInsulinaMensaje = "No es necesario que te inyectes.";
                    }
                    else if (valorControl < valorGlucosa.ValorNormalMinimo && valorControl >= valorGlucosa.ValorBajoMinimo)
                    {
                        resultadoInsulinaMensaje = "Será necesario que consumas alimentos con carbohidratos o un poco de azucar para volver a tus valores normales.";
                    }
                    else
                    {
                        resultadoInsulinaMensaje = "Debes ingerir azucar lo antes posible!";
                    }
                }
                else
                {
                    double glucosaGeneradaPorHidratos = insulinaPorHidratos * Math.Abs(dosisInsu.DosisEfecto);

                    double sumaGlucosaConHidratos = valorControl + glucosaGeneradaPorHidratos;


                    if (sumaGlucosaConHidratos > valorGlucosa.ValorNormalMaximo)
                    {
                        double difGlucosa = sumaGlucosaConHidratos - valorGlucosa.ValorNormalMaximo;

                        double cantidadDeMinimas = difGlucosa / Math.Abs(dosisInsu.DosisEfecto);

                        totalInsulina = dosisInsu.DosisCantidadMin * Math.Ceiling(cantidadDeMinimas);

                        resultadoInsulinaMensaje = "Según los valores proporcionados deberías inyectarte.";
                    }
                    else if (sumaGlucosaConHidratos >= valorGlucosa.ValorNormalMinimo && sumaGlucosaConHidratos <= valorGlucosa.ValorNormalMaximo)
                    {
                        resultadoInsulinaMensaje = "No es necesario que te inyectes.";
                    }
                    else if (sumaGlucosaConHidratos >= valorGlucosa.ValorBajoMinimo)
                    {
                        resultadoInsulinaMensaje = "Será necesario que consumas más alimentos con carbohidratos o un poco de azucar para volver a tus valores normales.";
                    }
                    else
                    {
                        resultadoInsulinaMensaje = "Debes ingerir azucar lo antes posible!";
                    }
                }
            }
            else
            {
                resultadoInsulinaMensaje = "Según los valores proporcionados deberías inyectarte.";
                totalInsulina = insulinaPorCorreccion + insulinaPorHidratos;
            }
            

            //return null;
            return Json(new { resultadoInsulinaMensaje = resultadoInsulinaMensaje, totalInsulina = totalInsulina }, JsonRequestBehavior.AllowGet);
            //return this.Content(notificaciones.Count().ToString());
        }

        [Route("eliminar")]
        //[ValidateAntiForgeryToken]
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ModelCL.Registro registro = db.Registro.Where(r => r.RegistroId == id).FirstOrDefault();
            if (!(registro.Control != null && (registro.Control.Valor.ValorNombre == "Glucosa")))
            {
                return HttpNotFound();
            }

            db.Registro.Remove(registro);
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
