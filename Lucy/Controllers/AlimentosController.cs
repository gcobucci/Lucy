using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelCL;
using System.Web.Security;
using System.Net;
using System.IO;
using Lucy.Models;

namespace Lucy.Controllers
{
    [RoutePrefix("alimentos")]
    public class AlimentosController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        [Route("listado")]
        public ActionResult Index()
        {
            List<ModelCL.Alimento> alimentos = null;

            if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
            {
                long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);
                ViewBag.idUsu = idUsu;

                alimentos = db.Alimento.Where(a => a.Usuario == null || a.Usuario.UsuarioId == idUsu).ToList();
            }
            else
            {
                alimentos = db.Alimento.Where(a => a.Usuario == null).ToList();
            }

            return View(alimentos);
        }

        [Authorize]
        [Route("crear")]
        public ActionResult Create()
        {
            AlimentoViewModel newAlimento = new AlimentoViewModel();
            
            return View(newAlimento);
        }

        [Authorize]
        [HttpPost]
        [Route("crear")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AlimentoViewModel datos, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                ModelCL.Alimento newAli = new ModelCL.Alimento();
                newAli.AlimentoNombre = datos.AlimentoNombre;
                newAli.AlimentoPorcion = datos.AlimentoPorcion;
                newAli.AlimentoCalorias = datos.AlimentoCalorias;
                newAli.AlimentoCarbohidratos = datos.AlimentoCarbohidratos;
                newAli.AlimentoAzucar = datos.AlimentoAzucar;
                newAli.AlimentoGrasa = datos.AlimentoGrasa;
                newAli.AlimentoSodio = datos.AlimentoSodio;
                newAli.AlimentoGluten = datos.AlimentoGluten;

                long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);
                newAli.Usuario = db.Usuario.Find(idUsu);
                               
                
                if (file != null)
                {
                    if (!Fachada.Functions.isValidContentType(file.ContentType))
                    {
                        ViewBag.ErrorMessage = "Solo se aceptan formatos de archivos JPG, JPEG, PNG y GIF.";
                        return View(datos);
                    }
                    else if (!Fachada.Functions.isValidContentLength(file.ContentLength))
                    {
                        ViewBag.ErrorMessage = "El archivo es muy pesado.";
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
                            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Lucy/Resources/Oficial", tipoArchivo, "Alimentos", nombreArchivo);

                            newAli.AlimentoImagen = "Resources/Oficial/" + tipoArchivo + "/Alimentos/" + nombreArchivo;

                            file.SaveAs(path);
                        }
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "La imagen del alimento es obligatoria.";
                    return View(datos);
                }

                db.Alimento.Add(newAli);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ErrorMessage = "Error inesperado";
            return View(datos);
        }

        [Authorize]
        [Route("editar")]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ModelCL.Alimento oldAlimento = db.Alimento.Find(id);
            if (oldAlimento == null)
            {
                return HttpNotFound();
            }

            AlimentoViewModel alimento = new AlimentoViewModel();

            alimento.AlimentoId = oldAlimento.AlimentoId;
            alimento.AlimentoNombre = oldAlimento.AlimentoNombre;
            alimento.AlimentoImagen = oldAlimento.AlimentoImagen;
            alimento.AlimentoPorcion = oldAlimento.AlimentoPorcion;
            alimento.AlimentoCalorias = oldAlimento.AlimentoCarbohidratos;
            alimento.AlimentoCarbohidratos = oldAlimento.AlimentoCarbohidratos;
            alimento.AlimentoGrasa = oldAlimento.AlimentoGrasa;
            alimento.AlimentoAzucar = oldAlimento.AlimentoAzucar;
            alimento.AlimentoSodio = oldAlimento.AlimentoSodio;
            alimento.AlimentoGluten = oldAlimento.AlimentoGluten;

            return View(alimento);
        }

        [Authorize]
        [HttpPost]
        [Route("editar")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AlimentoViewModel datos, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                ModelCL.Alimento alimento = db.Alimento.Find(datos.AlimentoId);

                alimento.AlimentoNombre = datos.AlimentoNombre;
                alimento.AlimentoPorcion = datos.AlimentoPorcion;
                alimento.AlimentoCalorias = datos.AlimentoCalorias;
                alimento.AlimentoCarbohidratos = datos.AlimentoCarbohidratos;
                alimento.AlimentoAzucar = datos.AlimentoAzucar;
                alimento.AlimentoGrasa = datos.AlimentoGrasa;
                alimento.AlimentoSodio = datos.AlimentoSodio;
                alimento.AlimentoGluten = datos.AlimentoGluten;

                if (file != null)
                {
                    if (!Fachada.Functions.isValidContentType(file.ContentType))
                    {
                        ViewBag.ErrorMessage = "Solo se aceptan formatos de archivos JPG, JPEG, PNG y GIF.";
                        return View(datos);
                    }
                    else if (!Fachada.Functions.isValidContentLength(file.ContentLength))
                    {
                        ViewBag.ErrorMessage = "El archivo es muy pesado.";
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
                            var newPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Lucy/Resources/Oficial", tipoArchivo, "Alimentos", nombreArchivo);

                            string newUrl = "Resources/Oficial/" + tipoArchivo + "/Alimentos/" + nombreArchivo;

                            var oldPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Lucy/", alimento.AlimentoImagen);
                            if (System.IO.File.Exists(oldPath))
                                System.IO.File.Delete(oldPath);

                            alimento.AlimentoImagen = newUrl;

                            file.SaveAs(newPath);
                        }
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ErrorMessage = "Error inesperado";
            return View(datos);
        }

        [Authorize]
        [Route("eliminar")]
        //[ValidateAntiForgeryToken]
        public ActionResult Delete(long? id/*, string url*/)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModelCL.Alimento alimento = db.Alimento.Where(r => r.AlimentoId == id).FirstOrDefault();

            if (alimento == null)
            {
                return HttpNotFound();
            }

            db.Alimento.Remove(alimento);
            db.SaveChanges();
            //return Redirect(url);
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