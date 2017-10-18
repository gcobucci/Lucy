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
    [RoutePrefix("recetas")]
    public class RecetasController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        [Route("index")]
        public ActionResult Index()
        {
            var recetas = db.Contenido.Where(c => c.Receta != null).ToList();

            return View(recetas);
        }

        [Route("details")]
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModelCL.Contenido contReceta = db.Contenido.Find(id);
            if (contReceta.Receta == null)
            {
                return HttpNotFound();
            }
            return View(contReceta);
        }

        [Route("create")]
        public ActionResult Create()
        {
            //ViewBag.RecetaId = new SelectList(db.Contenido, "ContenidoId", "ContenidoTitulo");
            return View();
        }

        [HttpPost]
        [Route("create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ModelCL.Contenido contenido, HttpPostedFileBase[] files)
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
                            return View();
                        }
                        else if (!Fachada.Functions.isValidContentLength(file.ContentLength))
                        {
                            ViewBag.ErrorMessage = "El archivo es muy pesado.";
                            return View();
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
                                    return View(); //Error inesperado
                                }

                                string nombreArchivo = Guid.NewGuid().ToString() + "." + file.ContentType.Split('/')[1];
                                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Lucy/Resources/Oficial", tipoArchivo, "Recetas", nombreArchivo);

                                ModelCL.Multimedia m = new ModelCL.Multimedia();
                                //m.MultimediaUrl = Path.Combine("Oficial", tipoArchivo, "Recetas", nombreArchivo);
                                m.MultimediaUrl = "Resources/Oficial/" + tipoArchivo + "/Recetas/" + nombreArchivo; //Ver cual es mejor entre estos 2
                                m.MultimediaTipo = file.ContentType.Split('/')[0];
                                m.MultimediaOrden = cont;

                                contenido.Multimedia.Add(m);

                                file.SaveAs(path);
                            }
                        }
                    }                    
                }
                
                //ModelCL.Contenido newContenido = new ModelCL.Contenido();
                //newContenido.ContenidoTitulo = contenido.ContenidoTitulo;
                //newContenido.ContenidoCuerpo = contenido.ContenidoCuerpo;

                //ModelCL.Receta newReceta = new ModelCL.Receta();

                //newReceta.RecetaCalorias = contenido.Receta.RecetaCalorias;
                //newReceta.RecetaHidratos = contenido.Receta.RecetaHidratos;
                //newReceta.RecetaGluten = contenido.Receta.RecetaGluten;
                //newReceta.RecetaSodio = contenido.Receta.RecetaSodio;              

                //newContenido.Receta

                db.Contenido.Add(contenido);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.RecetaId = new SelectList(db.Contenido, "ContenidoId", "ContenidoTitulo", receta.RecetaId);
            return View(contenido);
        }

        [Route("edit")]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ModelCL.Contenido contReceta = db.Contenido.Find(id);
            if (contReceta.Receta == null)
            {
                return HttpNotFound();
            }
            //ViewBag.RecetaId = new SelectList(db.Contenido, "ContenidoId", "ContenidoTitulo", receta.RecetaId);
            return View(contReceta);
        }

        [HttpPost]
        [Route("edit")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ModelCL.Contenido contenido, HttpPostedFileBase[] files)
        {
            if (ModelState.IsValid)
            {
                ModelCL.Contenido oldContenido = db.Contenido.Find(contenido.ContenidoId);
                oldContenido.ContenidoTitulo = contenido.ContenidoTitulo;
                oldContenido.ContenidoDescripcion = contenido.ContenidoDescripcion;
                oldContenido.ContenidoCuerpo = contenido.ContenidoCuerpo;

                oldContenido.Receta.RecetaCalorias = contenido.Receta.RecetaCalorias;
                oldContenido.Receta.RecetaHidratos = contenido.Receta.RecetaHidratos;
                oldContenido.Receta.RecetaGluten = contenido.Receta.RecetaGluten;
                oldContenido.Receta.RecetaSodio = contenido.Receta.RecetaSodio;

                //db.Entry(contenido).State = EntityState.Modified;
                //db.Entry(contenido.Receta).State = EntityState.Modified;

                short cont = 0;
                foreach (var file in files)
                {
                    cont += 1;

                    if (file != null)
                    {                      
                        if (!Fachada.Functions.isValidContentType(file.ContentType))
                        {
                            ViewBag.ErrorMessage = "Solo se aceptan formatos de archivos JPG, JPEG, PNG y GIF.";
                            return View();
                        }
                        else if (!Fachada.Functions.isValidContentLength(file.ContentLength))
                        {
                            ViewBag.ErrorMessage = "El archivo es muy pesado.";
                            return View();
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
                                    return View(); //Error inesperado
                                }

                                ModelCL.Multimedia oldMult = oldContenido.Multimedia.Where(m => m.MultimediaOrden == cont).FirstOrDefault();

                                string nombreArchivo = Guid.NewGuid().ToString() + "." + file.ContentType.Split('/')[1];
                                var newPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Lucy/Resources/Oficial", tipoArchivo, "Recetas", nombreArchivo);

                                string newUrl = "Resources/Oficial/" + tipoArchivo + "/Recetas/" + nombreArchivo;

                                if (oldMult != null)
                                {
                                    var oldPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Lucy/", oldMult.MultimediaUrl);
                                    if (System.IO.File.Exists(oldPath))
                                        System.IO.File.Delete(oldPath);

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

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(contenido);
        }

        [Route("delete")]
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModelCL.Contenido contReceta = db.Contenido.Find(id);
            if (contReceta.Receta == null)
            {
                return HttpNotFound();
            }
            return View(contReceta);
        }

        [HttpPost, ActionName("Delete")]
        [Route("delete")]        
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ModelCL.Contenido contReceta = db.Contenido.Find(id);
            foreach (ModelCL.Multimedia m in contReceta.Multimedia.ToList())
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Lucy/", m.MultimediaUrl);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
            }

            db.Contenido.Remove(contReceta);     
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
