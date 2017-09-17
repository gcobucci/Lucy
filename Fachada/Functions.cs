using System;
using System.Web;
using System.Web.Security;

namespace Fachada
{
    public class Functions
    {
        public static bool isValidContentType(string contentType)
        {
            return contentType.Equals("image/png") || contentType.Equals("image/gif") ||
                contentType.Equals("image/jpg") || contentType.Equals("image/jpeg");//Posiblemente agregar videos
        }


        public static bool isValidContentLength(int contentLenght)
        {
            return ((contentLenght / 1024) / 1024) < 1; //1MB
        }


        public static int get_idUsu(HttpCookie cookie)
        {
            FormsAuthenticationTicket usu = FormsAuthentication.Decrypt(cookie.Value);
            int idUsu = Convert.ToInt32(usu.Name);

            return idUsu;
        }

        public static int get_idPer(HttpCookie cookie)
        {
            FormsAuthenticationTicket per = FormsAuthentication.Decrypt(cookie.Value);
            int idPer = Convert.ToInt32(per.Name);

            return idPer;
        }
    }
}