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
            if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
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
            else
            {
                return null;
            }
        }

        // Guardar Persona a cargo en cookie
        public ActionResult selPersona(int PerId)
        {

            if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
            {
                HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
                DateTime expiration = ticket.Expiration;

                var auxTO = expiration - DateTime.Now;
                double timeout = auxTO.TotalMinutes;

                var cookiePer = new HttpCookie("cookiePer");
                cookiePer.Expires = DateTime.Now.AddMinutes(timeout);
                cookiePer.Values["PerId"] = PerId.ToString();
                Response.Cookies.Add(cookiePer);
            }
            
            return null;
        }
    }
}