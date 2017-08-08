﻿using Lucy.Models.Personas;
using ModelCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Lucy.Controllers
{
    public class PersonasController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        [Authorize]
        [HttpGet]
        public ActionResult Datos_Clinicos()
        {
            HttpCookie cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket usu = FormsAuthentication.Decrypt(cookie.Value);
            int idUsu = Convert.ToInt32(usu.Name);

            ModelCL.Usuario Usuario = db.Usuario.Find(idUsu);
            List<ModelCL.Persona> lPersonas = Usuario.Persona.ToList();
            ModelCL.Persona Persona = new Persona();
            Persona.PersonaId = 0;
            Persona.PersonaApellido = ">> CREAR NUEVA PERSONA <<";
            lPersonas.Add(Persona);
            ViewBag.listaPersonas = new SelectList(lPersonas, "PersonaId", "nombreCompleto");
            return View();
        }

        [HttpGet]
        public PartialViewResult _DatCli(int id)
        {
            try
            {
                DatCliViewModel newDatCli = new DatCliViewModel();

                List<ModelCL.Enfermedad> lEnfermedades = db.Enfermedad.Where(enf => enf.Usuario == null).ToList();//Hay que filtrar por oficiales
                List<ViewModelCheckBox> lEnf = new List<ViewModelCheckBox>();
                foreach (ModelCL.Enfermedad enf in lEnfermedades)
                {
                    lEnf.Add(new ViewModelCheckBox() { Id = enf.EnfermedadId, Nombre = enf.EnfermedadNombre });
                }

                newDatCli.Enfermedades = lEnf;


                if (id != 0)
                {
                    ModelCL.Persona Persona = db.Persona.Find(id);

                    newDatCli.PersonaNombre = Persona.PersonaNombre;
                    newDatCli.PersonaApellido = Persona.PersonaApellido;
                    newDatCli.PersonaFchNac = Persona.PersonaFchNac.ToString();
                    newDatCli.SexoId = Persona.SexoId;

                    //ModelCL.DatCli DatCli = db.Persona.Where(dc => dc.PersonaId );
                    ModelCL.DatCli DatCli = Persona.DatCli.FirstOrDefault();

                    if (DatCli != null)
                    {
                        newDatCli.DatCliPeso = DatCli.DatCliPeso;
                        newDatCli.DatCliAltura = DatCli.DatCliAltura;
                    }

                    for (int i = 0; i < newDatCli.Enfermedades.Count; i++)
                    {
                        ModelCL.RelPerEnf rel = Persona.RelPerEnf.Where(rpe => rpe.EnfermedadId == newDatCli.Enfermedades[i].Id).FirstOrDefault();

                        if (rel != null)
                        {
                            newDatCli.Enfermedades[i].Checked = true;
                        }
                    }

                    ModelCL.Valor Valor = Persona.Valor.Where(v => v.Diabetes != null).FirstOrDefault();

                    if (Valor != null)
                    {
                        newDatCli.DiabetesTipo = Valor.Diabetes.DiabetesTipo;
                        newDatCli.DiabetesGlicemiaBaja = Convert.ToDouble(Valor.Diabetes.DiabetesGlicemiaBaja);
                        newDatCli.DiabetesGlicemiaAlta = Convert.ToDouble(Valor.Diabetes.DiabetesGlicemiaAlta);
                        newDatCli.DiabetesHidratosPorUniInsu = Convert.ToInt16(Valor.Diabetes.DiabetesHidratosPorUniInsu);

                        if (Valor.Diabetes.DiabetesTipo == "1")
                        {
                            //Aca se podría agregar un filtro segun si la medicina es oficial o no a menos que queramos tener en cuenta medicinas registradas por el usuario//
                            ModelCL.Medicina InsulinaRetardada = Persona.RelPerEnf.Where(rpe => rpe.Enfermedad.EnfermedadNombre == "Diabetes").FirstOrDefault().RelMedRelPerEnf.Where(rmrpe => rmrpe.Medicina.MedicinaTipo == "retardada").FirstOrDefault().Medicina;
                            newDatCli.InsulinaRetardadaId = InsulinaRetardada.MedicinaId;

                            ModelCL.Medicina InsulinaCorreccion = Persona.RelPerEnf.Where(rpe => rpe.Enfermedad.EnfermedadNombre == "Diabetes").FirstOrDefault().RelMedRelPerEnf.Where(rmrpe => rmrpe.Medicina.MedicinaTipo == "correccion").FirstOrDefault().Medicina;
                            newDatCli.InsulinaCorreccionId = InsulinaCorreccion.MedicinaId;

                        }
                    }
                }

                ViewBag.idPersona = id;

                List<ModelCL.Sexo> lSexos = db.Sexo.ToList();
                ViewBag.listaSexos = new SelectList(lSexos, "SexoId", "SexoNombre");

                List<ViewModelSelectList> lTiposDiabetes = new List<ViewModelSelectList>()
                {
                    new ViewModelSelectList { Id = 1, Valor = "1" },
                    new ViewModelSelectList { Id = 2, Valor = "2" },
                };
                ViewBag.listaTiposDiabetes = new SelectList(lTiposDiabetes, "Id", "Valor");

                //Lo mismo que antes - Aca se podría agregar un filtro segun si la medicina es oficial o no a menos que queramos tener en cuenta medicinas registradas por el usuario//
                List<ModelCL.Medicina> lInsulinasRetardadas = db.Medicina.Where(m => m.MedicinaTipo == "retardada" && m.Enfermedad.Where(e => e.EnfermedadNombre == "Diabetes").FirstOrDefault() != null).ToList();
                ViewBag.listaInsulinasRetardadas = new SelectList(lInsulinasRetardadas, "MedicinaId", "MedicinaNombre");

                List<ModelCL.Medicina> lInsulinasCorreccion = db.Medicina.Where(m => m.MedicinaTipo == "correccion" && m.Enfermedad.Where(e => e.EnfermedadNombre == "Diabetes").FirstOrDefault() != null).ToList();
                ViewBag.listaInsulinasCorreccion = new SelectList(lInsulinasCorreccion, "MedicinaId", "MedicinaNombre");

                return PartialView("_DatCli", newDatCli);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult _DatCli(DatCliViewModel Datos, int id)
        {
            try
            {
                ModelCL.Persona Persona = new ModelCL.Persona();
                if (id != 0)
                {
                    Persona = db.Persona.Find(id);
                }

                Persona.PersonaNombre = Datos.PersonaNombre;
                Persona.PersonaApellido = Datos.PersonaApellido;
                Persona.PersonaFchNac = Convert.ToDateTime(Datos.PersonaFchNac);
                Persona.Sexo = db.Sexo.Find(Datos.SexoId);


                ModelCL.DatCli oldDatCli = Persona.DatCli.FirstOrDefault();

                ModelCL.DatCli DatCli = new ModelCL.DatCli();

                DatCli.Persona = Persona;
                DatCli.DatCliPeso = Datos.DatCliPeso;
                DatCli.DatCliAltura = Datos.DatCliAltura;

                if (oldDatCli != null)
                {
                    if (oldDatCli.DatCliPeso != DatCli.DatCliPeso || oldDatCli.DatCliAltura != DatCli.DatCliAltura)
                    {
                        Persona.DatCli.Remove(oldDatCli);
                        Persona.DatCli.Add(DatCli);
                    }
                }
                else
                {
                    Persona.DatCli.Remove(oldDatCli);
                    Persona.DatCli.Add(DatCli);
                }

                foreach (ViewModelCheckBox enf in Datos.Enfermedades)
                {
                    enf.Nombre = db.Enfermedad.Find(enf.Id).EnfermedadNombre;

                    ModelCL.RelPerEnf rel = Persona.RelPerEnf.Where(rpe => rpe.EnfermedadId == enf.Id).FirstOrDefault();

                    if (rel == null)
                    {
                        if (enf.Checked == true)
                        {
                            ModelCL.RelPerEnf newrel = new ModelCL.RelPerEnf();
                            newrel.Persona = Persona;
                            newrel.Enfermedad = db.Enfermedad.Find(enf.Id);

                            Persona.RelPerEnf.Add(newrel);
                        }
                    }
                    else
                    {
                        if (enf.Checked == false)
                        {
                            ModelCL.RelPerEnf oldRelPerEnf = Persona.RelPerEnf.Where(rpe => rpe.EnfermedadId == enf.Id).FirstOrDefault();

                            foreach (ModelCL.RelMedRelPerEnf oldrmrpe in oldRelPerEnf.RelMedRelPerEnf)
                            {
                                Persona.RelPerEnf.Where(rpe => rpe.EnfermedadId == enf.Id).FirstOrDefault().RelMedRelPerEnf.Remove(oldrmrpe);
                            }

                            Persona.RelPerEnf.Remove(oldRelPerEnf);
                        }
                    }
                }


                if (Datos.Enfermedades.Where(e => e.Nombre == "Diabetes").FirstOrDefault().Checked == true)
                {
                    ModelCL.Valor oldValor = Persona.Valor.Where(v => v.Diabetes != null).FirstOrDefault();
                    if (oldValor != null && oldValor.Diabetes != null)
                    {
                        if (oldValor.Diabetes.DiabetesTipo != Datos.DiabetesTipo ||
                        oldValor.Diabetes.DiabetesGlicemiaAlta != Datos.DiabetesGlicemiaAlta ||
                        oldValor.Diabetes.DiabetesGlicemiaBaja != Datos.DiabetesGlicemiaBaja ||
                        oldValor.Diabetes.DiabetesHidratosPorUniInsu != Datos.DiabetesHidratosPorUniInsu)
                        {
                            Persona.Valor.Remove(oldValor);

                            ModelCL.Valor newValor = new ModelCL.Valor();
                            newValor.Persona = Persona;
                            //Persona.Valor.Add(newValor);

                            ModelCL.Diabetes newDiabetes = new ModelCL.Diabetes();
                            newDiabetes.Valor = newValor;
                            newDiabetes.DiabetesTipo = Datos.DiabetesTipo;

                            if (Datos.DiabetesTipo == "1")
                            {
                                newDiabetes.DiabetesGlicemiaAlta = Datos.DiabetesGlicemiaAlta;
                                newDiabetes.DiabetesGlicemiaBaja = Datos.DiabetesGlicemiaBaja;
                                newDiabetes.DiabetesHidratosPorUniInsu = Datos.DiabetesHidratosPorUniInsu;
                            }

                            //db.Diabetes.Add(newDiabetes);
                            newValor.Diabetes = newDiabetes;
                            Persona.Valor.Add(newValor);
                        }

                        if (Datos.DiabetesTipo == "1")
                        {
                            ModelCL.RelMedRelPerEnf oldInsulinaRetardada = Persona.RelPerEnf.Where(rpe => rpe.Enfermedad.EnfermedadNombre == "Diabetes").FirstOrDefault().RelMedRelPerEnf.Where(rmrpe => rmrpe.Medicina.MedicinaTipo == "retardada").FirstOrDefault();
                            if (oldInsulinaRetardada != null)
                            {
                                if (oldInsulinaRetardada.MedicinaId != Datos.InsulinaRetardadaId)
                                {
                                    //Probando si se puede cambiar a traves de la variable
                                    Persona.RelPerEnf.Where(rpe => rpe.Enfermedad.EnfermedadNombre == "Diabetes").FirstOrDefault().RelMedRelPerEnf.Remove(oldInsulinaRetardada);

                                    ModelCL.RelMedRelPerEnf newInsulinaRetardada = new ModelCL.RelMedRelPerEnf();

                                    //Hay que ver si no falla si se acaba de agregar la relperenf a la persona
                                    newInsulinaRetardada.RelPerEnf = Persona.RelPerEnf.Where(rpe => rpe.Enfermedad.EnfermedadNombre == "Diabetes").FirstOrDefault();
                                    newInsulinaRetardada.Medicina = db.Medicina.Find(Datos.InsulinaRetardadaId);

                                    //Frecuencia harcodeada, hay que agregar luego en la view//
                                    newInsulinaRetardada.RelMedRelPerEnfFrecuencia = 1;
                                    newInsulinaRetardada.RelMedRelPerEnfFrecuenciaTipo = "día";

                                    Persona.RelPerEnf.Where(rpe => rpe.Enfermedad.EnfermedadNombre == "Diabetes").FirstOrDefault().RelMedRelPerEnf.Add(newInsulinaRetardada);
                                }
                            }
                            else
                            {
                                ModelCL.RelMedRelPerEnf newInsulinaRetardada = new ModelCL.RelMedRelPerEnf();

                                //Hay que ver si no falla si se acaba de agregar la relperenf a la persona
                                newInsulinaRetardada.RelPerEnf = Persona.RelPerEnf.Where(rpe => rpe.Enfermedad.EnfermedadNombre == "Diabetes").FirstOrDefault();
                                newInsulinaRetardada.Medicina = db.Medicina.Find(Datos.InsulinaRetardadaId);

                                //Frecuencia harcodeada, hay que agregar luego en la view//
                                newInsulinaRetardada.RelMedRelPerEnfFrecuencia = 1;
                                newInsulinaRetardada.RelMedRelPerEnfFrecuenciaTipo = "día";

                                Persona.RelPerEnf.Where(rpe => rpe.Enfermedad.EnfermedadNombre == "Diabetes").FirstOrDefault().RelMedRelPerEnf.Add(newInsulinaRetardada);
                            }


                            ModelCL.RelMedRelPerEnf oldInsulinaCorreccion = Persona.RelPerEnf.Where(rpe => rpe.Enfermedad.EnfermedadNombre == "Diabetes").FirstOrDefault().RelMedRelPerEnf.Where(rmrpe => rmrpe.Medicina.MedicinaTipo == "correccion").FirstOrDefault();
                            if (oldInsulinaCorreccion != null)
                            {
                                if (oldInsulinaCorreccion.MedicinaId != Datos.InsulinaCorreccionId)
                                {
                                    //Probando si se puede cambiar a traves de la variable
                                    Persona.RelPerEnf.Where(rpe => rpe.Enfermedad.EnfermedadNombre == "Diabetes").FirstOrDefault().RelMedRelPerEnf.Remove(oldInsulinaCorreccion);

                                    ModelCL.RelMedRelPerEnf newInsulinaCorreccion = new ModelCL.RelMedRelPerEnf();

                                    //Hay que ver si no falla si se acaba de agregar la relperenf a la persona
                                    newInsulinaCorreccion.RelPerEnf = Persona.RelPerEnf.Where(rpe => rpe.Enfermedad.EnfermedadNombre == "Diabetes").FirstOrDefault();
                                    newInsulinaCorreccion.Medicina = db.Medicina.Find(Datos.InsulinaCorreccionId);

                                    //Frecuencia harcodeada, hay que agregar luego en la view//
                                    newInsulinaCorreccion.RelMedRelPerEnfFrecuencia = 1;
                                    newInsulinaCorreccion.RelMedRelPerEnfFrecuenciaTipo = "día";

                                    Persona.RelPerEnf.Where(rpe => rpe.Enfermedad.EnfermedadNombre == "Diabetes").FirstOrDefault().RelMedRelPerEnf.Add(newInsulinaCorreccion);
                                }
                            }
                            else
                            {
                                ModelCL.RelMedRelPerEnf newInsulinaCorreccion = new ModelCL.RelMedRelPerEnf();

                                //Hay que ver si no falla si se acaba de agregar la relperenf a la persona
                                newInsulinaCorreccion.RelPerEnf = Persona.RelPerEnf.Where(rpe => rpe.Enfermedad.EnfermedadNombre == "Diabetes").FirstOrDefault();
                                newInsulinaCorreccion.Medicina = db.Medicina.Find(Datos.InsulinaCorreccionId);

                                //Frecuencia harcodeada, hay que agregar luego en la view//
                                newInsulinaCorreccion.RelMedRelPerEnfFrecuencia = 1;
                                newInsulinaCorreccion.RelMedRelPerEnfFrecuenciaTipo = "día";

                                Persona.RelPerEnf.Where(rpe => rpe.Enfermedad.EnfermedadNombre == "Diabetes").FirstOrDefault().RelMedRelPerEnf.Add(newInsulinaCorreccion);
                            }
                        }
                    }
                    else
                    {
                        ModelCL.Valor newValor = new ModelCL.Valor();
                        newValor.Persona = Persona;

                        ModelCL.Diabetes newDiabetes = new ModelCL.Diabetes();
                        newDiabetes.Valor = newValor;
                        newDiabetes.DiabetesTipo = Datos.DiabetesTipo;

                        if (Datos.DiabetesTipo == "1")
                        {
                            newDiabetes.DiabetesGlicemiaAlta = Datos.DiabetesGlicemiaAlta;
                            newDiabetes.DiabetesGlicemiaBaja = Datos.DiabetesGlicemiaBaja;
                            newDiabetes.DiabetesHidratosPorUniInsu = Datos.DiabetesHidratosPorUniInsu;
                        }

                        //db.Diabetes.Add(newDiabetes);
                        newValor.Diabetes = newDiabetes;
                        Persona.Valor.Add(newValor);


                        if (Datos.DiabetesTipo == "1")
                        {
                            ModelCL.RelMedRelPerEnf newInsulinaRetardada = new ModelCL.RelMedRelPerEnf();

                            //Hay que ver si no falla si se acaba de agregar la relperenf a la persona
                            newInsulinaRetardada.RelPerEnf = Persona.RelPerEnf.Where(rpe => rpe.Enfermedad.EnfermedadNombre == "Diabetes").FirstOrDefault();
                            newInsulinaRetardada.Medicina = db.Medicina.Find(Datos.InsulinaRetardadaId);

                            //Frecuencia harcodeada, hay que agregar luego en la view//
                            newInsulinaRetardada.RelMedRelPerEnfFrecuencia = 1;
                            newInsulinaRetardada.RelMedRelPerEnfFrecuenciaTipo = "día";

                            Persona.RelPerEnf.Where(rpe => rpe.Enfermedad.EnfermedadNombre == "Diabetes").FirstOrDefault().RelMedRelPerEnf.Add(newInsulinaRetardada);



                            ModelCL.RelMedRelPerEnf newInsulinaCorreccion = new ModelCL.RelMedRelPerEnf();

                            //Hay que ver si no falla si se acaba de agregar la relperenf a la persona
                            newInsulinaCorreccion.RelPerEnf = Persona.RelPerEnf.Where(rpe => rpe.Enfermedad.EnfermedadNombre == "Diabetes").FirstOrDefault();
                            newInsulinaCorreccion.Medicina = db.Medicina.Find(Datos.InsulinaCorreccionId);

                            //Frecuencia harcodeada, hay que agregar luego en la view//
                            newInsulinaCorreccion.RelMedRelPerEnfFrecuencia = 1;
                            newInsulinaCorreccion.RelMedRelPerEnfFrecuenciaTipo = "día";

                            Persona.RelPerEnf.Where(rpe => rpe.Enfermedad.EnfermedadNombre == "Diabetes").FirstOrDefault().RelMedRelPerEnf.Add(newInsulinaCorreccion);
                        }
                    }
                }
                else
                {
                    ModelCL.Valor oldValor = Persona.Valor.Where(v => v.Diabetes != null).FirstOrDefault();

                    if (oldValor != null)
                    {
                        //Ver si se puede borrar en cascada - supongo que si
                        Persona.Valor.Remove(oldValor);
                    }
                }


                //Hay que evaluar aca, si lo vemos desde el lado de bd primero deberiamos crear la persona y
                //despues relacionar las tablas, pero actualmente estamos manejando objetos y es medio raro
                //puede ser que mas adelante cuando agreguemos mas tablas se cree un "relusuper" y ahi nos cambie
                //la forma de guardado    

                if (id == 0)
                {
                    HttpCookie cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                    FormsAuthenticationTicket usu = FormsAuthentication.Decrypt(cookie.Value);
                    int idUsu = Convert.ToInt32(usu.Name);

                    ModelCL.Usuario Usuario = db.Usuario.Find(idUsu);

                    Usuario.Persona.Add(Persona);
                    //Persona.Usuario.Add(Usuario);

                    //db.Persona.Add(Persona);
                }

                db.SaveChanges();

                return RedirectToAction("Datos_Clinicos");
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
    }
}