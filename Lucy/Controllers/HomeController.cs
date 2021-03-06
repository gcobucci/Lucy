﻿using ModelCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            //string valorEn = Fachada.Functions.encriptar("1");

            //string valorDe = Fachada.Functions.desencriptar(valorEn);

            if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
            {
                long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);

                ModelCL.Usuario Usuario = db.Usuario.Find(idUsu);
                List<ModelCL.RelUsuPer> lRelUsuPer = Usuario.RelUsuPer.ToList();
                List<ModelCL.Persona> lPersonas = new List<ModelCL.Persona>();
                foreach (ModelCL.RelUsuPer rup in lRelUsuPer)
                {
                    lPersonas.Add(rup.Persona);
                }
                
                if (Request.Cookies["cookiePer"] == null)
                {
                    HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                    FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
                    DateTime expiration = ticket.Expiration;

                    var auxTO = expiration - DateTime.Now;
                    double timeout = auxTO.TotalMinutes;
                    var v = db.Usuario.Where(a => a.UsuarioId == idUsu).FirstOrDefault();

                    var cookiePer = new HttpCookie("cookiePer");
                    cookiePer.Expires = DateTime.Now.AddMinutes(timeout);
                    idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);
                    cookiePer.Values["PerId"] = v.RelUsuPer.Where(u => u.UsuarioId == idUsu).FirstOrDefault().PersonaId.ToString();
                    Response.Cookies.Add(cookiePer);
                }
                return PartialView("_Personas", lPersonas);
            }
            else
            {
                return null;
            }
        }

        // Guardar Persona a cargo en cookie
        public ActionResult selPersona(int PerId, string url)
        {
            if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
            {
                long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);

                ModelCL.Usuario Usuario = db.Usuario.Find(idUsu);
                List<ModelCL.RelUsuPer> lRelUsuPer = Usuario.RelUsuPer.ToList();
                List<ModelCL.Persona> lPersonas = new List<ModelCL.Persona>();
                foreach (ModelCL.RelUsuPer rup in lRelUsuPer)
                {
                    lPersonas.Add(rup.Persona);
                }

                foreach (ModelCL.Persona item in lPersonas)
                {
                    if (item.PersonaId == PerId)
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
                }
                
            }
            return Redirect(url);
        }
    }
}