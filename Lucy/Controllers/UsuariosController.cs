using Lucy.Models;
using ModelCL;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web.Security;
using System.Web;
using System.Web.Script.Serialization;
using System.Security.Principal;

namespace Lucy.Controllers
{
    public class UsuariosController : Controller
    {
        //private AgustinaEntities db = new AgustinaEntities(); // MM - 18/07/2017 - Lo cambie por el using porque mejora rendimiento

        // Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        // Login POST
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Login(LoginViewModel login, string ReturnUrl = "")
        {
            string message = "";
            using (AgustinaEntities db = new AgustinaEntities())
            {
                var v = db.Usuario.Where(a => (a.UsuarioEmail == login.LoginUsuario || a.UsuarioNombre == login.LoginUsuario)).FirstOrDefault();
                if (v != null)
                {
                    if (v.UsuarioMailConf)
                    {
                        if (string.Compare(Crypto.Hash(login.LoginPass), v.UsuarioPass) == 0)
                        {
                            int timeout = login.Recordarme ? 525600 : 20; // 525600 min = 1 year
                            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(Convert.ToString(v.UsuarioId), login.Recordarme, timeout);
                            string encrypted = FormsAuthentication.Encrypt(ticket);
                            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                            cookie.Expires = DateTime.Now.AddMinutes(timeout);
                            cookie.HttpOnly = true;

                            Response.Cookies.Add(cookie);

                            var cookieUsu = new HttpCookie("cookieUsu");
                            cookieUsu.Expires = DateTime.Now.AddMinutes(timeout);
                            cookieUsu.Values["UsuNom"] = v.UsuarioNombre;
                            Response.Cookies.Add(cookieUsu);

                            if (Url.IsLocalUrl(ReturnUrl))
                            {
                                return Redirect(ReturnUrl);
                            }
                            else
                            {
                                return RedirectToAction("Index", "Home");
                            }
                        }
                        else
                        {
                            message = "La contraseña no es correcta. Por favor vuelva a ingresarla.";
                        }
                    }
                    else
                    {
                        message = "Esta cuenta no está activada. Por favor diríjase al enlace que le hemos enviado a su correo.";
                    }
                }
                else
                {
                    message = "Usuario y/o contraseña inválidos. Intente de nuevo.";
                }
            }
            ViewBag.Message = message;
            return View();
        }

        // Logout
        [Authorize]
        //[HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            HttpCookie cookieUsu = Request.Cookies["cookieUsu"];
            //HttpCookie cookieUsu = new HttpCookie("UserSettings");
            cookieUsu.Expires = DateTime.Now.AddDays(-1d);
            Response.Cookies.Add(cookieUsu);
            return RedirectToAction("Index", "Home");
        }

        // Registro
        [HttpGet]
        public ActionResult Registro()
        {
            CargarSexo();
            return View();
        }

        // Registro POST
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Registro(RegistroViewModel reg)
        {
            bool Status = false;
            string msg = "";

            try
            {
                // Model Validation
                if (ModelState.IsValid)
                {

                    #region Email is already Exist
                    var isExist = IsEmailExist(reg.UsuarioEmail);
                    if (isExist)
                    {
                        ModelState.AddModelError("EmailExist", "Ya existe el correo electrónico");
                        CargarSexo();
                        return View(reg);
                    }
                    #endregion

                    Usuario usu = new Usuario();

                    #region Generate Activation Code
                    usu.UsuarioCodAct = Guid.NewGuid();
                    #endregion

                    #region  Password Hashing
                    usu.UsuarioPass = Crypto.Hash(reg.UsuarioPass);
                    reg.UsuarioPassConfirmacion = Crypto.Hash(reg.UsuarioPassConfirmacion);
                    #endregion

                    #region Cargo Usuario
                    usu.UsuarioNombre = reg.UsuarioNombre;
                    usu.UsuarioEmail = reg.UsuarioEmail;
                    usu.UsuarioApp = "Web";
                    #endregion

                    #region Cargo Persona
                    Persona per = new Persona();
                    per.PersonaNombre = reg.PersonaNombre;
                    per.PersonaApellido = reg.PersonaApellido;
                    per.PersonaFchNac = Convert.ToDateTime(reg.PersonaFchNac);
                    per.SexoId = reg.SexoId;
                    #endregion

                    usu.UsuarioMailConf = false;

                    #region Save to Database
                    using (AgustinaEntities db = new AgustinaEntities())
                    {
                        #region  Junto todo
                        Rol rol = db.Rol.Where(r => r.RolNombre == "Free").FirstOrDefault();

                        usu.Persona.Add(per);
                        //usu.Rol.Add(rol);
                        per.Usuario.Add(usu);
                        #endregion

                        db.Usuario.Add(usu);
                        db.Persona.Add(per);
                        db.SaveChanges();

                        //Send Email to User
                        SendVerificationLinkEmail(reg.UsuarioEmail, usu.UsuarioCodAct.ToString());
                        msg = "Registro realizado con éxito. El enlace de activación de la cuenta se ha enviado a tu correo electrónico: " + usu.UsuarioEmail;
                        Status = true;
                    }
                    #endregion
                }
                else
                {
                    msg = "Solicitud Inválida";
                }

                ViewBag.Message = msg;
                ViewBag.Status = Status;

                return View(reg);
            }
            catch (DbEntityValidationException dbEx)
            {
                #region Catch
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                    }
                }

