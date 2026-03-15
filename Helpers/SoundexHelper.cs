using System.Collections.Generic;

namespace ColegioPrivado.Helpers
{
    public static class SoundexHelper
    {
        public static string Generar(string palabra)
        {
            if (string.IsNullOrEmpty(palabra))
                return "";

            palabra = palabra.ToUpper();

            char primeraLetra = palabra[0];

            var mapa = new Dictionary<char, char>
            {
                {'B','1'}, {'F','1'}, {'P','1'}, {'V','1'},
                {'C','2'}, {'G','2'}, {'J','2'}, {'K','2'}, {'Q','2'}, {'S','2'}, {'X','2'}, {'Z','2'},
                {'D','3'}, {'T','3'},
                {'L','4'},
                {'M','5'}, {'N','5'},
                {'R','6'}
            };

            string resultado = primeraLetra.ToString();

            char ultimoCodigo = '0';

            for (int i = 1; i < palabra.Length; i++)
            {
                if (mapa.ContainsKey(palabra[i]))
                {
                    char codigo = mapa[palabra[i]];

                    if (codigo != ultimoCodigo)
                    {
                        resultado += codigo;
                        ultimoCodigo = codigo;
                    }
                }
            }

            resultado = resultado.PadRight(4, '0');

            return resultado.Substring(0, 4);
        }
    }
}
