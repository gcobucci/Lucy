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

            ModelCL.Persona Persona = db.Persona.Find(idPer);


            //Datos generales//            
            List<ModelCL.Registro> regPeso = Persona.Registro.Where(r => r.Peso != null).OrderBy(r => r.RegistroFchHora).ToList();

            ViewBag.regPeso = regPeso;

            List<ModelCL.Registro> Pesos = Persona.Registro.Where(r => r.Peso != null).OrderBy(r => r.RegistroFchHora).ToList();


            //Dieta//         
            List<ModelCL.Registro> lRegDesayunos = Persona.Registro.Where(r => r.Comida != null && r.Comida.ComidaComida == "Desayuno" && r.Comida.ComidaCalorias != null).ToList();

            int cantDesayunos = lRegDesayunos.Count();
            int promedioCaloriasDesayuno = 0;

            if (cantDesayunos > 0)
            {
                int totalCalDes = 0;
                foreach (ModelCL.Registro regDes in lRegDesayunos)
                {
                    totalCalDes += Convert.ToInt32(regDes.Comida.ComidaCalorias);
                }

                promedioCaloriasDesayuno = totalCalDes / cantDesayunos;
            }



            List<ModelCL.Registro> lRegAlmuerzos = Persona.Registro.Where(r => r.Comida != null && r.Comida.ComidaComida == "Almuerzo" && r.Comida.ComidaCalorias != null).ToList();

            int cantAlmuerzos = lRegAlmuerzos.Count();
            int promedioCaloriasAlmuerzo = 0;

            if (cantAlmuerzos > 0)
            {
                int totalCalAlm = 0;
                foreach (ModelCL.Registro regAlm in lRegAlmuerzos)
                {
                    totalCalAlm += Convert.ToInt32(regAlm.Comida.ComidaCalorias);
                }

                promedioCaloriasAlmuerzo = totalCalAlm / cantAlmuerzos;
            }



            List<ModelCL.Registro> lRegMeriendas = Persona.Registro.Where(r => r.Comida != null && r.Comida.ComidaComida == "Merienda" && r.Comida.ComidaCalorias != null).ToList();

            int cantMeriendas = lRegMeriendas.Count();
            int promedioCaloriasMerienda = 0;

            if (cantMeriendas > 0)
            {
                int totalCalMer = 0;
                foreach (ModelCL.Registro regMer in lRegMeriendas)
                {
                    totalCalMer += Convert.ToInt32(regMer.Comida.ComidaCalorias);
                }

                promedioCaloriasMerienda = totalCalMer / cantMeriendas;
            }



            List<ModelCL.Registro> lRegCenas = Persona.Registro.Where(r => r.Comida != null && r.Comida.ComidaComida == "Cena" && r.Comida.ComidaCalorias != null).ToList();

            int cantCenas = lRegCenas.Count();
            int promedioCaloriasCena = 0;

            if (cantCenas > 0)
            {
                int totalCalCen = 0;
                foreach (ModelCL.Registro regCen in lRegCenas)
                {
                    totalCalCen += Convert.ToInt32(regCen.Comida.ComidaCalorias);
                }

                promedioCaloriasCena = totalCalCen / cantCenas;
            }



            List<ModelCL.Registro> lRegIngestas = Persona.Registro.Where(r => r.Comida != null && r.Comida.ComidaComida == "Ingesta" && r.Comida.ComidaCalorias != null).ToList();

            int cantIngestas = lRegIngestas.Count();
            int promedioCaloriasIngestas = 0;

            if (cantIngestas > 0)
            {
                int totalCalIng = 0;
                foreach (ModelCL.Registro regIng in lRegCenas)
                {
                    totalCalIng += Convert.ToInt32(regIng.Comida.ComidaCalorias);
                }

                promedioCaloriasIngestas = totalCalIng / cantIngestas;
            }


            int totalPromedioCalorias = promedioCaloriasDesayuno + promedioCaloriasAlmuerzo + promedioCaloriasMerienda + promedioCaloriasCena + promedioCaloriasIngestas;

            ViewBag.VerGraficaDieta = false;
            if (totalPromedioCalorias > 0)
            {
                ViewBag.VerGraficaDieta = true;

                ViewBag.CaloriasDesayunoPorcentaje = promedioCaloriasDesayuno / totalPromedioCalorias;
                ViewBag.CaloriasAlmuerzoPorcentaje = promedioCaloriasAlmuerzo / totalPromedioCalorias;
                ViewBag.CaloriasMeriendaPorcentaje = promedioCaloriasMerienda / totalPromedioCalorias;
                ViewBag.CaloriasCenaPorcentaje = promedioCaloriasCena / totalPromedioCalorias;
                ViewBag.CaloriasIngestasPorcentaje = promedioCaloriasIngestas / totalPromedioCalorias;
            }            


            //Diabetes tipo 1//
            ViewBag.VerDiabeticoTipo1 = false;

            if (Fachada.Functions.es_diabetico_tipo_1(idPer) == true || Fachada.Functions.fue_diabetico_tipo_1(idPer) == true)
            {
                ViewBag.VerDiabeticoTipo1 = true;


                List<ModelCL.Registro> regsControlGlucosa = Persona.Registro.Where(r => r.Control != null && r.Control.Valor.ValorNombre == "Glucosa").ToList();

                int controlesTotal = regsControlGlucosa.Count();
                
                int controlesCorrectos = 0;
                int controlesRegulares = 0;
                int controlesPeligrosos = 0;

                ModelCL.Valor valGlu = db.Valor.Where(v => v.ValorNombre == "Glucosa").FirstOrDefault();

                foreach (ModelCL.Registro reg in regsControlGlucosa)
                {
                    double valor = reg.Control.ControlValor;
                    if (valor >= valGlu.ValorNormalMinimo && valor <= valGlu.ValorNormalMaximo)
                    {
                        controlesCorrectos += 1;
                    }
                    else if ((valor >= valGlu.ValorBajoMinimo && valor < valGlu.ValorNormalMinimo) || (valor > valGlu.ValorNormalMaximo && valor <= valGlu.ValorAltoMaximo))
                    {
                        controlesRegulares += 1;
                    }
                    else
                    {
                        controlesPeligrosos += 1;
                    }
                }

                if (controlesCorrectos > 0)
                {
                    ViewBag.ControlesCorrectosPorcentaje = (controlesCorrectos * 100) / controlesTotal;
                }
                else
                {
                    ViewBag.ControlesCorrectosPorcentaje = 0;
                }


                if (controlesRegulares > 0)
                {
                    ViewBag.ControlesRegularesPorcentaje = (controlesRegulares * 100) / controlesTotal;
                }
                else
                {
                    ViewBag.ControlesRegularesPorcentaje = 0;
                }


                if (controlesPeligrosos > 0)
                {
                    ViewBag.ControlesPeligrososPorcentaje = (controlesPeligrosos * 100) / controlesTotal;
                }
                else
                {
                    ViewBag.ControlesPeligrososPorcentaje = 0;
                }
            }          
                       
            return View();
        }



        [Route("_datos_generales")]
        public PartialViewResult _DatosGenerales()
        {
            long idPer = Convert.ToInt64(Request.Cookies["cookiePer"]["PerId"]);
            ModelCL.Persona Persona = db.Persona.Find(idPer);

            ModelCL.Registro regPeso = Persona.Registro.Where(r => r.Peso != null).OrderByDescending(r => r.RegistroFchHora).FirstOrDefault();
            Nullable<double> peso = null;

            if (regPeso != null)
            {
                peso = regPeso.Peso.PesoValor;
            }

            ModelCL.Registro regAltura = Persona.Registro.Where(r => r.DatCli != null && r.DatCli.DatCliAltura != null).OrderByDescending(r => r.RegistroFchHora).FirstOrDefault();
            Nullable<short> altura = null;

            if (regAltura != null)
            {
                altura = Convert.ToInt16(regAltura.DatCli.DatCliAltura);
            }

            if (peso != null)
            {
                ViewBag.Peso = peso;
            }
            else
            {
                ViewBag.Peso = "No hay registros";
            }


            if (altura != null)
            {
                ViewBag.Altura = altura;
            }
            else
            {
                ViewBag.Altura = "No hay registros";
            }


            if (peso == null || altura == null)
            {
                ViewBag.IMC = "No ha registrado los datos requeridos para este calculo";
            }
            else
            {
                ViewBag.IMC = Fachada.Functions.calcular_IMC(Convert.ToDouble(peso), Convert.ToInt16(altura));
            }


            if (peso == null || altura == null || (Persona.Sexo.SexoNombre != "Hombre" && Persona.Sexo.SexoNombre != "Mujer"))
            {
                ViewBag.IMC = "No ha registrado los datos requeridos para este calculo";
            }
            else
            {
                ViewBag.TMB = Fachada.Functions.calcular_TMB(Convert.ToDouble(peso), Convert.ToInt16(altura), Persona.edad, Persona.Sexo.SexoNombre);
            }
            
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

                ViewBag.CaloriasDesayuno = totalCalDes / cantDesayunos + " cal.";
            }
            else
            {
                ViewBag.CaloriasDesayuno = 0 + " cal.";
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

                ViewBag.CaloriasAlmuerzo = totalCalAlm / cantAlmuerzos + " cal.";
            }
            else
            {
                ViewBag.CaloriasAlmuerzo = 0 + " cal.";
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

                ViewBag.CaloriasMerienda = totalCalMer / cantMeriendas + " cal.";
            }
            else
            {
                ViewBag.CaloriasMerienda = 0 + " cal.";
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

                ViewBag.CaloriasCena = totalCalCen / cantCenas + " cal.";
            }
            else
            {
                ViewBag.CaloriasCena = 0 + " cal.";
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

                ViewBag.CaloriasIngestas = totalCalIng / cantIngestas + " cal.";
            }
            else
            {
                ViewBag.CaloriasIngestas = 0 + " cal.";
            }

            return PartialView();
        }



        [Route("_diabetes_tipo_1")]
        public PartialViewResult _DiabetesTipo1()
        {
            long idPer = Convert.ToInt64(Request.Cookies["cookiePer"]["PerId"]);
            ModelCL.Persona Persona = db.Persona.Find(idPer);


            List<ModelCL.Registro> regsControlGlucosa = Persona.Registro.Where(r => r.Control != null && r.Control.Valor.ValorNombre == "Glucosa").ToList();

            ViewBag.ControlesTotal = regsControlGlucosa.Count();


            int controlesCorrectos = 0;
            int controlesRegulares = 0;
            int controlesPeligrosos = 0;

            ModelCL.Valor valGlu = db.Valor.Where(v => v.ValorNombre == "Glucosa").FirstOrDefault();

            foreach (ModelCL.Registro reg in regsControlGlucosa)
            {
                double valor = reg.Control.ControlValor;
                if (valor > valGlu.ValorNormalMinimo && valor < valGlu.ValorNormalMaximo)
                {
                    controlesCorrectos += 1;
                }
                else if ((valor > valGlu.ValorBajoMinimo && valor < valGlu.ValorNormalMinimo) || (valor > valGlu.ValorNormalMaximo && valor < valGlu.ValorAltoMaximo))
                {
                    controlesRegulares += 1;
                }
                else
                {
                    controlesPeligrosos += 1;
                }
            }

            ViewBag.ControlesCorrectos = controlesCorrectos;
            ViewBag.ControlesRegulares = controlesRegulares;
            ViewBag.ControlesPeligrosos = controlesPeligrosos;

            return PartialView();
        }        
    }
}