                msg = "Lo sentimos, ha ocurrido un error inesperado. Intente mas tarde.";

                ViewBag.ErrorMessage = msg;
                ViewBag.Status = true;
                CargarSexo();
                return View(reg);
                #endregion
            }
        }

        // Lista de Sexos
        [NonAction]
        public void CargarSexo()
        {
            using (AgustinaEntities db = new AgustinaEntities())
            {
                List<ModelCL.Sexo> lSexo = db.Sexo.ToList();
                ViewData["listaSexos"] = new SelectList(lSexo, "SexoId", "SexoNombre");
            }
        }

        [NonAction]
        public bool IsEmailExist(string email)
        {
            using (AgustinaEntities db = new AgustinaEntities())
            {
                var v = db.Usuario.Where(a => a.UsuarioEmail == email).FirstOrDefault();
                return v != null;
            }
        }

        [NonAction]
        public void SendVerificationLinkEmail(string email, string activationCode)
        {
            var verifyUrl = "/Usuarios/VerifyAccount/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("mateswdv@gmail.com", "Lucy");
            var toEmail = new MailAddress(email);
            var fromEmailPassword = "mateSolutions07uy";
            string subject = "¡Su cuenta se ha creado correctamente!";

            string body = "<br/><br/>Estamos encantados de decirle que su cuenta de Lucy se ha" +
                " creado correctamente. Por favor haga clic en el siguiente enlace para verificar su cuenta" +
                " <br/><br/><a href='" + link + "'>" + link + "</a> ";

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
            })
                smtp.Send(message);
        }

        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool Status = false;
            using (AgustinaEntities dc = new AgustinaEntities())
            {
                dc.Configuration.ValidateOnSaveEnabled = false; // This line I have added here to avoid
                                                                // Confirm password does not match issue on save changes
                var v = dc.Usuario.Where(a => a.UsuarioCodAct == new Guid(id)).FirstOrDefault();
                if (v != null)
                {
                    v.UsuarioMailConf = true;
                    dc.SaveChanges();
                    Status = true;
                }
                else
                {
                    ViewBag.Message = "Solicitud Inválida";
                }
            }
            ViewBag.Status = Status;
            return View();
        }
    }
}

#region OLD Registro
//public ActionResult Registro(RegistroViewModel Registro)
//{
//try
//{
//    ModelCL.Usuario Usuario = new ModelCL.Usuario();
//    Usuario.UsuarioNombre = Registro.UsuarioNombre;
//    Usuario.UsuarioEmail = Registro.UsuarioEmail;
//    Usuario.UsuarioPass = Registro.UsuarioPass;
//    Usuario.UsuarioApp = "Web";

//    ModelCL.Rol Rol = db.Rol.Find(2);//Free

//    Usuario.Rol.Add(Rol);

//    //db.Usuario.Add(Usuario);

//    ModelCL.Persona Persona = new ModelCL.Persona();
//    Persona.PersonaNombre = Registro.PersonaNombre;
//    Persona.PersonaApellido = Registro.PersonaApellido;
//    Persona.PersonaFchNac = Convert.ToDateTime(Registro.PersonaFchNac);
//    Persona.SexoId = Registro.SexoId;

//    //db.Persona.Add(Persona);

//    Usuario.Persona.Add(Persona);
//    Persona.Usuario.Add(Usuario);

//    db.Usuario.Add(Usuario);
//    db.Persona.Add(Persona);

//    db.SaveChanges();

//    // inicio la sesion con el usuario creado
//    Session["UsuarioId"] = Usuario.UsuarioId;
//    Session["UsuarioNombre"] = Usuario.UsuarioNombre;
//    Session["UsuarioRol"] = Usuario.Rol;

//    return View("~/Views/Home/Index");
//}
//catch (Exception ex)
//{
//    // cargo nuevamente la lista de sexos porque no pasa por el Get
//    CargarSexo();

//    ViewData["msgError"] = ex.GetBaseException();
//    return View();
//}
//}
#endregion

#region OLD Login
//public ActionResult Login(LoginViewModel Login)
//{
//    //var query = db.Usuario.Where(usu => (usu.UsuarioNombre == Login.LoginUsuario || usu.UsuarioEmail == Login.LoginUsuario) && usu.UsuarioPass == Login.LoginPass).FirstOrDefault();
//    using (AgustinaEntities db = new AgustinaEntities())
//    {
//        Usuario query = db.Usuario.SingleOrDefault(usu => (usu.UsuarioNombre == Login.LoginUsuario || usu.UsuarioEmail == Login.LoginUsuario) && usu.UsuarioPass == Login.LoginPass);


//        if (query != null)
//        {
//            // inicio la sesion con el usuario creado
//            Session["UsuarioId"] = query.UsuarioId;
//            Session["UsuarioNombre"] = query.UsuarioNombre;
//            Session["UsuarioRol"] = query.Rol;

//            return View("~/Views/Home/Index.cshtml");
//        }
//        else
//        {
//            return Content("No ingresaste");
//        }
//    }
//}
#endregion