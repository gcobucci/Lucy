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

        //public static int get_idPer(HttpCookie cookie)
        //{

        //    FormsAuthenticationTicket per = FormsAuthentication.Decrypt(cookie.Value);
        //    int idPer = Convert.ToInt32(per.Name);

        //    return idPer;
        //}

        public static double calcular_IMC(double peso, short altura)
        {
            return Math.Round(peso / ((Convert.ToDouble(altura) / 100) * (Convert.ToDouble(altura) / 100)), 2);
        }

        public static short calcular_TMB(double peso, short altura, short edad, string sexo)
        {
            short tmb;

            if (sexo == "Hombre")
            {
                //tmb = Convert.ToInt16((10 * peso) + (6.25 * altura) - (5 * edad) + 5);
                tmb = Convert.ToInt16(66.4730 + ((13.751 * peso) + (5.0033 * altura) - (6.75 * edad)));
            }
            else
            {
                //tmb = Convert.ToInt16((10 * peso) + (6.25 * altura) - (5 * edad) - 161);
                tmb = Convert.ToInt16(655.1 + ((9.463 * peso) + (1.8 * altura) - (4.6756 * edad)));
            }

            return tmb;
        }

        public static double calcular_calorias_para_mantenerse(short tmb, string nivelActividad)
        {
            short calorias;

            switch (nivelActividad)
            {
                case "Sedentario":
                    calorias = Convert.ToInt16(Convert.ToDouble(tmb) * 1.2);
                    break;

                case "Escasa":
                    calorias = Convert.ToInt16(Convert.ToDouble(tmb) * 1.375);
                    break;

                case "Moderada":
                    calorias = Convert.ToInt16(Convert.ToDouble(tmb) * 1.55);
                    break;

                case "Alta":
                    calorias = Convert.ToInt16(Convert.ToDouble(tmb) * 1.725);
                    break;

                case "Muy alta":
                    calorias = Convert.ToInt16(tmb * 1.9);
                    break;

                default:
                    calorias = 0;
                    break;
            }

            return calorias;
        }
    }
}