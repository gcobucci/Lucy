using Backend.Models;
using ModelCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Backend.Controllers
{
    [RoutePrefix("emails")]
    public class EmailsController : Controller
    {
        private AgustinaEntities db = new AgustinaEntities();

        [HttpGet]
        [Route("enviar")]
        public ActionResult EnviarEmails()
        {
            EmailViewModel eVM = new EmailViewModel();

            List<ModelCL.Enfermedad> lEnfermedades = db.Enfermedad.Where(e => e.Usuario == null).ToList();

            ModelCL.Enfermedad enfermedad = new ModelCL.Enfermedad();
            enfermedad.EnfermedadId = 0;
            enfermedad.EnfermedadNombre = ">> Enviar a todos los usuarios <<";

            lEnfermedades.Add(enfermedad);
            ViewBag.lEnfermedades = new SelectList(lEnfermedades, "EnfermedadId", "EnfermedadNombre");

            return View(eVM);
        }

        [HttpPost]
        [Route("enviar")]
        [ValidateAntiForgeryToken]
        public ActionResult EnviarEmails(EmailViewModel datos)
        {
            try
            {
                List<ModelCL.Usuario> usuarios = db.Usuario.ToList();

                if (datos.RespetarDecisión == true)
                {
                    usuarios = usuarios.Where(u => u.UsuarioRecibirEmails == true).ToList();
                }

                if (datos.EnfermedadId != 0)
                {
                    usuarios = usuarios.Where(u => u.RelUsuPer.Where(rup => rup.Persona.RelPerEnf.Where(rpe => rpe.Enfermedad.EnfermedadId == datos.EnfermedadId).FirstOrDefault() != null).FirstOrDefault() != null).ToList();
                }


                var fromEmail = new MailAddress("mateswdv@gmail.com", "YoTeCuido");
                var fromEmailPassword = "mateSolutions07uy";

                foreach (ModelCL.Usuario usu in usuarios)
                {                  
                    var toEmail = new MailAddress(usu.UsuarioEmail);

                    string subject = datos.Asunto;
                    string body = datos.Mensaje;

                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
                    };

                    using (var message = new MailMessage(fromEmail, toEmail)
                    {
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    }) smtp.Send(message);
                }

                TempData["Success"] = "Listo";
                return RedirectToAction("EnviarEmails");
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "Error inesperado";
                return View(datos);
            }            
        }
    }
}