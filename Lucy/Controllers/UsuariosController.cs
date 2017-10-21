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
    [RoutePrefix("usuarios")]
    public class UsuariosController : Controller
    {
        //private AgustinaEntities db = new AgustinaEntities(); // MM - 18/07/2017 - Lo cambie por el using porque mejora rendimiento

        // Login
        [HttpGet]
        [Route("login")]
        public ActionResult Login()
        {
            return View();
        }

        // Login POST
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("login")]
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

                            var cookiePer = new HttpCookie("cookiePer");
                            cookiePer.Expires = DateTime.Now.AddMinutes(timeout);
                            long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);
                            cookiePer.Values["PerId"] = v.RelUsuPer.Where(u => u.UsuarioId == idUsu).FirstOrDefault().PersonaId.ToString();
                            Response.Cookies.Add(cookiePer);

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
        [Route("logout")]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            HttpCookie cookieUsu = Request.Cookies["cookieUsu"];
            cookieUsu.Expires = DateTime.Now.AddDays(-1d);
            Response.Cookies.Add(cookieUsu);

            if (Request.Cookies["cookiePer"] != null)
            {
                HttpCookie cookiePer = Request.Cookies["cookiePer"];
                cookiePer.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(cookiePer);
            }

            return RedirectToAction("Index", "Home");
        }

        // Registro
        [HttpGet]
        [Route("registro")]
        public ActionResult Registro()
        {
            CargarPaises();
            CargarSexo();
            return View();
        }

        // Registro POST
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("registro")]
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
                        CargarPaises();
                        CargarSexo();
                        return View(reg);
                    }
                    #endregion

                    ModelCL.Usuario usu = new ModelCL.Usuario();

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
                    usu.UsuarioPais = reg.UsuarioPais;
                    usu.UsuarioApp = "Web";
                    usu.UsuarioRecibirEmails = true;
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
                        db.Usuario.Add(usu);

                        db.Persona.Add(per);

                        ModelCL.RelUsuPer rup = new ModelCL.RelUsuPer();
                        rup.Usuario = usu;
                        rup.Persona = per;

                        db.RelUsuPer.Add(rup);


                        ModelCL.RelUsuRol rur = new ModelCL.RelUsuRol();

                        ModelCL.Rol rol = db.Rol.Where(r => r.RolNombre == "Free").FirstOrDefault();

                        rur.Usuario = usu;
                        rur.Rol = rol;

                        db.RelUsuRol.Add(rur);                                                            
                        #endregion

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

                CargarPaises();
                CargarSexo();

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
                CargarPaises();
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
                ViewBag.listaSexos = new SelectList(lSexo, "SexoId", "SexoNombre");
            }
        }

        public void CargarPaises()
        {
            List<Fachada.ViewModelSelectListChk> lPaises = new List<Fachada.ViewModelSelectListChk>()
            {
                new Fachada.ViewModelSelectListChk { Id = "Afghanistan", Valor = "Afghanistan" },
                new Fachada.ViewModelSelectListChk { Id = "Albania", Valor = "Albania" },
                new Fachada.ViewModelSelectListChk { Id = "Algeria", Valor = "Algeria" },
                new Fachada.ViewModelSelectListChk { Id = "Andorra", Valor = "Andorra" },
                new Fachada.ViewModelSelectListChk { Id = "Angola", Valor = "Angola" },
                new Fachada.ViewModelSelectListChk { Id = "Antigua and Barbuda", Valor = "Antigua and Barbuda" },
                new Fachada.ViewModelSelectListChk { Id = "Argentina", Valor = "Argentina" },
                new Fachada.ViewModelSelectListChk { Id = "Armenia", Valor = "Armenia" },
                new Fachada.ViewModelSelectListChk { Id = "Australia", Valor = "Australia" },
                new Fachada.ViewModelSelectListChk { Id = "Austria", Valor = "Austria" },
                new Fachada.ViewModelSelectListChk { Id = "Azerbaijan", Valor = "Azerbaijan" },
                new Fachada.ViewModelSelectListChk { Id = "Bahamas", Valor = "Bahamas" },
                new Fachada.ViewModelSelectListChk { Id = "Bahrain", Valor = "Bahrain" },
                new Fachada.ViewModelSelectListChk { Id = "Bangladesh", Valor = "Bangladesh" },
                new Fachada.ViewModelSelectListChk { Id = "Barbados", Valor = "Barbados" },
                new Fachada.ViewModelSelectListChk { Id = "Belarus", Valor = "Belarus" },
                new Fachada.ViewModelSelectListChk { Id = "Belgium", Valor = "Belgium" },
                new Fachada.ViewModelSelectListChk { Id = "Belize", Valor = "Belize" },
                new Fachada.ViewModelSelectListChk { Id = "Benin", Valor = "Benin" },
                new Fachada.ViewModelSelectListChk { Id = "Bhutan", Valor = "Bhutan" },
                new Fachada.ViewModelSelectListChk { Id = "Bolivia", Valor = "Bolivia" },
                new Fachada.ViewModelSelectListChk { Id = "Bosnia and Herzegovina", Valor = "Bosnia and Herzegovina" },
                new Fachada.ViewModelSelectListChk { Id = "Botswana", Valor = "Botswana" },
                new Fachada.ViewModelSelectListChk { Id = "Brazil", Valor = "Brazil" },
                new Fachada.ViewModelSelectListChk { Id = "Brunei", Valor = "Brunei" },
                new Fachada.ViewModelSelectListChk { Id = "Bulgaria", Valor = "Bulgaria" },
                new Fachada.ViewModelSelectListChk { Id = "Burkina Faso", Valor = "Burkina Faso" },
                new Fachada.ViewModelSelectListChk { Id = "Burundi", Valor = "Burundi" },
                new Fachada.ViewModelSelectListChk { Id = "Cabo Verde", Valor = "Cabo Verde" },
                new Fachada.ViewModelSelectListChk { Id = "Cambodia", Valor = "Cambodia" },
                new Fachada.ViewModelSelectListChk { Id = "Cameroon", Valor = "Cameroon" },
                new Fachada.ViewModelSelectListChk { Id = "Canada", Valor = "Canada" },
                new Fachada.ViewModelSelectListChk { Id = "Central African Republic (CAR)", Valor = "Central African Republic (CAR)" },
                new Fachada.ViewModelSelectListChk { Id = "Chad", Valor = "Chad" },
                new Fachada.ViewModelSelectListChk { Id = "Chile", Valor = "Chile" },
                new Fachada.ViewModelSelectListChk { Id = "China", Valor = "China" },
                new Fachada.ViewModelSelectListChk { Id = "Colombia", Valor = "Colombia" },
                new Fachada.ViewModelSelectListChk { Id = "Comoros", Valor = "Comoros" },
                new Fachada.ViewModelSelectListChk { Id = "Democratic Republic of the Congo", Valor = "Democratic Republic of the Congo" },
                new Fachada.ViewModelSelectListChk { Id = "Republic of the Congo", Valor = "Republic of the Congo" },
                new Fachada.ViewModelSelectListChk { Id = "Costa Rica", Valor = "Costa Rica" },
                new Fachada.ViewModelSelectListChk { Id = "Cote d'Ivoire", Valor = "Cote d'Ivoire" },
                new Fachada.ViewModelSelectListChk { Id = "Croatia", Valor = "Croatia" },
                new Fachada.ViewModelSelectListChk { Id = "Cuba", Valor = "Cuba" },
                new Fachada.ViewModelSelectListChk { Id = "Cyprus", Valor = "Cyprus" },
                new Fachada.ViewModelSelectListChk { Id = "Czech Republic", Valor = "Czech Republic" },
                new Fachada.ViewModelSelectListChk { Id = "Denmark", Valor = "Denmark" },
                new Fachada.ViewModelSelectListChk { Id = "Djibouti", Valor = "Djibouti" },
                new Fachada.ViewModelSelectListChk { Id = "Dominica", Valor = "Dominica" },
                new Fachada.ViewModelSelectListChk { Id = "Dominican Republic", Valor = "Dominican Republic" },
                new Fachada.ViewModelSelectListChk { Id = "Ecuador", Valor = "Ecuador" },
                new Fachada.ViewModelSelectListChk { Id = "Egypt", Valor = "Egypt" },
                new Fachada.ViewModelSelectListChk { Id = "El Salvador", Valor = "El Salvador" },
                new Fachada.ViewModelSelectListChk { Id = "Equatorial Guinea", Valor = "Equatorial Guinea" },
                new Fachada.ViewModelSelectListChk { Id = "Eritrea", Valor = "Eritrea" },
                new Fachada.ViewModelSelectListChk { Id = "Estonia", Valor = "Estonia" },
                new Fachada.ViewModelSelectListChk { Id = "Ethiopia", Valor = "Ethiopia" },
                new Fachada.ViewModelSelectListChk { Id = "Fiji", Valor = "Fiji" },
                new Fachada.ViewModelSelectListChk { Id = "Finland", Valor = "Finland" },
                new Fachada.ViewModelSelectListChk { Id = "France", Valor = "France" },
                new Fachada.ViewModelSelectListChk { Id = "Gabon", Valor = "Gabon" },
                new Fachada.ViewModelSelectListChk { Id = "Gambia", Valor = "Gambia" },
                new Fachada.ViewModelSelectListChk { Id = "Georgia", Valor = "Georgia" },
                new Fachada.ViewModelSelectListChk { Id = "Germany", Valor = "Germany" },
                new Fachada.ViewModelSelectListChk { Id = "Ghana", Valor = "Ghana" },
                new Fachada.ViewModelSelectListChk { Id = "Greece", Valor = "Greece" },
                new Fachada.ViewModelSelectListChk { Id = "Grenada", Valor = "Grenada" },
                new Fachada.ViewModelSelectListChk { Id = "Guatemala", Valor = "Guatemala" },
                new Fachada.ViewModelSelectListChk { Id = "Guinea", Valor = "Guinea" },
                new Fachada.ViewModelSelectListChk { Id = "Guinea-Bissau", Valor = "Guinea-Bissau" },
                new Fachada.ViewModelSelectListChk { Id = "Guyana", Valor = "Guyana" },
                new Fachada.ViewModelSelectListChk { Id = "Haiti", Valor = "Haiti" },
                new Fachada.ViewModelSelectListChk { Id = "Honduras", Valor = "Honduras" },
                new Fachada.ViewModelSelectListChk { Id = "Hungary", Valor = "Hungary" },
                new Fachada.ViewModelSelectListChk { Id = "Iceland", Valor = "Iceland" },
                new Fachada.ViewModelSelectListChk { Id = "India", Valor = "India" },
                new Fachada.ViewModelSelectListChk { Id = "Indonesia", Valor = "Indonesia" },
                new Fachada.ViewModelSelectListChk { Id = "Iran", Valor = "Iran" },
                new Fachada.ViewModelSelectListChk { Id = "Iraq", Valor = "Iraq" },
                new Fachada.ViewModelSelectListChk { Id = "Ireland", Valor = "Ireland" },
                new Fachada.ViewModelSelectListChk { Id = "Israel", Valor = "Israel" },
                new Fachada.ViewModelSelectListChk { Id = "Italy", Valor = "Italy" },
                new Fachada.ViewModelSelectListChk { Id = "Jamaica", Valor = "Jamaica" },
                new Fachada.ViewModelSelectListChk { Id = "Japan", Valor = "Japan" },
                new Fachada.ViewModelSelectListChk { Id = "Jordan", Valor = "Jordan" },
                new Fachada.ViewModelSelectListChk { Id = "Kazakhstan", Valor = "Kazakhstan" },
                new Fachada.ViewModelSelectListChk { Id = "Kenya", Valor = "Kenya" },
                new Fachada.ViewModelSelectListChk { Id = "Kiribati", Valor = "Kiribati" },
                new Fachada.ViewModelSelectListChk { Id = "Kosovo", Valor = "Kosovo" },
                new Fachada.ViewModelSelectListChk { Id = "Kuwait", Valor = "Kuwait" },
                new Fachada.ViewModelSelectListChk { Id = "Kyrgyzstan", Valor = "Kyrgyzstan" },
                new Fachada.ViewModelSelectListChk { Id = "Laos", Valor = "Laos" },
                new Fachada.ViewModelSelectListChk { Id = "Latvia", Valor = "Latvia" },
                new Fachada.ViewModelSelectListChk { Id = "Lebanon", Valor = "Lebanon" },
                new Fachada.ViewModelSelectListChk { Id = "Lesotho", Valor = "Lesotho" },
                new Fachada.ViewModelSelectListChk { Id = "Liberia", Valor = "Liberia" },
                new Fachada.ViewModelSelectListChk { Id = "Libya", Valor = "Libya" },
                new Fachada.ViewModelSelectListChk { Id = "Liechtenstein", Valor = "Liechtenstein" },
                new Fachada.ViewModelSelectListChk { Id = "Lithuania", Valor = "Lithuania" },
                new Fachada.ViewModelSelectListChk { Id = "Luxembourg", Valor = "Luxembourg" },
                new Fachada.ViewModelSelectListChk { Id = "Macedonia (FYROM)", Valor = "Macedonia (FYROM)" },
                new Fachada.ViewModelSelectListChk { Id = "Madagascar", Valor = "Madagascar" },
                new Fachada.ViewModelSelectListChk { Id = "Malawi", Valor = "Malawi" },
                new Fachada.ViewModelSelectListChk { Id = "Malaysia", Valor = "Malaysia" },
                new Fachada.ViewModelSelectListChk { Id = "Maldives", Valor = "Maldives" },
                new Fachada.ViewModelSelectListChk { Id = "Mali", Valor = "Mali" },
                new Fachada.ViewModelSelectListChk { Id = "Malta", Valor = "Malta" },
                new Fachada.ViewModelSelectListChk { Id = "Marshall Islands", Valor = "Marshall Islands" },
                new Fachada.ViewModelSelectListChk { Id = "Mauritania", Valor = "Mauritania" },
                new Fachada.ViewModelSelectListChk { Id = "Mauritius", Valor = "Mauritius" },
                new Fachada.ViewModelSelectListChk { Id = "Mexico", Valor = "Mexico" },
                new Fachada.ViewModelSelectListChk { Id = "Micronesia", Valor = "Micronesia" },
                new Fachada.ViewModelSelectListChk { Id = "Moldova", Valor = "Moldova" },
                new Fachada.ViewModelSelectListChk { Id = "Monaco", Valor = "Monaco" },
                new Fachada.ViewModelSelectListChk { Id = "Mongolia", Valor = "Mongolia" },
                new Fachada.ViewModelSelectListChk { Id = "Montenegro", Valor = "Montenegro" },
                new Fachada.ViewModelSelectListChk { Id = "Morocco", Valor = "Morocco" },
                new Fachada.ViewModelSelectListChk { Id = "Mozambique", Valor = "Mozambique" },
                new Fachada.ViewModelSelectListChk { Id = "Myanmar (Burma)", Valor = "Myanmar (Burma)" },
                new Fachada.ViewModelSelectListChk { Id = "Namibia", Valor = "Namibia" },
                new Fachada.ViewModelSelectListChk { Id = "Nauru", Valor = "Nauru" },
                new Fachada.ViewModelSelectListChk { Id = "Nepal", Valor = "Nepal" },
                new Fachada.ViewModelSelectListChk { Id = "Netherlands", Valor = "Netherlands" },
                new Fachada.ViewModelSelectListChk { Id = "New Zealand", Valor = "New Zealand" },
                new Fachada.ViewModelSelectListChk { Id = "Nicaragua", Valor = "Nicaragua" },
                new Fachada.ViewModelSelectListChk { Id = "Niger", Valor = "Niger" },
                new Fachada.ViewModelSelectListChk { Id = "Nigeria", Valor = "Nigeria" },
                new Fachada.ViewModelSelectListChk { Id = "North Korea", Valor = "North Korea" },
                new Fachada.ViewModelSelectListChk { Id = "Norway", Valor = "Norway" },
                new Fachada.ViewModelSelectListChk { Id = "Oman", Valor = "Oman" },
                new Fachada.ViewModelSelectListChk { Id = "Pakistan", Valor = "Pakistan" },
                new Fachada.ViewModelSelectListChk { Id = "Palau", Valor = "Palau" },
                new Fachada.ViewModelSelectListChk { Id = "Palestine", Valor = "Palestine" },
                new Fachada.ViewModelSelectListChk { Id = "Panama", Valor = "Panama" },
                new Fachada.ViewModelSelectListChk { Id = "Papua New Guinea", Valor = "Papua New Guinea" },
                new Fachada.ViewModelSelectListChk { Id = "Paraguay", Valor = "Paraguay" },
                new Fachada.ViewModelSelectListChk { Id = "Peru", Valor = "Peru" },
                new Fachada.ViewModelSelectListChk { Id = "Philippines", Valor = "Philippines" },
                new Fachada.ViewModelSelectListChk { Id = "Poland", Valor = "Poland" },
                new Fachada.ViewModelSelectListChk { Id = "Portugal", Valor = "Portugal" },
                new Fachada.ViewModelSelectListChk { Id = "Qatar", Valor = "Qatar" },
                new Fachada.ViewModelSelectListChk { Id = "Romania", Valor = "Romania" },
                new Fachada.ViewModelSelectListChk { Id = "Russia", Valor = "Russia" },
                new Fachada.ViewModelSelectListChk { Id = "Rwanda", Valor = "Rwanda" },
                new Fachada.ViewModelSelectListChk { Id = "Saint Kitts and Nevis", Valor = "Saint Kitts and Nevis" },
                new Fachada.ViewModelSelectListChk { Id = "Saint Lucia", Valor = "Saint Lucia" },
                new Fachada.ViewModelSelectListChk { Id = "Saint Vincent and the Grenadines", Valor = "Saint Vincent and the Grenadines" },
                new Fachada.ViewModelSelectListChk { Id = "Samoa", Valor = "Samoa" },
                new Fachada.ViewModelSelectListChk { Id = "San Marino", Valor = "San Marino" },
                new Fachada.ViewModelSelectListChk { Id = "Sao Tome and Principe", Valor = "Sao Tome and Principe" },
                new Fachada.ViewModelSelectListChk { Id = "Saudi Arabia", Valor = "Saudi Arabia" },
                new Fachada.ViewModelSelectListChk { Id = "Senegal", Valor = "Senegal" },
                new Fachada.ViewModelSelectListChk { Id = "Serbia", Valor = "Serbia" },
                new Fachada.ViewModelSelectListChk { Id = "Seychelles", Valor = "Seychelles" },
                new Fachada.ViewModelSelectListChk { Id = "Sierra Leone", Valor = "Sierra Leone" },
                new Fachada.ViewModelSelectListChk { Id = "Singapore", Valor = "Singapore" },
                new Fachada.ViewModelSelectListChk { Id = "Slovakia", Valor = "Slovakia" },
                new Fachada.ViewModelSelectListChk { Id = "Slovenia", Valor = "Slovenia" },
                new Fachada.ViewModelSelectListChk { Id = "Solomon Islands", Valor = "Solomon Islands" },
                new Fachada.ViewModelSelectListChk { Id = "Somalia", Valor = "Somalia" },
                new Fachada.ViewModelSelectListChk { Id = "South Africa", Valor = "South Africa" },
                new Fachada.ViewModelSelectListChk { Id = "South Korea", Valor = "South Korea" },
                new Fachada.ViewModelSelectListChk { Id = "South Sudan", Valor = "South Sudan" },
                new Fachada.ViewModelSelectListChk { Id = "Spain", Valor = "Spain" },
                new Fachada.ViewModelSelectListChk { Id = "Sri Lanka", Valor = "Sri Lanka" },
                new Fachada.ViewModelSelectListChk { Id = "Sudan", Valor = "Sudan" },
                new Fachada.ViewModelSelectListChk { Id = "Suriname", Valor = "Suriname" },
                new Fachada.ViewModelSelectListChk { Id = "Swaziland", Valor = "Swaziland" },
                new Fachada.ViewModelSelectListChk { Id = "Sweden", Valor = "Sweden" },
                new Fachada.ViewModelSelectListChk { Id = "Switzerland", Valor = "Switzerland" },
                new Fachada.ViewModelSelectListChk { Id = "Syria", Valor = "Syria" },
                new Fachada.ViewModelSelectListChk { Id = "Taiwan", Valor = "Taiwan" },
                new Fachada.ViewModelSelectListChk { Id = "Tajikistan", Valor = "Tajikistan" },
                new Fachada.ViewModelSelectListChk { Id = "Tanzania", Valor = "Tanzania" },
                new Fachada.ViewModelSelectListChk { Id = "Thailand", Valor = "Thailand" },
                new Fachada.ViewModelSelectListChk { Id = "Timor-Leste", Valor = "Timor-Leste" },
                new Fachada.ViewModelSelectListChk { Id = "Togo", Valor = "Togo" },
                new Fachada.ViewModelSelectListChk { Id = "Tonga", Valor = "Tonga" },
                new Fachada.ViewModelSelectListChk { Id = "Trinidad and Tobago", Valor = "Trinidad and Tobago" },
                new Fachada.ViewModelSelectListChk { Id = "Tunisia", Valor = "Tunisia" },
                new Fachada.ViewModelSelectListChk { Id = "Turkey", Valor = "Turkey" },
                new Fachada.ViewModelSelectListChk { Id = "Turkmenistan", Valor = "Turkmenistan" },
                new Fachada.ViewModelSelectListChk { Id = "Tuvalu", Valor = "Tuvalu" },
                new Fachada.ViewModelSelectListChk { Id = "Uganda", Valor = "Uganda" },
                new Fachada.ViewModelSelectListChk { Id = "Ukraine", Valor = "Ukraine" },
                new Fachada.ViewModelSelectListChk { Id = "United Arab Emirates (UAE)", Valor = "United Arab Emirates (UAE)" },
                new Fachada.ViewModelSelectListChk { Id = "United Kingdom (UK)", Valor = "United Kingdom (UK)" },
                new Fachada.ViewModelSelectListChk { Id = "United States of America (USA)", Valor = "United States of America (USA)" },
                new Fachada.ViewModelSelectListChk { Id = "Uruguay", Valor = "Uruguay" },
                new Fachada.ViewModelSelectListChk { Id = "Uzbekistan", Valor = "Uzbekistan" },
                new Fachada.ViewModelSelectListChk { Id = "Vanuatu", Valor = "Vanuatu" },
                new Fachada.ViewModelSelectListChk { Id = "Vatican City (Holy See)", Valor = "Vatican City (Holy See)" },
                new Fachada.ViewModelSelectListChk { Id = "Venezuela", Valor = "Venezuela" },
                new Fachada.ViewModelSelectListChk { Id = "Vietnam", Valor = "Vietnam" },
                new Fachada.ViewModelSelectListChk { Id = "Yemen", Valor = "Yemen" },
                new Fachada.ViewModelSelectListChk { Id = "Zambia", Valor = "Zambia" },
                new Fachada.ViewModelSelectListChk { Id = "Zimbabwe", Valor = "Zimbabwe" },
            };
            ViewBag.lPaises = new SelectList(lPaises, "Id", "Valor");
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
            var verifyUrl = "/Usuarios/VerifyAccount/?id=" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("mateswdv@gmail.com", "YoTeCuido");
            var toEmail = new MailAddress(email);
            var fromEmailPassword = "mateSolutions07uy";
            string subject = "¡Su cuenta se ha creado correctamente!";

            string body = "<br/><br/>Estamos encantados de decirle que su cuenta en YoTeCuido se ha" +
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
        [Route("verifyaccount")]
        public ActionResult VerifyAccount(string id)
        {
            bool Status = false;
            using (AgustinaEntities db = new AgustinaEntities())
            {
                db.Configuration.ValidateOnSaveEnabled = false; // This line I have added here to avoid
                                                                // Confirm password does not match issue on save changes
                var v = db.Usuario.Where(a => a.UsuarioCodAct == new Guid(id)).FirstOrDefault();
                if (v != null)
                {
                    v.UsuarioMailConf = true;
                    db.SaveChanges();
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

        [Route("lastlogin")]
        public ActionResult LastLogin()
        {
            if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
            {
                long idUsu = Fachada.Functions.get_idUsu(Request.Cookies[FormsAuthentication.FormsCookieName]);

                using (AgustinaEntities db = new AgustinaEntities())
                {
                    ModelCL.Usuario Usuario = db.Usuario.Find(idUsu);

                    Usuario.UsuarioLastLogin = DateTime.Now;

                    db.SaveChanges();
                }
            }
            
            return null;
        }
    }
}
