using ModelCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

        [Route("_cambiar_datos_generales")]
        public PartialViewResult _CambiarDatosGenerales()
        {
            return PartialView();
        }

        [Route("_cambiar_contraseña")]
        public PartialViewResult _CambiarContraseña()
        {
            return PartialView();
        }

        [Route("_cambiar_email")]
        public PartialViewResult _CambiarEmail()
        {
            return PartialView();
        }

        [Route("_premium")]
        public PartialViewResult _Premium()
        {
            return PartialView();
        }

        [Route("_notificaciones")]
        public PartialViewResult _Notificaciones()
        {
            return PartialView();
        }                
    }
}