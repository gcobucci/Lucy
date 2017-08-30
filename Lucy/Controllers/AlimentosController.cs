using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelCL;
using System.Web.Security;
using System.Net;

namespace Lucy.Controllers
{
    [RoutePrefix("alimentos")]
    public class AlimentosController : Controller
    {
        //    // GET: InfoNutricional
        //    [AllowAnonymous]
        //    [HttpGet]
        //    public ActionResult InfNut()
        //    {
        //        List<ModelCL.Alimento> Alimentos = null;

        //        #region UsuarioId por cookie
        //        HttpCookie cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
        //        FormsAuthenticationTicket usu = FormsAuthentication.Decrypt(cookie.Value);
        //        int idUsu = Convert.ToInt32(usu.Name);
        //        #endregion

        //        using (AgustinaEntities db = new AgustinaEntities())
        //        {

        //            if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
        //            {
        //                Alimentos = db.Alimento.Where(a => a.Usuario == null || a.Usuario.UsuarioId == idUsu).ToList();
        //            }
        //            else
        //            {
        //                Alimentos = db.Alimento.Where(a => a.Usuario == null).ToList();
        //            }

        //        }

        //        return View(Alimentos);
        //    }

        private AgustinaEntities db = new AgustinaEntities();

        [Route("informacion_nutricional")]
        public ActionResult Index()
        {
            List<ModelCL.Alimento> alimentos = null;

            if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
            {
                #region UsuarioId por cookie
                HttpCookie cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                FormsAuthenticationTicket usu = FormsAuthentication.Decrypt(cookie.Value);
                int idUsu = Convert.ToInt32(usu.Name);
                #endregion

                alimentos = db.Alimento.Where(a => a.Usuario == null || a.Usuario.UsuarioId == idUsu).ToList();
            }
            else
            {
                alimentos = db.Alimento.Where(a => a.Usuario == null).ToList();
            }

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

        //[Route("create")]
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[Route("create")]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(ModelCL.Alimento alimento, HttpPostedFileBase file)
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
        //                    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Lucy/Resources/Oficial", tipoArchivo, "Alimentos", nombreArchivo);

        //                    alimento.AlimentoImagen = "Resources/Oficial/" + tipoArchivo + "/Alimentos/" + nombreArchivo;

        //                    file.SaveAs(path);
        //                }
        //            }
        //        }

        //        db.Alimento.Add(alimento);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(alimento);
        //}

        //[Route("edit")]
        //public ActionResult Edit(long? id)
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

        //[HttpPost]
        //[Route("edit")]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(ModelCL.Alimento alimento, HttpPostedFileBase file)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        ModelCL.Alimento newAli = db.Alimento.Find(alimento.AlimentoId);

        //        newAli.AlimentoNombre = alimento.AlimentoNombre;
        //        newAli.AlimentoPorcion = alimento.AlimentoPorcion;
        //        newAli.AlimentoCalorias = alimento.AlimentoCalorias;
        //        newAli.AlimentoCarbohidratos = alimento.AlimentoCarbohidratos;
        //        newAli.AlimentoAzucar = alimento.AlimentoAzucar;
        //        newAli.AlimentoGrasa = alimento.AlimentoGrasa;
        //        newAli.AlimentoSodio = alimento.AlimentoSodio;
        //        newAli.AlimentoGluten = alimento.AlimentoGluten;

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
        //                    var newPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Lucy/Resources/Oficial", tipoArchivo, "Alimentos", nombreArchivo);

        //                    string newUrl = "Resources/Oficial/" + tipoArchivo + "/Alimentos/" + nombreArchivo;

        //                    var oldPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Lucy/", newAli.AlimentoImagen);
        //                    if (System.IO.File.Exists(oldPath))
        //                        System.IO.File.Delete(oldPath);

        //                    newAli.AlimentoImagen = newUrl;

        //                    file.SaveAs(newPath);
        //                }
        //            }
        //        }

        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(alimento);
        //}

        //[Route("delete")]
        //public ActionResult Delete(long? id)
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

        //[HttpPost, ActionName("Delete")]
        //[Route("delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(long id)
        //{
        //    ModelCL.Alimento alimento = db.Alimento.Find(id);

        //    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Lucy/", alimento.AlimentoImagen);
        //    if (System.IO.File.Exists(path))
        //        System.IO.File.Delete(path);

        //    db.Alimento.Remove(alimento);
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