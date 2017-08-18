using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelCL;
using System.Web.Security;

namespace Lucy.Controllers
{
    public class RecetasController : Controller
    {
        [HttpGet]
        public ActionResult List()
        {
            using (AgustinaEntities db = new AgustinaEntities())
            {
                #region UsuarioId por cookie
                HttpCookie cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                FormsAuthenticationTicket usu = FormsAuthentication.Decrypt(cookie.Value);
                int idUsu = Convert.ToInt32(usu.Name);
                #endregion

                List<ModelCL.Receta> recetas = db.Receta.Where(r => r.Contenido.UsuarioAutor == null || r.Contenido.UsuarioAutor.UsuarioId == idUsu).ToList();

                return View(recetas);
            }
        }
    }
}