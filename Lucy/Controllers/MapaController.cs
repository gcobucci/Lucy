using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lucy.Controllers
{
    [RoutePrefix("mapa")]
    public class MapaController : Controller
    {
        // GET: Mapa
        [Route("")]
        public ActionResult Index()
        {
            return View();
        }
    }
}