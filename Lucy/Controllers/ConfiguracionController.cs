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
    [RoutePrefix("configuracion")]
    public class ConfiguracionController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        [Route("")]
        public ActionResult Index()
        {
            return View();
        }


        //[Route("_cambiar_datos_generales")]
        //public PartialViewResult _CambiarDatosGenerales()
        //{
        //    return PartialView();
        //}


        //[Route("_cambiar_contraseña")]
        //public PartialViewResult _CambiarContraseña()
        //{
        //    return PartialView();
        //}


        //[Route("_cambiar_email")]
        //public PartialViewResult _CambiarEmail()
        //{
        //    return PartialView();
        //}


        [HttpGet]
        [Route("_premium")]
        public PartialViewResult _Premium()
        {
            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);

            ModelCL.Usuario Usuario = db.Usuario.Find(idUsu);

            bool esPremium = false;

            if (Usuario.RelUsuRol.Where(rur => rur.Rol.RolNombre == "Premium").FirstOrDefault() != null)
            {
                esPremium = true;
            }

            ViewBag.esPremium = esPremium;

            return PartialView();
        }

        [HttpPost]
        [Route("_premium")]
        public ActionResult _Premium(bool accion) //true = suscribrirse - false = cancelar
        {
            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);

            ModelCL.Usuario Usuario = db.Usuario.Find(idUsu);


            if (accion == true)
            {
                if (Usuario.RelUsuRol.Where(rur => rur.Rol.RolNombre == "Premium").FirstOrDefault() == null)
                {
                    ModelCL.RelUsuRol rur = new ModelCL.RelUsuRol();

                    rur.Usuario = Usuario;
                    rur.Rol = db.Rol.Where(r => r.RolNombre == "Premium").FirstOrDefault();

                    db.RelUsuRol.Add(rur);

                    db.SaveChanges();
                }
                else
                {
                    ViewBag.ErrorMessage = "Error inesperado";
                }
            }
            else
            {
                ModelCL.RelUsuRol rur = Usuario.RelUsuRol.Where(r => r.Rol.RolNombre == "Premium").FirstOrDefault();

                if (rur != null)
                {
                    Usuario.RelUsuRol.Remove(rur);

                    db.SaveChanges();
                }
                else
                {
                    ViewBag.ErrorMessage = "Error inesperado";
                }
            }
            
            return RedirectToAction("Index");
        }


        [Route("_notificaciones")]
        public PartialViewResult _Notificaciones()
        {
            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);

            ModelCL.Usuario Usuario = db.Usuario.Find(idUsu);


            ConfiguracionNotificacionesViewModel confNot = new ConfiguracionNotificacionesViewModel();

            confNot.UsuarioRecibirEmails = Usuario.UsuarioRecibirEmails;

            return PartialView(confNot);
        }


        [HttpPost]
        [Route("_notificaciones")]
        public ActionResult _Notificaciones(ConfiguracionNotificacionesViewModel datos) //true = suscribrirse - false = cancelar
        {
            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);

            ModelCL.Usuario Usuario = db.Usuario.Find(idUsu);

            Usuario.UsuarioRecibirEmails = datos.UsuarioRecibirEmails;

            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}