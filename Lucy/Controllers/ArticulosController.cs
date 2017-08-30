using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelCL;
using System.Web.Security;
using System.Data;
using System.Data.Entity;
using System.Net;
using System.IO;

        //public ActionResult List()
        //{

        //    List<ModelCL.Articulo> Articulos = null;
        //    Articulos = db.Articulo.ToList();
        //    return View(Articulos);

        //}




namespace Lucy.Controllers
{
    [RoutePrefix("articulos")]
    public class ArticulosController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        [Route("index")]
        public ActionResult Index()
        {
            var articulos = db.Contenido.Where(c => c.Articulo != null).ToList();

            return View(articulos);
        }

        [Route("details")]
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

        //[Route("create")]
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[Route("create")]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(ModelCL.Contenido contenido, HttpPostedFileBase[] files)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        short cont = 0;
        //        foreach (var file in files)
        //        {
        //            if (file != null)
        //            {
        //                cont += 1;

        //                if (!Functions.isValidContentType(file.ContentType))
        //                {
        //                    ViewBag.Error = "Solo se aceptan formatos de archivos JPG, JPEG, PNG y GIF.";
        //                    return View();
        //                }
        //                else if (!Functions.isValidContentLength(file.ContentLength))
        //                {
        //                    ViewBag.Error = "El archivo es muy pesado.";
        //                    return View();
        //                }
        //                else
        //                {

        //                    if (file.ContentLength > 0)
        //                    {
        //                        //var fileName = Path.GetFileName(file.FileName);
        //                        string tipoArchivo = "";
        //                        if (file.ContentType.Split('/')[0] == "image")
        //                        {
        //                            tipoArchivo = "Imagenes";
        //                        }
        //                        else if (file.ContentType.Split('/')[0] == "video")
        //                        {
        //                            tipoArchivo = "Videos";
        //                        }
        //                        else
        //                        {
        //                            return View(); //Error inesperado
        //                        }

        //                        string nombreArchivo = Guid.NewGuid().ToString() + "." + file.ContentType.Split('/')[1];
        //                        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Lucy/Resources/Oficial", tipoArchivo, "Articulos", nombreArchivo);

        //                        ModelCL.Multimedia m = new ModelCL.Multimedia();
        //                        m.MultimediaUrl = "Resources/Oficial/" + tipoArchivo + "/Articulos/" + nombreArchivo;
        //                        m.MultimediaTipo = file.ContentType.Split('/')[0];
        //                        m.MultimediaOrden = cont;

        //                        contenido.Multimedia.Add(m);

        //                        file.SaveAs(path);
        //                    }
        //                }
        //            }
        //        }

        //        ModelCL.Articulo newArticulo = new ModelCL.Articulo();
        //        contenido.Articulo = newArticulo;
        //        db.Contenido.Add(contenido);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(contenido);
        //}

        //[Route("edit")]
        //public ActionResult Edit(long? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    ModelCL.Contenido contArticulo = db.Contenido.Find(id);
        //    if (contArticulo.Articulo == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(contArticulo);
        //}

        //[HttpPost]
        //[Route("edit")]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(ModelCL.Contenido contenido, HttpPostedFileBase[] files)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        ModelCL.Contenido oldContenido = db.Contenido.Find(contenido.ContenidoId);
        //        oldContenido.ContenidoTitulo = contenido.ContenidoTitulo;
        //        oldContenido.ContenidoDescripcion = contenido.ContenidoDescripcion;
        //        oldContenido.ContenidoCuerpo = contenido.ContenidoCuerpo;

        //        short cont = 0;
        //        foreach (var file in files)
        //        {
        //            cont += 1;

        //            if (file != null)
        //            {
        //                if (!Functions.isValidContentType(file.ContentType))
        //                {
        //                    ViewBag.Error = "Solo se aceptan formatos de archivos JPG, JPEG, PNG y GIF.";
        //                    return View();
        //                }
        //                else if (!Functions.isValidContentLength(file.ContentLength))
        //                {
        //                    ViewBag.Error = "El archivo es muy pesado.";
        //                    return View();
        //                }
        //                else
        //                {
        //                    if (file.ContentLength > 0)
        //                    {
        //                        //var fileName = Path.GetFileName(file.FileName);
        //                        string tipoArchivo = "";
        //                        if (file.ContentType.Split('/')[0] == "image")
        //                        {
        //                            tipoArchivo = "Imagenes";
        //                        }
        //                        else if (file.ContentType.Split('/')[0] == "video")
        //                        {
        //                            tipoArchivo = "Videos";
        //                        }
        //                        else
        //                        {
        //                            return View(); //Error inesperado
        //                        }

        //                        ModelCL.Multimedia oldMult = oldContenido.Multimedia.Where(m => m.MultimediaOrden == cont).FirstOrDefault();

        //                        string nombreArchivo = Guid.NewGuid().ToString() + "." + file.ContentType.Split('/')[1];
        //                        var newPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Lucy/Resources/Oficial", tipoArchivo, "Articulos", nombreArchivo);

        //                        string newUrl = "Resources/Oficial/" + tipoArchivo + "/Articulos/" + nombreArchivo;

        //                        if (oldMult != null)
        //                        {
        //                            var oldPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Lucy/", oldMult.MultimediaUrl);
        //                            if (System.IO.File.Exists(oldPath))
        //                                System.IO.File.Delete(oldPath);

        //                            //oldContenido.Multimedia.Remove(oldMult);
        //                            oldMult.MultimediaUrl = newUrl;
        //                            oldMult.MultimediaTipo = file.ContentType.Split('/')[0];
        //                        }
        //                        else
        //                        {
        //                            ModelCL.Multimedia newMult = new ModelCL.Multimedia();

        //                            newMult.MultimediaUrl = newUrl;

        //                            newMult.MultimediaTipo = file.ContentType.Split('/')[0];
        //                            newMult.MultimediaOrden = Convert.ToInt16(oldContenido.Multimedia.Count() + 1);

        //                            oldContenido.Multimedia.Add(newMult);
        //                        }

        //                        file.SaveAs(newPath);
        //                    }
        //                }
        //            }
        //        }

        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(contenido);
        //}

        //[Route("delete")]
        //public ActionResult Delete(long? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    ModelCL.Contenido contArticulo = db.Contenido.Find(id);
        //    if (contArticulo.Articulo == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(contArticulo);
        //}

        //[HttpPost, ActionName("Delete")]
        //[Route("delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(long id)
        //{
        //    ModelCL.Contenido contArticulo = db.Contenido.Find(id);
        //    foreach (ModelCL.Multimedia m in contArticulo.Multimedia.ToList())
        //    {
        //        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Lucy/", m.MultimediaUrl);
        //        if (System.IO.File.Exists(path))
        //            System.IO.File.Delete(path);
        //    }

        //    db.Contenido.Remove(contArticulo);
        //    db.SaveChanges();

        //    return RedirectToAction("Index");
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
