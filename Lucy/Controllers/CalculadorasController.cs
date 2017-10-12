using Lucy.Models;
using ModelCL;
using Fachada;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Lucy.Controllers
{
    [RoutePrefix("calculadoras")]
    public class CalculadorasController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        [HttpGet]
        [Route("imc")]
        public ActionResult calcularIMC()
        {
            CalcIMCViewModel cimc = new CalcIMCViewModel();

            if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
            {
                //long idPer = Fachada.Functions.get_idPer(Request.Cookies[FormsAuthentication.FormsCookieName]);
                long idPer = 1;


                //cimc.PersonaId = idPer;

                ModelCL.Persona per = db.Persona.Find(idPer);
                                
                ModelCL.Registro regPeso = per.Registro.Where(r => r.Peso != null).OrderByDescending(r => r.RegistroFchHora).FirstOrDefault();
                if (regPeso != null)
                {
                    cimc.Peso = regPeso.Peso.PesoValor;
                }

                ModelCL.Registro regDatCliAltura = per.Registro.Where(r => r.DatCli != null && r.DatCli.DatCliAltura != null).OrderByDescending(r => r.RegistroFchHora).FirstOrDefault();
                if (regDatCliAltura != null)
                {
                    cimc.Altura = Convert.ToInt16(regDatCliAltura.DatCli.DatCliAltura);
                }

                if (cimc.Peso != 0 || cimc.Altura != 0)
                {
                    ViewBag.Persona = per.nombreCompleto;
                }                
            }
        
            return View(cimc);
        }

        [HttpPost]
        [Route("imc")]
        [ValidateAntiForgeryToken]
        public ActionResult calcularIMC(CalcIMCViewModel datos)
        {
            double imc = Fachada.Functions.calcular_IMC(datos.Peso, datos.Altura);

            ViewBag.imc = imc;

            if (imc < 18.5)
            {
                ViewBag.imcMessage = "Tu peso es bajo, ten cuidado, recuerda que estar muy delgado no es bueno para la salud.";
            }
            else if (imc >= 18.5 && imc <= 24.9){
                ViewBag.imcMessage = "Tu peso es normal.";
            }
            else if (imc > 24.9 && imc <= 29.9)
            {
                ViewBag.imcMessage = "Tu valor de IMC indica que tienes sobrepeso.";
            }
            else //imc > 29.9
            {
                ViewBag.imcMessage = "Tu valor de IMC indica que tienes obesidad. Te recomendamos consultar con un médico.";
            }

            return View();
        }


        [HttpGet]
        [Route("tmb")]
        public ActionResult calcularTMB()
        {
            CalcTMBViewModel cimc = new CalcTMBViewModel();

            if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
            {
                //long idPer = Fachada.Functions.get_idPer(Request.Cookies[FormsAuthentication.FormsCookieName]);
                long idPer = 1;


                //cimc.PersonaId = idPer;

                ModelCL.Persona per = db.Persona.Find(idPer);

                ModelCL.Registro regPeso = per.Registro.Where(r => r.Peso != null).OrderByDescending(r => r.RegistroFchHora).FirstOrDefault();
                if (regPeso != null)
                {
                    cimc.Peso = regPeso.Peso.PesoValor;
                }

                ModelCL.Registro regDatCliAltura = per.Registro.Where(r => r.DatCli != null && r.DatCli.DatCliAltura != null).OrderByDescending(r => r.RegistroFchHora).FirstOrDefault();
                if (regDatCliAltura != null)
                {
                    cimc.Altura = Convert.ToInt16(regDatCliAltura.DatCli.DatCliAltura);
                }
                                
                cimc.Edad = per.edad;
                cimc.Sexo = per.SexoId;

                ModelCL.Registro regDatCliActividad = per.Registro.Where(r => r.DatCli != null && r.DatCli.DatCliNivelActividad != null).OrderByDescending(r => r.RegistroFchHora).FirstOrDefault();
                if (regDatCliActividad != null)
                {
                    cimc.NivelActividad = regDatCliActividad.DatCli.DatCliNivelActividad;
                }

                ViewBag.Persona = per.nombreCompleto;                
            }

            List<ModelCL.Sexo> lSexos = db.Sexo.Where(s => s.SexoNombre == "Hombre" || s.SexoNombre == "Mujer").ToList();
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

            return View(cimc);
        }
        
        [HttpPost]
        [Route("tmb")]
        [ValidateAntiForgeryToken]
        public ActionResult calcularTMB(CalcTMBViewModel datos)
        {
            short tmb = Fachada.Functions.calcular_TMB(datos.Peso, datos.Altura, datos.Edad, (db.Sexo.Find(datos.Sexo)).SexoNombre);

            ViewBag.tmb = tmb;

            ViewBag.CaloriasParaMantenerse = Fachada.Functions.calcular_calorias_para_mantenerse(tmb, datos.NivelActividad);


            List<ModelCL.Sexo> lSexos = db.Sexo.Where(s => s.SexoNombre == "Hombre" || s.SexoNombre == "Mujer").ToList();
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

            return View();
        }
    }
}