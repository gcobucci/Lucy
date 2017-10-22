using Lucy.Models;
using ModelCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Lucy.Controllers
{
    [Authorize]
    [RoutePrefix("personas")]
    public class PersonasController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        [HttpGet]
        [Route("datos_clinicos")]
        public ActionResult Datos_Clinicos()
        {
            HttpCookie cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket usu = FormsAuthentication.Decrypt(cookie.Value);
            long idUsu = Convert.ToInt32(usu.Name);

            ModelCL.Usuario Usuario = db.Usuario.Find(idUsu);
            List<ModelCL.RelUsuPer> lRelUsuPer = Usuario.RelUsuPer.ToList();
            List<ModelCL.Persona> lPersonas = new List<ModelCL.Persona>();
            foreach (ModelCL.RelUsuPer rup in lRelUsuPer)
            {
                lPersonas.Add(rup.Persona);
            }

            if (Fachada.Functions.es_premium(idUsu) == true)
            {
                if (lPersonas.Count() < 5)
                {
                    ModelCL.Persona Persona = new ModelCL.Persona();
                    Persona.PersonaId = 0;
                    Persona.PersonaNombre = ">> CREAR NUEVA PERSONA <<";
                    lPersonas.Add(Persona);
                }
            }
            else
            {
                if (lPersonas.Count() < 2)
                {
                    ModelCL.Persona Persona = new ModelCL.Persona();
                    Persona.PersonaId = 0;
                    Persona.PersonaNombre = ">> CREAR NUEVA PERSONA <<";
                    lPersonas.Add(Persona);
                }
            }
            
            
            ViewBag.listaPersonas = new SelectList(lPersonas, "PersonaId", "nombreCompleto");
            return View();
        }

        [HttpGet]
        [Route("_datcli")]
        public PartialViewResult _DatCli(long id)
        {
            try
            {
                DatCliViewModel newDatCli = new DatCliViewModel();

                List<ModelCL.Enfermedad> lEnfermedades = db.Enfermedad.Where(enf => enf.Usuario == null).ToList();
                List<Fachada.ViewModelCheckBox> lEnf = new List<Fachada.ViewModelCheckBox>();
                foreach (ModelCL.Enfermedad enf in lEnfermedades)
                {
                    lEnf.Add(new Fachada.ViewModelCheckBox() { Id = enf.EnfermedadId, Nombre = enf.EnfermedadNombre });
                }

                newDatCli.Enfermedades = lEnf;


                if (id != 0)
                {
                    ModelCL.Persona Persona = db.Persona.Find(id);

                    newDatCli.PersonaNombre = Persona.PersonaNombre;
                    newDatCli.PersonaApellido = Persona.PersonaApellido;
                    newDatCli.PersonaFchNac = Persona.PersonaFchNac.ToString();
                    newDatCli.SexoId = Persona.SexoId;


                    ModelCL.Registro RegistroDatCliActividad = Persona.Registro.Where(reg => reg.DatCli != null && reg.DatCli.DatCliNivelActividad != null).OrderByDescending(reg => reg.RegistroFchHora).FirstOrDefault();

                    if (RegistroDatCliActividad != null)
                    {
                        newDatCli.DatCliNivelActividad = RegistroDatCliActividad.DatCli.DatCliNivelActividad;
                    }

                    ModelCL.Registro RegistroPeso = Persona.Registro.Where(reg => reg.Peso != null).OrderByDescending(reg => reg.RegistroFchHora).FirstOrDefault();                     

                    if (RegistroPeso != null)
                    {
                        newDatCli.PesoValor = RegistroPeso.Peso.PesoValor;
                    }                    

                    ModelCL.Registro RegistroDatCliAltura = Persona.Registro.Where(reg => reg.DatCli != null && reg.DatCli.DatCliAltura != null).OrderByDescending(reg => reg.RegistroFchHora).FirstOrDefault();              

                    if (RegistroDatCliAltura != null)
                    {
                        newDatCli.DatCliAltura = RegistroDatCliAltura.DatCli.DatCliAltura;
                    }
                    
                    

                    for (int i = 0; i < newDatCli.Enfermedades.Count; i++)
                    {
                        ModelCL.RelPerEnf rel = Persona.RelPerEnf.Where(rpe => rpe.EnfermedadId == newDatCli.Enfermedades[i].Id).FirstOrDefault();

                        if (rel != null)
                        {
                            newDatCli.Enfermedades[i].Checked = true;
                        }
                    }


                    if (Persona.RelPerEnf.Where(rpe => rpe.Enfermedad.EnfermedadNombre == "Diabetes tipo 1").FirstOrDefault() != null)
                    {
                        ModelCL.Datos Datos = Persona.Datos.Where(v => v.Diabetes != null).FirstOrDefault();

                        //if (Datos.Diabetes != null)
                        //{
                        newDatCli.DiabetesHidratosPorUniInsu = Datos.Diabetes.DiabetesHidratosPorUniInsu;
                        //}

                        //Aca se podría agregar un filtro segun si la medicina es oficial o no a menos que queramos tener en cuenta medicinas registradas por el usuario//
                        ModelCL.Medicina InsulinaRetardada = Persona.RelPerEnf.Where(rpe => rpe.Enfermedad.EnfermedadNombre == "Diabetes tipo 1").FirstOrDefault().RelMedRelPerEnf.Where(rmrpe => rmrpe.Medicina.MedicinaTipo == "Pasiva").FirstOrDefault().Medicina;
                        newDatCli.InsulinaRetardadaId = InsulinaRetardada.MedicinaId;

                        ModelCL.Medicina InsulinaCorreccion = Persona.RelPerEnf.Where(rpe => rpe.Enfermedad.EnfermedadNombre == "Diabetes tipo 1").FirstOrDefault().RelMedRelPerEnf.Where(rmrpe => rmrpe.Medicina.MedicinaTipo == "Activa").FirstOrDefault().Medicina;
                        newDatCli.InsulinaCorreccionId = InsulinaCorreccion.MedicinaId;
                    }
                }

                ViewBag.idPersona = id;

                List<ModelCL.Sexo> lSexos = db.Sexo.ToList();
                ViewBag.listaSexos = new SelectList(lSexos, "SexoId", "SexoNombre");

                List<Fachada.ViewModelSelectListChk> lNivelesActividad = new List<Fachada.ViewModelSelectListChk>()
                {
                    new Fachada.ViewModelSelectListChk { Id = "Sedentario", Valor = "Sedentario" },
                    new Fachada.ViewModelSelectListChk { Id = "Escasa", Valor = "Escasa" },
                    new Fachada.ViewModelSelectListChk { Id = "Moderada", Valor = "Moderada" },
                    new Fachada.ViewModelSelectListChk { Id = "Alta", Valor = "Alta" },
                    new Fachada.ViewModelSelectListChk { Id = "Muy alta", Valor = "Muy alta" },
                };
                ViewBag.lNivelesActividad = new SelectList(lNivelesActividad, "Id", "Valor");

                //List<Fachada.ViewModelSelectList> lTiposDiabetes = new List<Fachada.ViewModelSelectList>()
                //{
                //    new Fachada.ViewModelSelectList { Id = 1, Valor = "1" },
                //    new Fachada.ViewModelSelectList { Id = 2, Valor = "2" },
                //};
                //ViewBag.listaTiposDiabetes = new SelectList(lTiposDiabetes, "Id", "Valor");

                //Lo mismo que antes - Aca se podría agregar un filtro segun si la medicina es oficial o no a menos que queramos tener en cuenta medicinas registradas por el usuario//
                List<ModelCL.Medicina> lInsulinasRetardadas = db.Medicina.Where(m => m.MedicinaTipo == "Pasiva" && m.Enfermedad.Where(e => e.EnfermedadNombre == "Diabetes tipo 1").FirstOrDefault() != null).ToList();
                ViewBag.listaInsulinasRetardadas = new SelectList(lInsulinasRetardadas, "MedicinaId", "MedicinaNombre");

                List<ModelCL.Medicina> lInsulinasCorreccion = db.Medicina.Where(m => m.MedicinaTipo == "Activa" && m.Enfermedad.Where(e => e.EnfermedadNombre == "Diabetes tipo 1").FirstOrDefault() != null).ToList();
                ViewBag.listaInsulinasCorreccion = new SelectList(lInsulinasCorreccion, "MedicinaId", "MedicinaNombre");

                return PartialView("_DatCli", newDatCli);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        
        [HttpPost]
        [Route("_datcli")]
        [ValidateAntiForgeryToken]
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
                Persona.SexoId = Datos.SexoId;


                if (Datos.DatCliNivelActividad != null || Datos.DatCliAltura != null)
                {
                    ModelCL.Registro oldRegistroDatCliNivelActividad = Persona.Registro.Where(reg => reg.DatCli != null && reg.DatCli.DatCliNivelActividad != null).OrderByDescending(reg => reg.RegistroFchHora).FirstOrDefault();
                    ModelCL.Registro oldRegistroDatCliAltura = Persona.Registro.Where(reg => reg.DatCli != null && reg.DatCli.DatCliAltura != null).OrderByDescending(reg => reg.RegistroFchHora).FirstOrDefault();

                    ModelCL.Registro RegistroDatCli = new ModelCL.Registro();
                    RegistroDatCli.RegistroFchHora = DateTime.Now.Date;

                    ModelCL.DatCli DatCli = new ModelCL.DatCli();

                    if (oldRegistroDatCliNivelActividad != null)
                    {
                        if (oldRegistroDatCliNivelActividad.DatCli.DatCliNivelActividad != Datos.DatCliNivelActividad)
                        {
                            DatCli.DatCliNivelActividad = Datos.DatCliNivelActividad;
                        }                            
                    }
                    else
                    {
                        DatCli.DatCliNivelActividad = Datos.DatCliNivelActividad;
                    }

                    if (oldRegistroDatCliAltura != null)
                    {
                        if (oldRegistroDatCliAltura.DatCli.DatCliAltura != Datos.DatCliAltura)
                        {
                            if (oldRegistroDatCliAltura.RegistroFchHora.Date != DateTime.Now.Date)
                            {
                                DatCli.DatCliAltura = Convert.ToInt16(Datos.DatCliAltura);
                            }
                            else
                            {
                                oldRegistroDatCliAltura.DatCli.DatCliAltura = Convert.ToInt16(Datos.DatCliAltura);
                            }                            
                        }                      
                    }
                    else
                    {
                        DatCli.DatCliAltura = Convert.ToInt16(Datos.DatCliAltura);
                    }

                    RegistroDatCli.DatCli = DatCli;


                    if (RegistroDatCli.DatCli.DatCliNivelActividad != null || RegistroDatCli.DatCli.DatCliAltura!= null)
                    {
                        Persona.Registro.Add(RegistroDatCli);                        
                    }
                }


                if (Datos.PesoValor != null)
                {
                    ModelCL.Registro oldRegistroPeso = Persona.Registro.Where(reg => reg.Peso != null).OrderByDescending(reg => reg.RegistroFchHora).FirstOrDefault();

                    ModelCL.Registro RegistroPeso = new ModelCL.Registro();
                    RegistroPeso.RegistroFchHora = DateTime.Now.Date;

                    ModelCL.Peso Peso = new ModelCL.Peso();
                    Peso.PesoValor = Convert.ToDouble(Datos.PesoValor);

                    RegistroPeso.Peso = Peso;

                    if (oldRegistroPeso != null)
                    {
                        if (oldRegistroPeso.Peso.PesoValor != Peso.PesoValor)
                        {
                            if (oldRegistroPeso.RegistroFchHora.Date != DateTime.Now.Date)
                            {
                                Persona.Registro.Add(RegistroPeso);
                            }
                            else
                            {
                                oldRegistroPeso.Peso.PesoValor = RegistroPeso.Peso.PesoValor;
                            }                            
                        }
                    }
                    else
                    {
                        Persona.Registro.Add(RegistroPeso);
                    }
                }

                

                foreach (Fachada.ViewModelCheckBox enf in Datos.Enfermedades)
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

                            foreach (ModelCL.RelMedRelPerEnf oldrmrpe in oldRelPerEnf.RelMedRelPerEnf.ToList())
                            {
                                Persona.RelPerEnf.Where(rpe => rpe.EnfermedadId == enf.Id).FirstOrDefault().RelMedRelPerEnf.Remove(oldrmrpe);
                            }

                            Persona.RelPerEnf.Remove(oldRelPerEnf);
                        }
                    }
                }


                if (Datos.Enfermedades.Where(e => e.Nombre == "Diabetes tipo 1").FirstOrDefault().Checked == true)
                {
                    ModelCL.Datos oldDatos = Persona.Datos.Where(v => v.Diabetes != null).FirstOrDefault();
                    if (oldDatos != null)
                    {
                        if (oldDatos.Diabetes.DiabetesHidratosPorUniInsu != Datos.DiabetesHidratosPorUniInsu)
                        {
                            Persona.Datos.Remove(oldDatos);

                            ModelCL.Datos newDatos = new ModelCL.Datos();
                            newDatos.Persona = Persona;

                            ModelCL.Diabetes newDiabetes = new ModelCL.Diabetes();
                            newDiabetes.Datos = newDatos;
                            newDiabetes.DiabetesHidratosPorUniInsu = Datos.DiabetesHidratosPorUniInsu;

                            newDatos.Diabetes = newDiabetes;
                            Persona.Datos.Add(newDatos);
                        }


                        ModelCL.RelMedRelPerEnf oldInsulinaRetardada = Persona.RelPerEnf.Where(rpe => rpe.Enfermedad.EnfermedadNombre == "Diabetes tipo 1").FirstOrDefault().RelMedRelPerEnf.Where(rmrpe => rmrpe.Medicina.MedicinaTipo == "Pasiva").FirstOrDefault();

                        if (oldInsulinaRetardada.MedicinaId != Datos.InsulinaRetardadaId)
                        {
                            Persona.RelPerEnf.Where(rpe => rpe.Enfermedad.EnfermedadNombre == "Diabetes tipo 1").FirstOrDefault().RelMedRelPerEnf.Remove(oldInsulinaRetardada);

                            ModelCL.RelMedRelPerEnf newInsulinaRetardada = new ModelCL.RelMedRelPerEnf();

                            newInsulinaRetardada.RelPerEnf = Persona.RelPerEnf.Where(rpe => rpe.Enfermedad.EnfermedadNombre == "Diabetes tipo 1").FirstOrDefault();
                            newInsulinaRetardada.Medicina = db.Medicina.Find(Datos.InsulinaRetardadaId);

                            //Frecuencia harcodeada, hay que agregar luego en la view//
                            newInsulinaRetardada.RelMedRelPerEnfFrecuencia = 1;
                            newInsulinaRetardada.RelMedRelPerEnfFrecuenciaTipo = "día";

                            Persona.RelPerEnf.Where(rpe => rpe.Enfermedad.EnfermedadNombre == "Diabetes tipo 1").FirstOrDefault().RelMedRelPerEnf.Add(newInsulinaRetardada);
                        }


                        ModelCL.RelMedRelPerEnf oldInsulinaCorreccion = Persona.RelPerEnf.Where(rpe => rpe.Enfermedad.EnfermedadNombre == "Diabetes tipo 1").FirstOrDefault().RelMedRelPerEnf.Where(rmrpe => rmrpe.Medicina.MedicinaTipo == "Activa").FirstOrDefault();

                        if (oldInsulinaCorreccion.MedicinaId != Datos.InsulinaCorreccionId)
                        {
                            Persona.RelPerEnf.Where(rpe => rpe.Enfermedad.EnfermedadNombre == "Diabetes tipo 1").FirstOrDefault().RelMedRelPerEnf.Remove(oldInsulinaCorreccion);

                            ModelCL.RelMedRelPerEnf newInsulinaCorreccion = new ModelCL.RelMedRelPerEnf();

                            newInsulinaCorreccion.RelPerEnf = Persona.RelPerEnf.Where(rpe => rpe.Enfermedad.EnfermedadNombre == "Diabetes tipo 1").FirstOrDefault();
                            newInsulinaCorreccion.Medicina = db.Medicina.Find(Datos.InsulinaCorreccionId);

                            //Frecuencia harcodeada, hay que agregar luego en la view//
                            newInsulinaCorreccion.RelMedRelPerEnfFrecuencia = 1;
                            newInsulinaCorreccion.RelMedRelPerEnfFrecuenciaTipo = "día";

                            Persona.RelPerEnf.Where(rpe => rpe.Enfermedad.EnfermedadNombre == "Diabetes tipo 1").FirstOrDefault().RelMedRelPerEnf.Add(newInsulinaCorreccion);
                        }
                    }
                    else
                    {
                        ModelCL.Datos newDatos = new ModelCL.Datos();
                        newDatos.Persona = Persona;

                        ModelCL.Diabetes newDiabetes = new ModelCL.Diabetes();
                        newDiabetes.Datos = newDatos;
                        newDiabetes.DiabetesHidratosPorUniInsu = Datos.DiabetesHidratosPorUniInsu;

                        newDatos.Diabetes = newDiabetes;
                        Persona.Datos.Add(newDatos);
                        

                        ModelCL.RelMedRelPerEnf newInsulinaRetardada = new ModelCL.RelMedRelPerEnf();

                        newInsulinaRetardada.RelPerEnf = Persona.RelPerEnf.Where(rpe => rpe.Enfermedad.EnfermedadNombre == "Diabetes tipo 1").FirstOrDefault();
                        newInsulinaRetardada.Medicina = db.Medicina.Find(Datos.InsulinaRetardadaId);

                        //Frecuencia harcodeada, hay que agregar luego en la view//
                        newInsulinaRetardada.RelMedRelPerEnfFrecuencia = 1;
                        newInsulinaRetardada.RelMedRelPerEnfFrecuenciaTipo = "día";

                        Persona.RelPerEnf.Where(rpe => rpe.Enfermedad.EnfermedadNombre == "Diabetes tipo 1").FirstOrDefault().RelMedRelPerEnf.Add(newInsulinaRetardada);


                        ModelCL.RelMedRelPerEnf newInsulinaCorreccion = new ModelCL.RelMedRelPerEnf();

                        newInsulinaCorreccion.RelPerEnf = Persona.RelPerEnf.Where(rpe => rpe.Enfermedad.EnfermedadNombre == "Diabetes tipo 1").FirstOrDefault();
                        newInsulinaCorreccion.Medicina = db.Medicina.Find(Datos.InsulinaCorreccionId);

                        //Frecuencia harcodeada, hay que agregar luego en la view//
                        newInsulinaCorreccion.RelMedRelPerEnfFrecuencia = 1;
                        newInsulinaCorreccion.RelMedRelPerEnfFrecuenciaTipo = "día";

                        Persona.RelPerEnf.Where(rpe => rpe.Enfermedad.EnfermedadNombre == "Diabetes tipo 1").FirstOrDefault().RelMedRelPerEnf.Add(newInsulinaCorreccion);
                    }     
                }
                else
                {
                    ModelCL.Datos oldDatos = Persona.Datos.Where(v => v.Diabetes != null).FirstOrDefault();

                    if (oldDatos != null)
                    {
                        Persona.Datos.Remove(oldDatos);
                    }
                }
 

                if (id == 0)
                {
                    HttpCookie cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                    FormsAuthenticationTicket usu = FormsAuthentication.Decrypt(cookie.Value);
                    long idUsu = Convert.ToInt32(usu.Name);

                    ModelCL.Usuario Usuario = db.Usuario.Find(idUsu);

                    db.Persona.Add(Persona);

                    ModelCL.RelUsuPer rup = new ModelCL.RelUsuPer();
                    rup.Usuario = Usuario;
                    rup.Persona = Persona;

                    db.RelUsuPer.Add(rup);
                }

                db.SaveChanges();

                return RedirectToAction("Datos_Clinicos");
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        [Route("eliminar")]
        //[ValidateAntiForgeryToken]
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ModelCL.Persona Persona = db.Persona.Find(id);
            if (Persona == null)
            {
                return HttpNotFound();
            }



            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);

            ModelCL.Usuario Usuario = db.Usuario.Find(idUsu);

            if (Usuario.RelUsuPer.Count() == 1)
            {
                TempData["ErrorMessage"] = "No puedes borrar todas tus personas.";
            }
            else
            {                
                if (Request.Cookies["cookiePer"] != null)
                {
                    if (Persona.PersonaId == Convert.ToInt64(Request.Cookies["cookiePer"]["PerId"]))
                    {
                        HttpCookie cookiePer = Request.Cookies["cookiePer"];
                        cookiePer.Expires = DateTime.Now.AddDays(-1d);
                        Response.Cookies.Add(cookiePer);
                    }
                }

                db.Persona.Remove(Persona);
                db.SaveChanges();
            }           

            return RedirectToAction("Datos_Clinicos", "Personas");
        }
    }
}