using ModelCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Lucy.Controllers
{
    public class HomeController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public PartialViewResult _Personas()
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

            return PartialView("_Personas", lPersonas);
        }
    }
}