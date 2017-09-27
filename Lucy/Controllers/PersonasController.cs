using Lucy.Models;
using ModelCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Lucy.Controllers
{
    [Authorize]
    public class PersonasController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        [HttpGet]
        public ActionResult Datos_Clinicos()
        {
            HttpCookie cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket usu = FormsAuthentication.Decrypt(cookie.Value);
            int idUsu = Convert.ToInt32(usu.Name);

            ModelCL.Usuario Usuario = db.Usuario.Find(idUsu);
            List<ModelCL.RelUsuPer> lRelUsuPer = Usuario.RelUsuPer.ToList();
            List<ModelCL.Persona> lPersonas = new List<ModelCL.Persona>();
            foreach (ModelCL.RelUsuPer rup in lRelUsuPer)
            {
                lPersonas.Add(rup.Persona);
            }
            ModelCL.Persona Persona = new ModelCL.Persona();
            Persona.PersonaId = 0;
            Persona.PersonaNombre = ">> CREAR NUEVA PERSONA <<";
            lPersonas.Add(Persona);
            ViewBag.listaPersonas = new SelectList(lPersonas, "PersonaId", "nombreCompleto");
            return View();
        }

        [HttpGet]
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



                    ModelCL.Registro RegistroPeso = Persona.Registro.Where(reg => reg.Peso != null).OrderByDescending(reg => reg.RegistroFchHora).FirstOrDefault();                     

                    if (RegistroPeso != null)
                    {
                        newDatCli.PesoValor = RegistroPeso.Peso.PesoValor;
                    }



                    ModelCL.Registro RegistroDatCli = Persona.Registro.Where(reg => reg.DatCli != null).OrderByDescending(reg => reg.RegistroFchHora).FirstOrDefault();              

                    if (RegistroDatCli != null)
                    {
                        newDatCli.DatCliAltura = RegistroDatCli.DatCli.DatCliAltura;
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

                
                if (Datos.PesoValor != null)
                {
                    ModelCL.Registro oldRegistroPeso = Persona.Registro.Where(reg => reg.Peso != null).OrderByDescending(reg => reg.RegistroFchHora).FirstOrDefault();

                    ModelCL.Registro RegistroPeso = new ModelCL.Registro();
                    RegistroPeso.RegistroFchHora = DateTime.Now;

                    ModelCL.Peso Peso = new ModelCL.Peso();
                    Peso.PesoValor = Convert.ToDouble(Datos.PesoValor);

                    RegistroPeso.Peso = Peso;

                    if (oldRegistroPeso != null)
                    {
                        if (oldRegistroPeso.Peso.PesoValor != Peso.PesoValor)
                        {
                            Persona.Registro.Add(RegistroPeso);
                        }
                    }
                    else
                    {
                        Persona.Registro.Add(RegistroPeso);
                    }
                }


                if (Datos.DatCliAltura != null)
                {
                    ModelCL.Registro oldRegistroDatCli = Persona.Registro.Where(reg => reg.DatCli != null).OrderByDescending(reg => reg.RegistroFchHora).FirstOrDefault();

                    ModelCL.Registro RegistroDatCli = new ModelCL.Registro();
                    RegistroDatCli.RegistroFchHora = DateTime.Now;

                    ModelCL.DatCli DatCli = new ModelCL.DatCli();
                    DatCli.DatCliAltura = Convert.ToInt16(Datos.DatCliAltura);

                    RegistroDatCli.DatCli = DatCli;

                    if (oldRegistroDatCli != null)
                    {
                        if (oldRegistroDatCli.DatCli.DatCliAltura != DatCli.DatCliAltura)
                        {
                            Persona.Registro.Add(RegistroDatCli);
                        }
                    }
                    else
                    {
                        Persona.Registro.Add(RegistroDatCli);
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
                    int idUsu = Convert.ToInt32(usu.Name);

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
    }
}