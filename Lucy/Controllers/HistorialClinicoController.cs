using ModelCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lucy.Controllers
{
    [Authorize]
    [RoutePrefix("historial_clinico")]
    public class HistorialClinicoController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        [Route("")]
        public ActionResult Index()
        {
            long idPer = Convert.ToInt64(Request.Cookies["cookiePer"]["PerId"]);

            ViewBag.Persona = db.Persona.Find(idPer).nombreCompleto;

            return View();
        }

        [Route("_relacion_peso_altura")]
        public PartialViewResult _RelacionPesoAltura()
        {
            long idPer = Convert.ToInt64(Request.Cookies["cookiePer"]["PerId"]);

            ViewBag.Persona = db.Persona.Find(idPer).nombreCompleto;

            return PartialView();
        }

        [Route("_dieta")]
        public PartialViewResult _Dieta()
        {
            long idPer = Convert.ToInt64(Request.Cookies["cookiePer"]["PerId"]);

            ViewBag.Persona = db.Persona.Find(idPer).nombreCompleto;

            return PartialView();
        }
    }
}