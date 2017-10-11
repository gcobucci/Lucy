using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelCL;

namespace Lucy.Controllers
{
    public class FavoritoController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        // Guardar un Favorito
        public ActionResult saveFav(int idUsu, int idCont)
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

        public ActionResult deleteFav(int idUsu, int idCont)
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