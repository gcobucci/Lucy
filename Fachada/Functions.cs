using System;

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
    }
}