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
using System.Web.Security;

namespace Lucy.Controllers
{
    [RoutePrefix("dietas")]
    public class DietasController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        [Route("index")]
        public ActionResult Index()
        {
            #region UsuarioId por cookie
            HttpCookie cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket usu = FormsAuthentication.Decrypt(cookie.Value);
            int idUsu = Convert.ToInt32(usu.Name);
            #endregion

            var dietas = db.Contenido.Where(c => c.Dieta != null && (c.UsuarioAutor == null || c.UsuarioAutor.UsuarioId == idUsu)).ToList();

            return View(dietas);
        }

        [Route("details")]
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModelCL.Contenido contDieta = db.Contenido.Find(id);
            if (contDieta.Dieta == null)
            {
                return HttpNotFound();
            }
            return View(contDieta);
        }

        //[Route("create")]
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[Route("create")]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(ModelCL.Contenido contenido, HttpPostedFileBase file)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (file != null)
        //        {
        //            if (!Fachada.Functions.isValidContentType(file.ContentType))
        //            {
        //                ViewBag.Error = "Solo se aceptan formatos de archivos JPG, JPEG, PNG y GIF.";
        //                return View();
        //            }
        //            else if (!Fachada.Functions.isValidContentLength(file.ContentLength))
        //            {
        //                ViewBag.Error = "El archivo es muy pesado.";
        //                return View();
        //            }
        //            else
        //            {
                            
        //                if (file.ContentLength > 0)
        //                {
        //                    //var fileName = Path.GetFileName(file.FileName);
        //                    string tipoArchivo = "";
        //                    if (file.ContentType.Split('/')[0] == "image")
        //                    {
        //                        tipoArchivo = "Imagenes";
        //                    }
        //                    else if (file.ContentType.Split('/')[0] == "video")
        //                    {
        //                        tipoArchivo = "Videos";
        //                    }
        //                    else
        //                    {
        //                        return View(); //Error inesperado
        //                    }

        //                    string nombreArchivo = Guid.NewGuid().ToString() + "." + file.ContentType.Split('/')[1];
        //                    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Lucy/Resources/Oficial", tipoArchivo, "Dietas", nombreArchivo);

        //                    ModelCL.Multimedia m = new ModelCL.Multimedia();
        //                    m.MultimediaUrl = "Resources/Oficial/" + tipoArchivo + "/Dietas/" + nombreArchivo;
        //                    m.MultimediaTipo = file.ContentType.Split('/')[0];
        //                    m.MultimediaOrden = 1;

        //                    contenido.Multimedia.Add(m);

        //                    file.SaveAs(path);
        //                }
        //            }
        //        }            

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

        //    ModelCL.Contenido contDieta = db.Contenido.Find(id);
        //    if (contDieta.Dieta == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    return View(contDieta);
        //}

        //[HttpPost]
        //[Route("edit")]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(ModelCL.Contenido contenido, HttpPostedFileBase file)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        ModelCL.Contenido oldContenido = db.Contenido.Find(contenido.ContenidoId);
        //        oldContenido.ContenidoTitulo = contenido.ContenidoTitulo;
        //        oldContenido.ContenidoDescripcion = contenido.ContenidoDescripcion;
        //        oldContenido.ContenidoCuerpo = contenido.ContenidoCuerpo;


        //        oldContenido.Dieta.DietaDesayunoCalorias = contenido.Dieta.DietaDesayunoCalorias;
        //        oldContenido.Dieta.DietaDesayunoDescripcion = contenido.Dieta.DietaDesayunoDescripcion;

        //        oldContenido.Dieta.DietaAlmuerzoCalorias = contenido.Dieta.DietaAlmuerzoCalorias;
        //        oldContenido.Dieta.DietaAlmuerzoDescripcion = contenido.Dieta.DietaAlmuerzoDescripcion;

        //        oldContenido.Dieta.DietaMeriendaCalorias= contenido.Dieta.DietaMeriendaCalorias;
        //        oldContenido.Dieta.DietaMeriendaDescripcion = contenido.Dieta.DietaMeriendaDescripcion;

        //        oldContenido.Dieta.DietaCenaCalorias = contenido.Dieta.DietaCenaCalorias;
        //        oldContenido.Dieta.DietaCenaDescripcion = contenido.Dieta.DietaCenaDescripcion;

        //        oldContenido.Dieta.DietaIngestasCalorias = contenido.Dieta.DietaIngestasCalorias;
        //        oldContenido.Dieta.DietaIngestasDescripcion = contenido.Dieta.DietaIngestasDescripcion;

        //        if (file != null)
        //        {                      
        //            if (!Fachada.Functions.isValidContentType(file.ContentType))
        //            {
        //                ViewBag.Error = "Solo se aceptan formatos de archivos JPG, JPEG, PNG y GIF.";
        //                return View();
        //            }
        //            else if (!Fachada.Functions.isValidContentLength(file.ContentLength))
        //            {
        //                ViewBag.Error = "El archivo es muy pesado.";
        //                return View();
        //            }
        //            else
        //            {
        //                if (file.ContentLength > 0)
        //                {
        //                    //var fileName = Path.GetFileName(file.FileName);
        //                    string tipoArchivo = "";
        //                    if (file.ContentType.Split('/')[0] == "image")
        //                    {
        //                        tipoArchivo = "Imagenes";
        //                    }
        //                    else if (file.ContentType.Split('/')[0] == "video")
        //                    {
        //                        tipoArchivo = "Videos";
        //                    }
        //                    else
        //                    {
        //                        return View(); //Error inesperado
        //                    }

        //                    ModelCL.Multimedia oldMult = oldContenido.Multimedia.Where(m => m.MultimediaOrden == 1).FirstOrDefault();

        //                    string nombreArchivo = Guid.NewGuid().ToString() + "." + file.ContentType.Split('/')[1];
        //                    var newPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Lucy/Resources/Oficial", tipoArchivo, "Dietas", nombreArchivo);

        //                    string newUrl = "Resources/Oficial/" + tipoArchivo + "/Dietas/" + nombreArchivo;

        //                    var oldPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Lucy/", oldMult.MultimediaUrl);
        //                    if (System.IO.File.Exists(oldPath))
        //                        System.IO.File.Delete(oldPath);

        //                    //oldContenido.Multimedia.Remove(oldMult);
        //                    oldMult.MultimediaUrl = newUrl;
        //                    oldMult.MultimediaTipo = file.ContentType.Split('/')[0];                           

        //                    file.SaveAs(newPath);
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
        //    ModelCL.Contenido contDieta = db.Contenido.Find(id);
        //    if (contDieta.Dieta == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(contDieta);
        //}

        //[HttpPost, ActionName("Delete")]
        //[Route("delete")]        
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(long id)
        //{
        //    ModelCL.Contenido contDieta = db.Contenido.Find(id);

        //    //Solo hay una imagen para las dietas actualmente pero es lo mismo
        //    foreach (ModelCL.Multimedia m in contDieta.Multimedia.ToList())
        //    {
        //        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Lucy/", m.MultimediaUrl);
        //        if (System.IO.File.Exists(path))
        //            System.IO.File.Delete(path);
        //    }

        //    db.Contenido.Remove(contDieta);     
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
