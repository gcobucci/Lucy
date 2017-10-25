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
    [RoutePrefix("ejercicios")]
    public class EjerciciosController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        [Route("listado")]
        public ActionResult Index()
        {
            var ejercicios = db.Contenido.Where(c => c.Ejercicio != null && c.UsuarioAutor == null).ToList();

            return View(ejercicios);
        }

        [Route("ver")]
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModelCL.Contenido contEjercicio = db.Contenido.Find(id);
            if (contEjercicio.Ejercicio == null)
            {
                return HttpNotFound();
            }
            return View(contEjercicio);
        }

        [Route("crear")]
        public ActionResult Create()
        {
            List<Fachada.ViewModelSelectListChk> lEjTipos = new List<Fachada.ViewModelSelectListChk>()
            {
                new Fachada.ViewModelSelectListChk { Id = "Ejercicio", Valor = "Ejercicio" },
                new Fachada.ViewModelSelectListChk { Id = "Actividad", Valor = "Actividad" },
            };
            ViewBag.lEjTipos = new SelectList(lEjTipos, "Id", "Valor");

            List<Fachada.ViewModelSelectListChk> lEjCategorias = new List<Fachada.ViewModelSelectListChk>()
            {
                new Fachada.ViewModelSelectListChk { Id = "Cuerpo completo", Valor = "Cuerpo completo" },
                new Fachada.ViewModelSelectListChk { Id = "Tren superior", Valor = "Tren superior" },
                new Fachada.ViewModelSelectListChk { Id = "Abdominales", Valor = "Abdominales" },
                new Fachada.ViewModelSelectListChk { Id = "Calentamiento", Valor = "Calentamiento" },
                new Fachada.ViewModelSelectListChk { Id = "Estiramiento", Valor = "Estiramiento" },
            };
            ViewBag.lEjCategorias = new SelectList(lEjCategorias, "Id", "Valor");

            return View();
        }

        [HttpPost]
        [Route("crear")]
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
                                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Lucy/Resources/Oficial", tipoArchivo, "Ejercicios", nombreArchivo);

                                ModelCL.Multimedia m = new ModelCL.Multimedia();
                                m.MultimediaUrl = "Resources/Oficial/" + tipoArchivo + "/Ejercicios/" + nombreArchivo;
                                m.MultimediaTipo = file.ContentType.Split('/')[0];
                                m.MultimediaOrden = cont;

                                contenido.Multimedia.Add(m);

                                file.SaveAs(path);
                            }
                        }
                    }                    
                }

                db.Contenido.Add(contenido);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            List<Fachada.ViewModelSelectListChk> lEjTipos = new List<Fachada.ViewModelSelectListChk>()
            {
                new Fachada.ViewModelSelectListChk { Id = "Ejercicio", Valor = "Ejercicio" },
                new Fachada.ViewModelSelectListChk { Id = "Actividad", Valor = "Actividad" },
            };
            ViewBag.lEjTipos = new SelectList(lEjTipos, "Id", "Valor");

            List<Fachada.ViewModelSelectListChk> lEjCategorias = new List<Fachada.ViewModelSelectListChk>()
            {
                new Fachada.ViewModelSelectListChk { Id = "Cuerpo completo", Valor = "Cuerpo completo" },
                new Fachada.ViewModelSelectListChk { Id = "Tren superior", Valor = "Tren superior" },
                new Fachada.ViewModelSelectListChk { Id = "Abdominales", Valor = "Abdominales" },
                new Fachada.ViewModelSelectListChk { Id = "Calentamiento", Valor = "Calentamiento" },
                new Fachada.ViewModelSelectListChk { Id = "Estiramiento", Valor = "Estiramiento" },
            };
            ViewBag.lEjCategorias = new SelectList(lEjCategorias, "Id", "Valor");

            return View(contenido);
        }

        [Route("editar")]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ModelCL.Contenido contEjercicio = db.Contenido.Find(id);
            if (contEjercicio.Ejercicio == null)
            {
                return HttpNotFound();
            }

            List<Fachada.ViewModelSelectListChk> lEjTipos = new List<Fachada.ViewModelSelectListChk>()
            {
                new Fachada.ViewModelSelectListChk { Id = "Ejercicio", Valor = "Ejercicio" },
                new Fachada.ViewModelSelectListChk { Id = "Actividad", Valor = "Actividad" },
            };
            ViewBag.lEjTipos = new SelectList(lEjTipos, "Id", "Valor");

            List<Fachada.ViewModelSelectListChk> lEjCategorias = new List<Fachada.ViewModelSelectListChk>()
            {
                new Fachada.ViewModelSelectListChk { Id = "Cuerpo completo", Valor = "Cuerpo completo" },
                new Fachada.ViewModelSelectListChk { Id = "Tren superior", Valor = "Tren superior" },
                new Fachada.ViewModelSelectListChk { Id = "Abdominales", Valor = "Abdominales" },
                new Fachada.ViewModelSelectListChk { Id = "Calentamiento", Valor = "Calentamiento" },
                new Fachada.ViewModelSelectListChk { Id = "Estiramiento", Valor = "Estiramiento" },
            };
            ViewBag.lEjCategorias = new SelectList(lEjCategorias, "Id", "Valor");

            return View(contEjercicio);
        }

        [HttpPost]
        [Route("editar")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ModelCL.Contenido contenido, HttpPostedFileBase[] files, int[] checkBorrar)
        {
            if (ModelState.IsValid)
            {
                ModelCL.Contenido oldContenido = db.Contenido.Find(contenido.ContenidoId);
                oldContenido.ContenidoTitulo = contenido.ContenidoTitulo;
                oldContenido.ContenidoDescripcion = contenido.ContenidoDescripcion;
                oldContenido.ContenidoCuerpo = contenido.ContenidoCuerpo;

                oldContenido.Ejercicio.EjercicioTipo = contenido.Ejercicio.EjercicioTipo;
                oldContenido.Ejercicio.EjercicioCategoria = contenido.Ejercicio.EjercicioCategoria;
                oldContenido.Ejercicio.EjercicioCaloriasPorMinuto = contenido.Ejercicio.EjercicioCaloriasPorMinuto;

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
                                var newPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Lucy/Resources/Oficial", tipoArchivo, "Ejercicios", nombreArchivo);

                                string newUrl = "Resources/Oficial/" + tipoArchivo + "/Ejercicios/" + nombreArchivo;

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

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            List<Fachada.ViewModelSelectListChk> lEjTipos = new List<Fachada.ViewModelSelectListChk>()
            {
                new Fachada.ViewModelSelectListChk { Id = "Ejercicio", Valor = "Ejercicio" },
                new Fachada.ViewModelSelectListChk { Id = "Actividad", Valor = "Actividad" },
            };
            ViewBag.lEjTipos = new SelectList(lEjTipos, "Id", "Valor");

            List<Fachada.ViewModelSelectListChk> lEjCategorias = new List<Fachada.ViewModelSelectListChk>()
            {
                new Fachada.ViewModelSelectListChk { Id = "Cuerpo completo", Valor = "Cuerpo completo" },
                new Fachada.ViewModelSelectListChk { Id = "Tren superior", Valor = "Tren superior" },
                new Fachada.ViewModelSelectListChk { Id = "Abdominales", Valor = "Abdominales" },
                new Fachada.ViewModelSelectListChk { Id = "Calentamiento", Valor = "Calentamiento" },
                new Fachada.ViewModelSelectListChk { Id = "Estiramiento", Valor = "Estiramiento" },
            };
            ViewBag.lEjCategorias = new SelectList(lEjCategorias, "Id", "Valor");

            return View(contenido);
        }

        [Route("eliminar")]
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModelCL.Contenido contEjercicio = db.Contenido.Find(id);
            if (contEjercicio.Ejercicio == null)
            {
                return HttpNotFound();
            }
            return View(contEjercicio);
        }

        [HttpPost, ActionName("Delete")]
        [Route("eliminar")]        
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ModelCL.Contenido contEjercicio = db.Contenido.Find(id);
            foreach (ModelCL.Multimedia m in contEjercicio.Multimedia.ToList())
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Lucy/", m.MultimediaUrl);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
            }

            db.Contenido.Remove(contEjercicio);     
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
