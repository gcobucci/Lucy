using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelCL;
using System.Web.Security;

namespace Lucy.Controllers
{
    public class AlimentosController : Controller
    {
        // GET: InfoNutricional
        [AllowAnonymous]
        [HttpGet]
        public ActionResult InfNut()
        {
            using (AgustinaEntities db = new AgustinaEntities())
            {
                List<ModelCL.Alimento> Alimentos = null;

                if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
                {
                    #region UsuarioId por cookie
                    HttpCookie cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                    FormsAuthenticationTicket usu = FormsAuthentication.Decrypt(cookie.Value);
                    int idUsu = Convert.ToInt32(usu.Name);
                    #endregion

                    Alimentos = db.Alimento.Where(a => a.Usuario == null || a.Usuario.UsuarioId == idUsu).ToList();
                }
                else
                {
                    Alimentos = db.Alimento.Where(a => a.Usuario == null).ToList();
                }
                
                return View(Alimentos);
            }
        }
    }
}