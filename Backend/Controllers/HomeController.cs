using ModelCL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Backend.Controllers
{
    public class HomeController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        // GET: Home
        public ActionResult Index()
        {

            //Count de datos
            ViewBag.TotalUsers = db.Usuario.Count();

            ViewBag.TotalMales = db.Persona.Where(p => p.Sexo.SexoNombre == "Hombre").Count();

            ViewBag.TotalFemales = db.Persona.Where(p => p.Sexo.SexoNombre == "Mujer").Count();

            ViewBag.TotalInter = db.Persona.Where(p => p.Sexo.SexoNombre == "Intersexual").Count();

            ViewBag.TotalDiabeticos = db.RelPerEnf.Where(p => p.Enfermedad.EnfermedadNombre == "Diabetes tipo 1" || p.Enfermedad.EnfermedadNombre == "Diabetes tipo 2").GroupBy(p => p.Persona).Count();

            ViewBag.TotalCeliacos = db.RelPerEnf.Where(p => p.Enfermedad.EnfermedadNombre == "Celiaquía").Count();

            ViewBag.TotalObesos = db.RelPerEnf.Where(p => p.Enfermedad.EnfermedadNombre == "Obesidad").Count();

            ViewBag.TotalEnfermedades = db.Enfermedad.GroupBy(p => p.EnfermedadNombre).Count();

            ViewBag.TotalMedicina = db.Medicina.GroupBy(p => p.MedicinaNombre).Count();

            ViewBag.TotalDailyConections = db.Usuario.Where(p => DbFunctions.TruncateTime(p.UsuarioLastLogin) == (DateTime.Today)).Count();


            //Porcentaje hombres
            ViewBag.PercentMales = (((db.Persona.Where(p => p.Sexo.SexoNombre == "Hombre").Count())*100)/ db.Usuario.Count());

            //Porcentaje mujeres
            ViewBag.PercentFemales = (((db.Persona.Where(p => p.Sexo.SexoNombre == "Mujer").Count()) * 100) / db.Usuario.Count());

            //Porcentaje inter
            ViewBag.PercentInter = (((db.Persona.Where(p => p.Sexo.SexoNombre == "Intersexual").Count()) * 100) / db.Usuario.Count());

            //Porcentaje diabeticos
            ViewBag.PercentDiabeticos = (((db.RelPerEnf.Where(p => p.Enfermedad.EnfermedadNombre == "Diabetes tipo 1" || p.Enfermedad.EnfermedadNombre == "Diabetes tipo 2").Count()) * 100) / db.Usuario.Count());

            //Porcentaje celiacos
            ViewBag.PercentCeliacos = (((db.RelPerEnf.Where(p => p.Enfermedad.EnfermedadNombre == "Celiaquía").Count()) * 100) / db.Usuario.Count());

            //Porcentaje obesos
            ViewBag.PercentObesos = (((db.RelPerEnf.Where(p => p.Enfermedad.EnfermedadNombre == "Obesidad").Count()) * 100) / db.Usuario.Count());

            return View();
        }
    }
}