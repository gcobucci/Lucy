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
    [RoutePrefix("alimentos")]
    public class AlimentosController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        [Route("index")]
        public ActionResult Index()
        {
            List<ModelCL.Alimento> alimentos = db.Alimento.ToList();

            return View(alimentos);
        }

        //[Route("details")]
        //public ActionResult Details(long? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    ModelCL.Alimento alimento = db.Alimento.Find(id);
        //    if (alimento == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(alimento);
        //}

        [Route("create")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Route("create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ModelCL.Alimento alimento, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    if (!Fachada.Functions.isValidContentType(file.ContentType))
                    {
                        ViewBag.Error = "Solo se aceptan formatos de archivos JPG, JPEG, PNG y GIF.";
                        return View();
                    }
                    else if (!Fachada.Functions.isValidContentLength(file.ContentLength))
                    {
                        ViewBag.Error = "El archivo es muy pesado.";
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
                            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Lucy/Resources/Oficial", tipoArchivo, "Alimentos", nombreArchivo);

                            alimento.AlimentoImagen = "Resources/Oficial/" + tipoArchivo + "/Alimentos/" + nombreArchivo;

                            file.SaveAs(path);
                        }
                    }
                }

                db.Alimento.Add(alimento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(alimento);
        }

        [Route("edit")]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ModelCL.Alimento alimento = db.Alimento.Find(id);
            if (alimento == null)
            {
                return HttpNotFound();
            }
            return View(alimento);
        }

        [HttpPost]
        [Route("edit")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ModelCL.Alimento alimento, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                ModelCL.Alimento newAli = db.Alimento.Find(alimento.AlimentoId);

                newAli.AlimentoNombre = alimento.AlimentoNombre;
                newAli.AlimentoPorcion = alimento.AlimentoPorcion;
                newAli.AlimentoCalorias = alimento.AlimentoCalorias;
                newAli.AlimentoCarbohidratos = alimento.AlimentoCarbohidratos;
                newAli.AlimentoAzucar = alimento.AlimentoAzucar;
                newAli.AlimentoGrasa = alimento.AlimentoGrasa;
                newAli.AlimentoSodio = alimento.AlimentoSodio;
                newAli.AlimentoGluten = alimento.AlimentoGluten;

                if (file != null)
                {
                    if (!Fachada.Functions.isValidContentType(file.ContentType))
                    {
                        ViewBag.Error = "Solo se aceptan formatos de archivos JPG, JPEG, PNG y GIF.";
                        return View();
                    }
                    else if (!Fachada.Functions.isValidContentLength(file.ContentLength))
                    {
                        ViewBag.Error = "El archivo es muy pesado.";
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
                            var newPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Lucy/Resources/Oficial", tipoArchivo, "Alimentos", nombreArchivo);

                            string newUrl = "Resources/Oficial/" + tipoArchivo + "/Alimentos/" + nombreArchivo;

                            var oldPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Lucy/", newAli.AlimentoImagen);
                            if (System.IO.File.Exists(oldPath))
                                System.IO.File.Delete(oldPath);

                            newAli.AlimentoImagen = newUrl;

                            file.SaveAs(newPath);
                        }
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(alimento);
        }

        [Route("delete")]
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ModelCL.Alimento alimento = db.Alimento.Find(id);
            if (alimento == null)
            {
                return HttpNotFound();
            }
            return View(alimento);
        }

        [HttpPost, ActionName("Delete")]
        [Route("delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ModelCL.Alimento alimento = db.Alimento.Find(id);

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Lucy/", alimento.AlimentoImagen);
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            db.Alimento.Remove(alimento);
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
