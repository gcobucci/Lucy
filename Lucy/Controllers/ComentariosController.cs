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
    public class ComentariosController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        // GET: Comentarios
        [Route("_listado")]
        public PartialViewResult _Index(long idContenido)
        {
            ModelCL.Contenido Contenido = db.Contenido.Find(idContenido);

            
            List<ModelCL.Comentario> comentarios = Contenido.Comentario.Where(c => c.ComentarioPadre == null).OrderByDescending(c => c.ComentarioFchHora).ToList();


            return PartialView(comentarios);
        }

        [HttpGet]
        [Route("_crear")]
        public PartialViewResult _Create(long idContenido)
        {
            ComentarioViewModel newComentario = new ComentarioViewModel();

            newComentario.ContenidoId = idContenido;

            return PartialView(newComentario);
        }

        [HttpPost]
        [Route("_crear")]
        [ValidateAntiForgeryToken]
        public ActionResult _Create(ComentarioViewModel datos, string url)
        {
            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);


            ModelCL.Comentario newComentario = new ModelCL.Comentario();

            newComentario.ContenidoId = datos.ContenidoId;
            newComentario.ComentarioCuerpo = datos.ComentarioCuerpo;

            //No estamos teniendo en cuenta que te encajen el id del contenido a prepo ni que el comentario padre pertenezca a otro contenido. Es un quilombo
            if (datos.ComentarioPadreId != null)
            {
                newComentario.ComentarioPadre = db.Comentario.Where(c => c.ComentarioId == datos.ComentarioPadreId).FirstOrDefault();
            }

            newComentario.UsuarioId = idUsu;

            db.Comentario.Add(newComentario);
            db.SaveChanges();

            return null;
        }
    }
}