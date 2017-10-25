using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelCL;
using System.IO;

namespace Backend.Controllers
{
    [RoutePrefix("articulos")]
    public class ArticulosController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        [Route("listado")]
        public ActionResult Index()
        {
            var articulos = db.Contenido.Where(c => c.Articulo != null && c.UsuarioAutor == null).ToList();

            return View(articulos);
        }

        [Route("ver")]
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModelCL.Contenido contArticulo = db.Contenido.Find(id);
            if (contArticulo.Articulo == null)
            {
                return HttpNotFound();
            }
            return View(contArticulo);
        }

        [Route("crear")]
        public ActionResult Create()
        {
            List<ModelCL.Tema> lTemas = db.Tema.ToList();
            ViewBag.lTemas = lTemas;

            return View();
        }

        [HttpPost]
        [Route("crear")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ModelCL.Contenido contenido, HttpPostedFileBase[] files, int[] temas)
        {          
            if (ModelState.IsValid)
            {
                short cont = 0;
                foreach (var file in files)
                {
                    if (file != null)
                    {
                        cont += 1;

                        if (!Fachada.Functions.isValidContentType(file.ContentType))
                        {
                            ViewBag.ErrorMessage = "Solo se aceptan formatos de archivos JPG, JPEG, PNG y GIF.";
                            return View(contenido);
                        }
                        else if (!Fachada.Functions.isValidContentLength(file.ContentLength))
                        {
                            ViewBag.ErrorMessage = "El archivo es muy pesado.";
                            return View(contenido);
                        }
                        else
                        {

                            if (file.ContentLength > 0)
                            {
                                //var fileName = Path.GetFileName(file.FileName);
                                string tipoArchivo = "";
                                if (file.ContentType.Split('/')[0] == "image")
                                {
                                    tipoArchivo = "Imagenes";
                                }
                                else if (file.ContentType.Split('/')[0] == "video")
                                {
                                    tipoArchivo = "Videos";
                                }
                                else
                                {
                                    ViewBag.ErrorMessage = "Error inesperado";
                                    return View(contenido); //Error inesperado
                                }

                                string nombreArchivo = Guid.NewGuid().ToString() + "." + file.ContentType.Split('/')[1];
                                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Lucy/Resources/Oficial", tipoArchivo, "Articulos", nombreArchivo);

                                ModelCL.Multimedia m = new ModelCL.Multimedia();
                                m.MultimediaUrl = "Resources/Oficial/" + tipoArchivo + "/Articulos/" + nombreArchivo;
                                m.MultimediaTipo = file.ContentType.Split('/')[0];
                                m.MultimediaOrden = cont;

                                contenido.Multimedia.Add(m);

                                file.SaveAs(path);
                            }
                        }
                    }
                }
                           


                ModelCL.Articulo newArticulo = new ModelCL.Articulo();

                if (temas != null)
                {
                    foreach (var t in temas)
                    {
                        newArticulo.Tema.Add(db.Tema.Find(t));
                    }
                }

                contenido.Articulo = newArticulo;
                db.Contenido.Add(contenido);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            List<ModelCL.Tema> lTemas = db.Tema.ToList();
            ViewBag.lTemas = lTemas;

            return View(contenido);
        }

        [Route("editar")]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ModelCL.Contenido contArticulo = db.Contenido.Find(id);
            if (contArticulo.Articulo == null)
            {
                return HttpNotFound();
            }

            List<ModelCL.Tema> lTemas = db.Tema.ToList();
            ViewBag.lTemas = lTemas;

            return View(contArticulo);
        }

        [HttpPost]
        [Route("editar")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ModelCL.Contenido contenido, HttpPostedFileBase[] files, int[] checkBorrar, int[] temas)
        {
            if (ModelState.IsValid)
            {                
                ModelCL.Contenido oldContenido = db.Contenido.Find(contenido.ContenidoId);
                oldContenido.ContenidoTitulo = contenido.ContenidoTitulo;
                oldContenido.ContenidoDescripcion = contenido.ContenidoDescripcion;
                oldContenido.ContenidoCuerpo = contenido.ContenidoCuerpo;

                //Agregar o cambiar multimedia
                short cont = 0;
                foreach (var file in files)
                {
                    cont += 1;

                    if (file != null)
                    {
                        if (!Fachada.Functions.isValidContentType(file.ContentType))
                        {
                            ViewBag.ErrorMessage = "Solo se aceptan formatos de archivos JPG, JPEG, PNG y GIF.";
                            return View(contenido);
                        }
                        else if (!Fachada.Functions.isValidContentLength(file.ContentLength))
                        {
                            ViewBag.ErrorMessage = "El archivo es muy pesado.";
                            return View(contenido);
                        }
                        else
                        {
                            if (file.ContentLength > 0)
                            {
                                //var fileName = Path.GetFileName(file.FileName);
                                string tipoArchivo = "";
                                if (file.ContentType.Split('/')[0] == "image")
                                {
                                    tipoArchivo = "Imagenes";
                                }
                                else if (file.ContentType.Split('/')[0] == "video")
                                {
                                    tipoArchivo = "Videos";
                                }
                                else
                                {
                                    ViewBag.ErrorMessage = "Error inesperado";
                                    return View(contenido); //Error inesperado
                                }

                                ModelCL.Multimedia oldMult = oldContenido.Multimedia.Where(m => m.MultimediaOrden == cont).FirstOrDefault();

                                string nombreArchivo = Guid.NewGuid().ToString() + "." + file.ContentType.Split('/')[1];
                                var newPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Lucy/Resources/Oficial", tipoArchivo, "Articulos", nombreArchivo);

                                string newUrl = "Resources/Oficial/" + tipoArchivo + "/Articulos/" + nombreArchivo;

                                if (oldMult != null)
                                {
                                    var oldPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Lucy/", oldMult.MultimediaUrl);
                                    if (System.IO.File.Exists(oldPath))
                                    {
                                        System.IO.File.Delete(oldPath);
                                    }                                        

                                    //oldContenido.Multimedia.Remove(oldMult);
                                    oldMult.MultimediaUrl = newUrl;
                                    oldMult.MultimediaTipo = file.ContentType.Split('/')[0];
                                }
                                else
                                {
                                    ModelCL.Multimedia newMult = new ModelCL.Multimedia();

                                    newMult.MultimediaUrl = newUrl;

                                    newMult.MultimediaTipo = file.ContentType.Split('/')[0];
                                    newMult.MultimediaOrden = Convert.ToInt16(oldContenido.Multimedia.Count() + 1);

                                    oldContenido.Multimedia.Add(newMult);
                                }

                                file.SaveAs(newPath);
                            }
                        }
                    }
                }


                //Eliminar multimedia
                if (checkBorrar != null)
                {
                    foreach (int c in checkBorrar)
                    {
                        ModelCL.Multimedia oldM = db.Multimedia.Where(m => m.MultimediaId == c).FirstOrDefault();

                        List<ModelCL.Multimedia> siguientesM = oldM.Contenido.Multimedia.Where(m => m.MultimediaOrden > oldM.MultimediaOrden).ToList();

                        if (siguientesM.Count() != 0)
                        {
                            foreach (ModelCL.Multimedia sigM in siguientesM)
                            {
                                sigM.MultimediaOrden -= 1;
                            }
                        }

                        var oldPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Lucy/", oldM.MultimediaUrl);
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }

                        db.Multimedia.Remove(oldM);
                    }
                }


                List<ModelCL.Tema> bkTemas = oldContenido.Articulo.Tema.ToList();
                foreach (ModelCL.Tema oldTema in bkTemas)
                {
                    oldContenido.Articulo.Tema.Remove(oldTema);
                }
                //bkTemas = null;


                if (temas != null)
                {
                    foreach (var t in temas)
                    {
                        oldContenido.Articulo.Tema.Add(db.Tema.Find(t));
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            List<ModelCL.Tema> lTemas = db.Tema.ToList();
            ViewBag.lTemas = lTemas;

            return View(contenido);
        }

        [Route("eliminar")]
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModelCL.Contenido contArticulo = db.Contenido.Find(id);
            if (contArticulo.Articulo == null)
            {
                return HttpNotFound();
            }
            return View(contArticulo);
        }

        [HttpPost, ActionName("Delete")]
        [Route("eliminar")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ModelCL.Contenido contArticulo = db.Contenido.Find(id);
            foreach (ModelCL.Multimedia m in contArticulo.Multimedia.ToList())
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Lucy/", m.MultimediaUrl);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
            }

            db.Contenido.Remove(contArticulo);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
