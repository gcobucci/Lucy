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
using PagedList;
using PagedList.Mvc;
using Lucy.Models;

namespace Lucy.Controllers
{
    [Authorize]
    [RoutePrefix("recetas")]
    public class RecetasController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        [Route("listado")]
        public ActionResult Index(int? page, string search, byte? enfermedad, int? calMax, int? calMin, int? carMax, int? carMin, byte? gluten, byte? sodio)
        {
            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);
            ViewBag.idUsu = idUsu;

            if (Fachada.Functions.es_premium(idUsu) == false)
            {
                TempData["PermisoDenegado"] = true;
                return RedirectToAction("Index", "Home");
            }

            IPagedList recetas = null;

            if ((gluten == null && sodio == null) || (gluten == 0 && sodio == 0) || (gluten == 0 && sodio == null) || (gluten == null && sodio == 0))
            {
                if (calMax != null || calMin != null || carMax != null || carMin != null)
                {
                    recetas = db.Contenido.Where(c => c.Receta != null && (c.UsuarioAutor == null || c.UsuarioAutor.UsuarioId == idUsu) &&
                    (c.ContenidoTitulo.Contains(search) || search == null) && (c.Receta.RecetaCalorias >= (calMin ?? 0))
                    && (c.Receta.RecetaCalorias <= (calMax ?? 1000000)) && (c.Receta.RecetaHidratos >= (carMin ?? 0))
                    && (c.Receta.RecetaHidratos <= (carMax ?? 1000000))).OrderBy(c => c.ContenidoTitulo).ToList().ToPagedList(page ?? 1, 10);
                }
                else
                {
                    recetas = db.Contenido.Where(c => c.Receta != null && (c.UsuarioAutor == null || c.UsuarioAutor.UsuarioId == idUsu) &&
                    (c.ContenidoTitulo.Contains(search) || search == null) && ((c.Receta.RecetaCalorias >= (calMin ?? 0)) || c.Receta.RecetaCalorias == null)
                    && ((c.Receta.RecetaCalorias <= (calMax ?? 1000000)) || c.Receta.RecetaCalorias == null) && ((c.Receta.RecetaHidratos >= (carMin ?? 0)) || c.Receta.RecetaHidratos == null)
                    && ((c.Receta.RecetaHidratos <= (carMax ?? 1000000)) || c.Receta.RecetaHidratos == null)).OrderBy(c => c.ContenidoTitulo).ToList().ToPagedList(page ?? 1, 10);
                }
            }
            else
            {
                if (gluten == 1 && sodio == 1)
                {
                    recetas = db.Contenido.Where(c => c.Receta != null && (c.UsuarioAutor == null || c.UsuarioAutor.UsuarioId == idUsu) &&
                    (c.ContenidoTitulo.Contains(search) || search == null) && (c.Receta.RecetaCalorias >= (calMin ?? 1)) && (c.Receta.RecetaCalorias <= (calMax ?? 1000000)) &&
                    (c.Receta.RecetaHidratos >= (carMin ?? 0)) && (c.Receta.RecetaHidratos <= (carMax ?? 1000000)) && (c.Receta.RecetaGluten == false) &&
                    (c.Receta.RecetaSodio == false)).OrderBy(c => c.ContenidoTitulo).ToList().ToPagedList(page ?? 1, 10);
                }
                else if (gluten == 1 && (sodio == 0 || sodio == null))
                {
                    recetas = db.Contenido.Where(c => c.Receta != null && (c.UsuarioAutor == null || c.UsuarioAutor.UsuarioId == idUsu) &&
                    (c.ContenidoTitulo.Contains(search) || search == null) && (c.Receta.RecetaCalorias >= (calMin ?? 1)) && (c.Receta.RecetaCalorias <= (calMax ?? 1000000)) &&
                    (c.Receta.RecetaHidratos >= (carMin ?? 0)) && (c.Receta.RecetaHidratos <= (carMax ?? 1000000)) && (c.Receta.RecetaGluten == false))
                    .OrderBy(c => c.ContenidoTitulo).ToList().ToPagedList(page ?? 1, 10);
                }
                else if ((gluten == 0 || gluten == null) && sodio == 1)
                {
                    recetas = db.Contenido.Where(c => c.Receta != null && (c.UsuarioAutor == null || c.UsuarioAutor.UsuarioId == idUsu) &&
                    (c.ContenidoTitulo.Contains(search) || search == null) && (c.Receta.RecetaCalorias >= (calMin ?? 1)) && (c.Receta.RecetaCalorias <= (calMax ?? 1000000)) &&
                    (c.Receta.RecetaHidratos >= (carMin ?? 0)) && (c.Receta.RecetaHidratos <= (carMax ?? 1000000)) && (c.Receta.RecetaSodio == false))
                    .OrderBy(c => c.ContenidoTitulo).ToList().ToPagedList(page ?? 1, 10);
                }
            }

