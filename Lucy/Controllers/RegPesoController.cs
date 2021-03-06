﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelCL;
using Lucy.Models;

namespace Lucy.Controllers
{
    [Authorize]
    [RoutePrefix("registros/peso")]
    public class RegPesoController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        [Route("listado")]
        public ActionResult Index()
        {
            long idPer = Convert.ToInt64(Request.Cookies["cookiePer"]["PerId"]);

            List<ModelCL.Registro> registrosPeso = db.Registro.Where(r => r.Peso != null && (r.Persona.PersonaId == idPer)).OrderByDescending(r => r.RegistroFchHora).ToList();

            //List<RegPesoViewModel> reg = new List<RegPesoViewModel>();

            //foreach (ModelCL.Registro rp in registrosPeso)
            //{

            //}

            return View(registrosPeso);
        }

        [Route("crear")]
        public ActionResult Create()
        {
            RegPesoViewModel newRegPeso = new RegPesoViewModel();
            return View(newRegPeso);
        }

        [HttpPost]
        [Route("crear")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RegPesoViewModel datos)
        {
            if (ModelState.IsValid)
            {
                long idPer = Convert.ToInt64(Request.Cookies["cookiePer"]["PerId"]);

                DateTime f = Convert.ToDateTime(datos.RegistroFchHora);
                ModelCL.Registro regPesoEx = db.Registro.Where(r => r.Peso != null && r.Persona.PersonaId == idPer && r.RegistroFchHora == f).FirstOrDefault();

                if (regPesoEx != null)
                {
                    ViewBag.ErrorMessage = "Ya existe un valor de peso registrado en esta fecha. Puede modificarlo si lo desea.";
                    return View(datos);
                }

                ModelCL.Persona Persona = db.Persona.Find(idPer);

                ModelCL.Registro regPeso = new ModelCL.Registro();
                regPeso.RegistroFchHora = Convert.ToDateTime(datos.RegistroFchHora);

                ModelCL.Peso Peso = new ModelCL.Peso();

                Peso.PesoValor = datos.PesoValor;
                regPeso.Peso = Peso;

                Persona.Registro.Add(regPeso);

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(datos);
        }

        [Route("editar")]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            long idPer = Convert.ToInt64(Request.Cookies["cookiePer"]["PerId"]);
            ModelCL.Registro regPeso = db.Registro.Where(r => r.RegistroId == id).FirstOrDefault();
            if (regPeso == null || regPeso.Peso == null || regPeso.Persona.PersonaId != idPer)
            {
                return HttpNotFound();
            }

            RegPesoViewModel vmRegPeso = new RegPesoViewModel();
            vmRegPeso.RegistroId = regPeso.RegistroId;
            //vmRegPeso.PersonaId = regPeso.PersonaId;
            vmRegPeso.RegistroFchHora = regPeso.RegistroFchHora.ToString();
            vmRegPeso.PesoValor = regPeso.Peso.PesoValor;

            return View(vmRegPeso);
        }

        [HttpPost]
        [Route("editar")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RegPesoViewModel datos)
        {
            if (ModelState.IsValid)
            {
                ModelCL.Registro regPeso = db.Registro.Where(r => r.RegistroId == datos.RegistroId).FirstOrDefault();

                DateTime f = Convert.ToDateTime(datos.RegistroFchHora);
                if (regPeso.RegistroFchHora != f)
                {                    
                    ModelCL.Registro regPesoEx = db.Registro.Where(r => r.Peso != null && r.Persona.PersonaId == regPeso.PersonaId && r.RegistroFchHora == f).FirstOrDefault();

                    if (regPesoEx != null)
                    {
                        ViewBag.ErrorMessage = "Ya existe un valor de peso registrado en esta fecha. Puede modificarlo si lo desea.";
                        return View(datos);
                    }
                }

                regPeso.RegistroFchHora = f;
                regPeso.Peso.PesoValor = datos.PesoValor;

                db.SaveChanges();
                return RedirectToAction("Index");
            }

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

            long idPer = Convert.ToInt64(Request.Cookies["cookiePer"]["PerId"]);
            ModelCL.Registro registro = db.Registro.Where(r => r.RegistroId == id && r.Peso != null).FirstOrDefault();
            if (registro == null || registro.Peso == null || registro.Persona.PersonaId != idPer)
            {
                return HttpNotFound();
            }

            db.Registro.Remove(registro);
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
