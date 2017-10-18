using ModelCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lucy.Controllers
{
    [Authorize]
    [RoutePrefix("historial_clinico")]
    public class HistorialClinicoController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        [Route("")]
        public ActionResult Index()
        {
            long idPer = Convert.ToInt64(Request.Cookies["cookiePer"]["PerId"]);

            ViewBag.Persona = db.Persona.Find(idPer).nombreCompleto;

            return View();
        }

        [Route("_relacion_peso_altura")]
        public PartialViewResult _RelacionPesoAltura()
        {
            long idPer = Convert.ToInt64(Request.Cookies["cookiePer"]["PerId"]);

            ViewBag.Persona = db.Persona.Find(idPer).nombreCompleto;

            return PartialView();
        }

        [Route("_dieta")]
        public PartialViewResult _Dieta()
        {
            long idPer = Convert.ToInt64(Request.Cookies["cookiePer"]["PerId"]);

            ModelCL.Persona Persona = db.Persona.Find(idPer);


            List<ModelCL.Registro> lRegDesayunos = Persona.Registro.Where(r => r.Comida != null && r.Comida.ComidaComida == "Desayuno" && r.Comida.ComidaCalorias != null).ToList();

            int cantDesayunos = lRegDesayunos.Count();

            if (cantDesayunos > 0)
            {
                int totalCalDes = 0;
                foreach (ModelCL.Registro regDes in lRegDesayunos)
                {
                    totalCalDes += Convert.ToInt32(regDes.Comida.ComidaCalorias);
                }

                ViewBag.CaloriasDesayuno = totalCalDes / cantDesayunos + " (" + cantDesayunos + " desayuno/s registrado/s)";
            }
            else
            {
                ViewBag.CaloriasDesayuno = 0 + " (No hay registros)";
            }


            
            List<ModelCL.Registro> lRegAlmuerzos = Persona.Registro.Where(r => r.Comida != null && r.Comida.ComidaComida == "Almuerzo" && r.Comida.ComidaCalorias != null).ToList();

            int cantAlmuerzos = lRegAlmuerzos.Count();

            if (cantAlmuerzos > 0)
            {                
                int totalCalAlm = 0;
                foreach (ModelCL.Registro regAlm in lRegAlmuerzos)
                {
                    totalCalAlm += Convert.ToInt32(regAlm.Comida.ComidaCalorias);
                }

                ViewBag.CaloriasAlmuerzo = totalCalAlm / cantAlmuerzos + " (" + cantAlmuerzos + " almuerzo/s registrado/s)";
            }
            else
            {
                ViewBag.CaloriasAlmuerzo = 0 + " (No hay registros)";
            }



            List<ModelCL.Registro> lRegMeriendas = Persona.Registro.Where(r => r.Comida != null && r.Comida.ComidaComida == "Merienda" && r.Comida.ComidaCalorias != null).ToList();

            int cantMeriendas = lRegMeriendas.Count();

            if (cantMeriendas > 0)
            {
                int totalCalMer = 0;
                foreach (ModelCL.Registro regMer in lRegMeriendas)
                {
                    totalCalMer += Convert.ToInt32(regMer.Comida.ComidaCalorias);
                }

                ViewBag.CaloriasMerienda = totalCalMer / cantMeriendas + " (" + cantMeriendas + " merienda/s registrada/s)";
            }
            else
            {
                ViewBag.CaloriasMerienda = 0 + " (No hay registros)";
            }



            List<ModelCL.Registro> lRegCenas = Persona.Registro.Where(r => r.Comida != null && r.Comida.ComidaComida == "Cena" && r.Comida.ComidaCalorias != null).ToList();

            int cantCenas = lRegCenas.Count();

            if (cantCenas > 0)
            {
                int totalCalCen = 0;
                foreach (ModelCL.Registro regCen in lRegCenas)
                {
                    totalCalCen += Convert.ToInt32(regCen.Comida.ComidaCalorias);
                }

                ViewBag.CaloriasCena = totalCalCen / cantCenas + " (" + cantCenas + " cena/s registrada/s)";
            }
            else
            {
                ViewBag.CaloriasCena = 0 + " (No hay registros)";
            }



            List<ModelCL.Registro> lRegIngestas = Persona.Registro.Where(r => r.Comida != null && r.Comida.ComidaComida == "Ingesta" && r.Comida.ComidaCalorias != null).ToList();

            int cantIngestas = lRegIngestas.Count();

            if (cantIngestas > 0)
            {
                int totalCalIng = 0;
                foreach (ModelCL.Registro regIng in lRegCenas)
                {
                    totalCalIng += Convert.ToInt32(regIng.Comida.ComidaCalorias);
                }

                ViewBag.CaloriasIngestas = totalCalIng / cantIngestas + " (" + cantIngestas + " ingesta/s registrada/s)";
            }
            else
            {
                ViewBag.CaloriasIngestas = 0 + " (No hay registros)";
            }



            return PartialView();
        }
    }
}