            if (recetas.TotalItemCount < 1)
            {
                if (search != null)
                {
                    ViewBag.Message = "No encontramos recetas con ese nombre.";
                }
                else
                {
                    ViewBag.Message = "No encontramos recetas con estos filtros.";
                }
            }

            return View(recetas);
        }

        [Route("ver")]
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);

            ViewBag.idUsu = idUsu;


            if (Fachada.Functions.es_premium(idUsu) == false)
            {
                TempData["PermisoDenegado"] = true;
                return RedirectToAction("Index", "Home");
            }



            ModelCL.Contenido contReceta = db.Contenido.Find(id);
            if (contReceta == null || contReceta.Receta == null || (contReceta.UsuarioAutor != null && contReceta.UsuarioAutor.UsuarioId != idUsu))
            {
                return HttpNotFound();
            }

            contReceta.ContenidoCantVisitas += 1;
            db.SaveChanges();

            return View(contReceta);
        }

        [Route("crear")]
        public ActionResult Create()
        {
            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);

            if (Fachada.Functions.es_premium(idUsu) == false)
            {
                TempData["PermisoDenegado"] = true;
                return RedirectToAction("Index", "Home");
            }


            RecetaViewModel newReceta = new RecetaViewModel();

            return View(newReceta);
        }

        [HttpPost]
        [Route("crear")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RecetaViewModel datos, HttpPostedFileBase[] files)
        {
            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);

            if (Fachada.Functions.es_premium(idUsu) == false)
            {
                TempData["PermisoDenegado"] = true;
                return RedirectToAction("Index", "Home");
            }


            if (ModelState.IsValid)
            {
                if (files.Where(f => f != null).Count() == 0)
                {
                    ViewBag.ErrorMessage = "Las recetas deben tener al menos una imagen.";
                    return View(datos);
                }

                ModelCL.Contenido newContRec = new ModelCL.Contenido();
                newContRec.ContenidoTitulo = datos.ContenidoTitulo;
                newContRec.ContenidoDescripcion = datos.ContenidoDescripcion;
                newContRec.ContenidoCuerpo = datos.ContenidoCuerpo;

                ModelCL.Receta receta = new ModelCL.Receta();
                receta.RecetaCalorias = datos.RecetaCalorias;
                receta.RecetaHidratos = datos.RecetaHidratos;
                receta.RecetaSodio = datos.RecetaSodio;
                receta.RecetaGluten = datos.RecetaGluten;

                newContRec.Receta = receta;

                newContRec.UsuarioAutor = db.Usuario.Find(idUsu);


                short cont = 0;
                foreach (var file in files)
                {
                    if (file != null)
                    {
                        cont += 1;

                        if (!Fachada.Functions.isValidContentType(file.ContentType))
                        {
                            ViewBag.ErrorMessage = "Solo se aceptan formatos de archivos JPG, JPEG, PNG y GIF.";
                            return View(datos);
                        }
                        else if (!Fachada.Functions.isValidContentLength(file.ContentLength))
                        {
                            ViewBag.ErrorMessage = "Una o varias imágenes se pasan del tamaño máximo admitido.";
                            return View(datos);
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
                                    return View(datos); //Error inesperado
                                }

                                string nombreArchivo = Guid.NewGuid().ToString() + "." + file.ContentType.Split('/')[1];
                                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Lucy/Resources/Users", tipoArchivo, "Recetas", nombreArchivo);

                                ModelCL.Multimedia m = new ModelCL.Multimedia();
                                m.MultimediaUrl = "Resources/Users/" + tipoArchivo + "/Recetas/" + nombreArchivo; //Ver cual es mejor entre estos 2
                                m.MultimediaTipo = file.ContentType.Split('/')[0];
                                m.MultimediaOrden = cont;

                                newContRec.Multimedia.Add(m);

                                file.SaveAs(path);
                            }
                        }
                    }
                }


                db.Contenido.Add(newContRec);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ErrorMessage = "Error inesperado";
            return View(datos);
        }

        [Route("editar")]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);

            if (Fachada.Functions.es_premium(idUsu) == false)
            {
                TempData["PermisoDenegado"] = true;
                return RedirectToAction("Index", "Home");
            }

            ModelCL.Contenido oldContReceta = db.Contenido.Find(id);
            if (oldContReceta == null || oldContReceta.Receta == null)
            {
                return HttpNotFound();
            }

            RecetaViewModel receta = new RecetaViewModel();

            receta.ContenidoId = oldContReceta.ContenidoId;
            receta.ContenidoTitulo = oldContReceta.ContenidoTitulo;
            receta.ContenidoDescripcion = oldContReceta.ContenidoDescripcion;
            receta.ContenidoCuerpo = oldContReceta.ContenidoCuerpo;

            receta.RecetaCalorias = oldContReceta.Receta.RecetaCalorias;
            receta.RecetaHidratos = oldContReceta.Receta.RecetaHidratos;
            receta.RecetaSodio = oldContReceta.Receta.RecetaSodio;
            receta.RecetaGluten = oldContReceta.Receta.RecetaGluten;

            ModelCL.Multimedia m1 = oldContReceta.Multimedia.Where(m => m.MultimediaOrden == 1).FirstOrDefault();
            if (m1 != null)
            {
                receta.RecetaImagen1Id = m1.MultimediaId;
                receta.RecetaImagen1Url = m1.MultimediaUrl;
            }

            ModelCL.Multimedia m2 = oldContReceta.Multimedia.Where(m => m.MultimediaOrden == 2).FirstOrDefault();
            if (m2 != null)
            {
                receta.RecetaImagen2Id = m2.MultimediaId;
                receta.RecetaImagen2Url = m2.MultimediaUrl;
            }

            ModelCL.Multimedia m3 = oldContReceta.Multimedia.Where(m => m.MultimediaOrden == 3).FirstOrDefault();
            if (m3 != null)
            {
                receta.RecetaImagen3Id = m3.MultimediaId;
                receta.RecetaImagen3Url = m3.MultimediaUrl;
            }

            ModelCL.Multimedia m4 = oldContReceta.Multimedia.Where(m => m.MultimediaOrden == 4).FirstOrDefault();
            if (m4 != null)
            {
                receta.RecetaImagen4Id = m4.MultimediaId;
                receta.RecetaImagen4Url = m4.MultimediaUrl;
            }

            ModelCL.Multimedia m5 = oldContReceta.Multimedia.Where(m => m.MultimediaOrden == 5).FirstOrDefault();
            if (m5 != null)
            {
                receta.RecetaImagen5Id = m5.MultimediaId;
                receta.RecetaImagen5Url = m5.MultimediaUrl;
            }

            return View(receta);
        }

        [HttpPost]
        [Route("editar")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RecetaViewModel datos, HttpPostedFileBase[] files, int[] checkBorrar)
        {
            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);

            if (Fachada.Functions.es_premium(idUsu) == false)
            {
                TempData["PermisoDenegado"] = true;
                return RedirectToAction("Index", "Home");
            }


            if (ModelState.IsValid)
            {
                ModelCL.Contenido contReceta = db.Contenido.Find(datos.ContenidoId);

                contReceta.ContenidoTitulo = datos.ContenidoTitulo;
                contReceta.ContenidoDescripcion = datos.ContenidoDescripcion;
                contReceta.ContenidoCuerpo = datos.ContenidoCuerpo;

                contReceta.Receta.RecetaCalorias = datos.RecetaCalorias;
                contReceta.Receta.RecetaHidratos = datos.RecetaHidratos;
                contReceta.Receta.RecetaSodio = datos.RecetaSodio;
                contReceta.Receta.RecetaGluten = datos.RecetaGluten;


                short cont = 0;
                foreach (var file in files)
                {
                    cont += 1;

                    if (file != null)
                    {
                        if (!Fachada.Functions.isValidContentType(file.ContentType))
                        {
                            ViewBag.ErrorMessage = "Solo se aceptan formatos de archivos JPG, JPEG, PNG y GIF.";
                            return View(datos);
                        }
                        else if (!Fachada.Functions.isValidContentLength(file.ContentLength))
                        {
                            ViewBag.ErrorMessage = "Una o varias imágenes se pasan del tamaño máximo admitido.";
                            return View(datos);
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
                                    return View(datos); //Error inesperado
                                }

                                ModelCL.Multimedia oldMult = contReceta.Multimedia.Where(m => m.MultimediaOrden == cont).FirstOrDefault();

                                string nombreArchivo = Guid.NewGuid().ToString() + "." + file.ContentType.Split('/')[1];
                                var newPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Lucy/Resources/Users", tipoArchivo, "Recetas", nombreArchivo);

                                string newUrl = "Resources/Users/" + tipoArchivo + "/Recetas/" + nombreArchivo;

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
                                    newMult.MultimediaOrden = Convert.ToInt16(contReceta.Multimedia.Count() + 1);

                                    contReceta.Multimedia.Add(newMult);
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

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ErrorMessage = "Error inesperado";
            return View(datos);
        }

        [Route("eliminar")]
        //[ValidateAntiForgeryToken]
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModelCL.Contenido contenido = db.Contenido.Where(c => c.ContenidoId == id && c.Receta != null).FirstOrDefault();

            if (contenido == null)
            {
                return HttpNotFound();
            }

            db.Contenido.Remove(contenido);
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