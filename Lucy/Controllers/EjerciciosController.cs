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
using Lucy.Models;

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


            if (Fachada.Functions.es_premium(idUsu) == false)
            {
                TempData["PermisoDenegado"] = true;
                return RedirectToAction("Index", "Home");
            }



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

            ViewBag.idUsu = idUsu;


            if (Fachada.Functions.es_premium(idUsu) == false)
            {
                TempData["PermisoDenegado"] = true;
                return RedirectToAction("Index", "Home");
            }



            ModelCL.Contenido contEjercicio = db.Contenido.Find(id);
            if (contEjercicio.Ejercicio == null || (contEjercicio.UsuarioAutor != null && contEjercicio.UsuarioAutor.UsuarioId != idUsu))
            {
                return HttpNotFound();
            }

            contEjercicio.ContenidoCantVisitas += 1;
            db.SaveChanges();

            return View(contEjercicio);
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
                new Fachada.ViewModelSelectListChk { Id = "Tren inferior", Valor = "Tren inferior" },
                new Fachada.ViewModelSelectListChk { Id = "Abdominales", Valor = "Abdominales" },
                new Fachada.ViewModelSelectListChk { Id = "Calentamiento", Valor = "Calentamiento" },
                new Fachada.ViewModelSelectListChk { Id = "Estiramiento", Valor = "Estiramiento" },
            };
            ViewBag.lEjCategorias = new SelectList(lEjCategorias, "Id", "Valor");


            EjercicioViewModel newEjercicio = new EjercicioViewModel();

            return View(newEjercicio);
        }

        [HttpPost]
        [Route("crear")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EjercicioViewModel datos, HttpPostedFileBase[] files)
        {
            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);

            if (Fachada.Functions.es_premium(idUsu) == false)
            {
                TempData["PermisoDenegado"] = true;
                return RedirectToAction("Index", "Home");
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
                new Fachada.ViewModelSelectListChk { Id = "Tren inferior", Valor = "Tren inferior" },
                new Fachada.ViewModelSelectListChk { Id = "Abdominales", Valor = "Abdominales" },
                new Fachada.ViewModelSelectListChk { Id = "Calentamiento", Valor = "Calentamiento" },
                new Fachada.ViewModelSelectListChk { Id = "Estiramiento", Valor = "Estiramiento" },
            };
            ViewBag.lEjCategorias = new SelectList(lEjCategorias, "Id", "Valor");


            if (ModelState.IsValid)
            {
                if (files.Where(f => f != null).Count() == 0)
                {
                    ViewBag.ErrorMessage = "Las ejercicios deben tener al menos una imagen.";
                    return View(datos);
                }

                ModelCL.Contenido newContEje = new ModelCL.Contenido();
                newContEje.ContenidoTitulo = datos.ContenidoTitulo;
                newContEje.ContenidoDescripcion = datos.ContenidoDescripcion;
                newContEje.ContenidoCuerpo = datos.ContenidoCuerpo;

                ModelCL.Ejercicio ejercicio = new ModelCL.Ejercicio();
                ejercicio.EjercicioTipo = datos.EjercicioTipo;
                ejercicio.EjercicioCategoria = datos.EjercicioCategoria;
                ejercicio.EjercicioCaloriasPorMinuto = datos.EjercicioCaloriasPorMinuto;

                newContEje.Ejercicio = ejercicio;

                newContEje.UsuarioAutor = db.Usuario.Find(idUsu);


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
                                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Lucy/Resources/Users", tipoArchivo, "Ejercicios", nombreArchivo);

                                ModelCL.Multimedia m = new ModelCL.Multimedia();
                                m.MultimediaUrl = "Resources/Users/" + tipoArchivo + "/Ejercicios/" + nombreArchivo; //Ver cual es mejor entre estos 2
                                m.MultimediaTipo = file.ContentType.Split('/')[0];
                                m.MultimediaOrden = cont;

                                newContEje.Multimedia.Add(m);

                                file.SaveAs(path);
                            }
                        }
                    }
                }


                db.Contenido.Add(newContEje);
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
                new Fachada.ViewModelSelectListChk { Id = "Tren inferior", Valor = "Tren inferior" },
                new Fachada.ViewModelSelectListChk { Id = "Abdominales", Valor = "Abdominales" },
                new Fachada.ViewModelSelectListChk { Id = "Calentamiento", Valor = "Calentamiento" },
                new Fachada.ViewModelSelectListChk { Id = "Estiramiento", Valor = "Estiramiento" },
            };
            ViewBag.lEjCategorias = new SelectList(lEjCategorias, "Id", "Valor");


            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);

            if (Fachada.Functions.es_premium(idUsu) == false)
            {
                TempData["PermisoDenegado"] = true;
                return RedirectToAction("Index", "Home");
            }

            ModelCL.Contenido oldContEjercicio = db.Contenido.Find(id);
            if (oldContEjercicio == null || oldContEjercicio.Ejercicio == null)
            {
                return HttpNotFound();
            }

            EjercicioViewModel ejercicio = new EjercicioViewModel();

            ejercicio.ContenidoId = oldContEjercicio.ContenidoId;
            ejercicio.ContenidoTitulo = oldContEjercicio.ContenidoTitulo;
            ejercicio.ContenidoDescripcion = oldContEjercicio.ContenidoDescripcion;
            ejercicio.ContenidoCuerpo = oldContEjercicio.ContenidoCuerpo;

            ejercicio.EjercicioTipo = oldContEjercicio.Ejercicio.EjercicioTipo;
            ejercicio.EjercicioCategoria = oldContEjercicio.Ejercicio.EjercicioCategoria;
            ejercicio.EjercicioCaloriasPorMinuto = oldContEjercicio.Ejercicio.EjercicioCaloriasPorMinuto;

            ModelCL.Multimedia m1 = oldContEjercicio.Multimedia.Where(m => m.MultimediaOrden == 1).FirstOrDefault();
            if (m1 != null)
            {
                ejercicio.EjercicioImagen1Id = m1.MultimediaId;
                ejercicio.EjercicioImagen1Url = m1.MultimediaUrl;
            }

            ModelCL.Multimedia m2 = oldContEjercicio.Multimedia.Where(m => m.MultimediaOrden == 2).FirstOrDefault();
            if (m2 != null)
            {
                ejercicio.EjercicioImagen2Id = m2.MultimediaId;
                ejercicio.EjercicioImagen2Url = m2.MultimediaUrl;
            }

            ModelCL.Multimedia m3 = oldContEjercicio.Multimedia.Where(m => m.MultimediaOrden == 3).FirstOrDefault();
            if (m3 != null)
            {
                ejercicio.EjercicioImagen3Id = m3.MultimediaId;
                ejercicio.EjercicioImagen3Url = m3.MultimediaUrl;
            }

            ModelCL.Multimedia m4 = oldContEjercicio.Multimedia.Where(m => m.MultimediaOrden == 4).FirstOrDefault();
            if (m4 != null)
            {
                ejercicio.EjercicioImagen4Id = m4.MultimediaId;
                ejercicio.EjercicioImagen4Url = m4.MultimediaUrl;
            }

            ModelCL.Multimedia m5 = oldContEjercicio.Multimedia.Where(m => m.MultimediaOrden == 5).FirstOrDefault();
            if (m5 != null)
            {
                ejercicio.EjercicioImagen5Id = m5.MultimediaId;
                ejercicio.EjercicioImagen5Url = m5.MultimediaUrl;
            }

            return View(ejercicio);
        }

        [HttpPost]
        [Route("editar")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EjercicioViewModel datos, HttpPostedFileBase[] files, int[] checkBorrar)
        {
            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);

            if (Fachada.Functions.es_premium(idUsu) == false)
            {
                TempData["PermisoDenegado"] = true;
                return RedirectToAction("Index", "Home");
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
                new Fachada.ViewModelSelectListChk { Id = "Tren inferior", Valor = "Tren inferior" },
                new Fachada.ViewModelSelectListChk { Id = "Abdominales", Valor = "Abdominales" },
                new Fachada.ViewModelSelectListChk { Id = "Calentamiento", Valor = "Calentamiento" },
                new Fachada.ViewModelSelectListChk { Id = "Estiramiento", Valor = "Estiramiento" },
            };
            ViewBag.lEjCategorias = new SelectList(lEjCategorias, "Id", "Valor");


            if (ModelState.IsValid)
            {
                ModelCL.Contenido contEjercicio = db.Contenido.Find(datos.ContenidoId);

                contEjercicio.ContenidoTitulo = datos.ContenidoTitulo;
                contEjercicio.ContenidoDescripcion = datos.ContenidoDescripcion;
                contEjercicio.ContenidoCuerpo = datos.ContenidoCuerpo;

                contEjercicio.Ejercicio.EjercicioTipo = datos.EjercicioTipo;
                contEjercicio.Ejercicio.EjercicioCategoria = datos.EjercicioCategoria;
                contEjercicio.Ejercicio.EjercicioCaloriasPorMinuto = datos.EjercicioCaloriasPorMinuto;


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

                                ModelCL.Multimedia oldMult = contEjercicio.Multimedia.Where(m => m.MultimediaOrden == cont).FirstOrDefault();

                                string nombreArchivo = Guid.NewGuid().ToString() + "." + file.ContentType.Split('/')[1];
                                var newPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Lucy/Resources/Users", tipoArchivo, "Ejercicios", nombreArchivo);

                                string newUrl = "Resources/Users/" + tipoArchivo + "/Ejercicios/" + nombreArchivo;

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
                                    newMult.MultimediaOrden = Convert.ToInt16(contEjercicio.Multimedia.Count() + 1);

                                    contEjercicio.Multimedia.Add(newMult);
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
