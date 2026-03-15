using System;

namespace ColegioPrivado.Security
{
    public static class CodigoHelper
    {
        public static string GenerarCodigo()
        {
            Random rnd = new Random();
            return rnd.Next(100000, 999999).ToString();
        }
    }
}