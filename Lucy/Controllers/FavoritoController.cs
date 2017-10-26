using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelCL;
using System.Web.Security;

namespace Lucy.Controllers
{
    public class FavoritoController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();
        
        public PartialViewResult _Index()
        {
            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);
            ViewBag.idUsu = idUsu;
            ModelCL.Usuario myUsu = db.Usuario.Find(idUsu);

            List<ModelCL.Contenido> favoritos = myUsu.ContenidosFav.OrderBy(r => r.ContenidoTitulo).ToList();
            
            return PartialView(favoritos);
        }

        // Guardar un Favorito
        public ActionResult saveFav(long idUsu, int idCont)
        {
            Contenido Contenido = db.Contenido.Find(idCont);

            try
            {
                if (Contenido.UsuariosFav.Where(u => u.UsuarioId == idUsu).FirstOrDefault() == null)
                {
                    Contenido.UsuariosFav.Add(db.Usuario.Find(idUsu));
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            return null;
        }

        public ActionResult deleteFav(long idUsu, int idCont)
        {
            Contenido Contenido = db.Contenido.Find(idCont);

            try
            {
                if (Contenido.UsuariosFav.Where(u => u.UsuarioId == idUsu).FirstOrDefault() != null)
                {
                    Contenido.UsuariosFav.Remove(db.Usuario.Find(idUsu));
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            return null;
        }
    }
}