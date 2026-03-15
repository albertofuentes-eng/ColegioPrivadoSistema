using System;
using System.Linq;

namespace ColegioPrivado.Helpers
{
    public static class PasswordGenerator
    {
        public static string GenerarPasswordTemporal()
        {
            string caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789@$*";
            Random random = new Random();

            return new string(Enumerable.Repeat(caracteres, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}