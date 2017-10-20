using System;
using PagedList;
using PagedList.Mvc;
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

namespace Backend.Controllers
{
    [Authorize]
    [RoutePrefix("ejercicios")]
    public class EjerciciosController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        [Route("listado")]
        public ActionResult Index(int? page, string search, int? calMax, int? calMin ,byte? categoria, byte? tipo)
        {
            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);
            string cat = null;
            string tip = null;
            IPagedList ejercicios = null;

            if ((categoria == null && tipo == null) || (categoria == 0 && tipo == 0))
            {
                ejercicios = db.Contenido.Where(c => c.Ejercicio != null && (c.UsuarioAutor == null || c.UsuarioAutor.UsuarioId == idUsu) &&
                    (c.ContenidoTitulo.Contains(search) || search == null) && (c.Ejercicio.EjercicioCaloriasPorMinuto >= (calMin ?? 1)) && (c.Ejercicio.EjercicioCaloriasPorMinuto <= (calMax ?? 1000000))).ToList().ToPagedList(page ?? 1, 10);
            }
            else
            {
                // Identifico la categoría seleccionada
                if (categoria != null && categoria != 0)
                {
                    switch (categoria)
                    {
                        case 1:
                            cat = "Cuerpo completo";
                            break;
                        case 2:
                            cat = "Tren superior";
                            break;
                        case 3:
                            cat = "Tren inferior";
                            break;
                        case 4:
                            cat = "Abdominales";
                            break;
                        case 5:
                            cat = "Calentamiento";
                            break;
                        case 6:
                            cat = "Estiramiento";
                            break;
                        default:
                            cat = null;
                            break;
                    }
                }

                // Identifico el tipo seleccionado
                if (tipo != null && tipo != 0)
                {
                    switch (tipo)
                    {
                        case 1:
                            tip = "Actividad";
                            break;
                        case 2:
                            tip = "Ejercicio";
                            break;
                        default:
                            tip = null;
                            break;
                    }
                }

                ejercicios = db.Contenido.Where(c => c.Ejercicio != null && (c.UsuarioAutor == null || c.UsuarioAutor.UsuarioId == idUsu) &&
                    (c.ContenidoTitulo.Contains(search) || search == null) && (c.Ejercicio.EjercicioCaloriasPorMinuto >= (calMin ?? 1)) && 
                    (c.Ejercicio.EjercicioCaloriasPorMinuto <= (calMax ?? 1000000)) && (c.Ejercicio.EjercicioCategoria.Contains(cat) || cat == null) && 
                    (c.Ejercicio.EjercicioTipo.Contains(tip) || tip == null)).ToList().ToPagedList(page ?? 1, 10);
            }

            // Valido si esta vacio
            if (ejercicios.TotalItemCount < 1)
            {
                if (search != null)
                {
                    ViewBag.Message = "No encontramos ejercicios con ese nombre.";
                }
                else
                {
                    ViewBag.Message = "No encontramos ejercicios con estos filtros.";
                }
            }

            ViewBag.idUsu = idUsu;
            return View(ejercicios);
        }

        [Route("ver")]
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);

            ModelCL.Contenido contEjercicio = db.Contenido.Find(id);
            if (contEjercicio.Ejercicio == null || (contEjercicio.UsuarioAutor != null && contEjercicio.UsuarioAutor.UsuarioId != idUsu))
            {
                return HttpNotFound();
            }
            return View(contEjercicio);
        }

        //[Route("crear")]
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[Route("crear")]
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

        //                if (!Fachada.Functions.isValidContentType(file.ContentType))
        //                {
        //                    ViewBag.Error = "Solo se aceptan formatos de archivos JPG, JPEG, PNG y GIF.";
        //                    return View();
        //                }
        //                else if (!Fachada.Functions.isValidContentLength(file.ContentLength))
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
        //                        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Lucy/Resources/Oficial", tipoArchivo, "Ejercicios", nombreArchivo);

        //                        ModelCL.Multimedia m = new ModelCL.Multimedia();
        //                        m.MultimediaUrl = "Resources/Oficial/" + tipoArchivo + "/Ejercicios/" + nombreArchivo;
        //                        m.MultimediaTipo = file.ContentType.Split('/')[0];
        //                        m.MultimediaOrden = cont;

        //                        contenido.Multimedia.Add(m);

        //                        file.SaveAs(path);
        //                    }
        //                }
        //            }                    
        //        }

        //        db.Contenido.Add(contenido);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(contenido);
        //}

        //[Route("editar")]
        //public ActionResult Edit(long? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    ModelCL.Contenido contEjercicio = db.Contenido.Find(id);
        //    if (contEjercicio.Ejercicio == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    return View(contEjercicio);
        //}

        //[HttpPost]
        //[Route("editar")]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(ModelCL.Contenido contenido, HttpPostedFileBase[] files)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        ModelCL.Contenido oldContenido = db.Contenido.Find(contenido.ContenidoId);
        //        oldContenido.ContenidoTitulo = contenido.ContenidoTitulo;
        //        oldContenido.ContenidoDescripcion = contenido.ContenidoDescripcion;
        //        oldContenido.ContenidoCuerpo = contenido.ContenidoCuerpo;

        //        oldContenido.Ejercicio.EjercicioTipo = contenido.Ejercicio.EjercicioTipo;
        //        oldContenido.Ejercicio.EjercicioCategoria = contenido.Ejercicio.EjercicioCategoria;

        //        short cont = 0;
        //        foreach (var file in files)
        //        {
        //            cont += 1;

        //            if (file != null)
        //            {                      
        //                if (!Fachada.Functions.isValidContentType(file.ContentType))
        //                {
        //                    ViewBag.Error = "Solo se aceptan formatos de archivos JPG, JPEG, PNG y GIF.";
        //                    return View();
        //                }
        //                else if (!Fachada.Functions.isValidContentLength(file.ContentLength))
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
        //                        var newPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Lucy/Resources/Oficial", tipoArchivo, "Ejercicios", nombreArchivo);

        //                        string newUrl = "Resources/Oficial/" + tipoArchivo + "/Ejercicios/" + nombreArchivo;

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

        //[Route("eliminar")]
        //public ActionResult Delete(long? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    ModelCL.Contenido contEjercicio = db.Contenido.Find(id);
        //    if (contEjercicio.Ejercicio == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(contEjercicio);
        //}

        //[HttpPost, ActionName("Delete")]
        //[Route("eliminar")]        
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(long id)
        //{
        //    ModelCL.Contenido contEjercicio = db.Contenido.Find(id);
        //    foreach (ModelCL.Multimedia m in contEjercicio.Multimedia.ToList())
        //    {
        //        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Lucy/", m.MultimediaUrl);
        //        if (System.IO.File.Exists(path))
        //            System.IO.File.Delete(path);
        //    }

        //    db.Contenido.Remove(contEjercicio);     
        //    db.SaveChanges();

        //    return RedirectToAction("Index");
        //}

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
