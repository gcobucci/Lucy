using ModelCL;
using System;
using System.Collections.Generic;
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
            ViewBag.TotalUsers = db.Usuario.Count();

            ViewBag.TotalMales = db.Persona.Where(p => p.Sexo.SexoNombre == "Hombre").Count();

            ViewBag.TotalFemales = db.Persona.Where(p => p.Sexo.SexoNombre == "Mujer").Count();

            return View();
        }
    }
}