using System.Net;
using System.Net.Mail;

namespace ColegioPrivado.Security
{
    public class EmailHelper
    {
        public static void EnviarCodigo(string destino, string codigo)
        {
            var mensaje = new MailMessage();

            mensaje.From = new MailAddress("josefuentesdeleon603@gmail.com");
            mensaje.To.Add(destino);

            mensaje.Subject = "Código de recuperación";
            mensaje.Body = "Su código de recuperación es: " + codigo;

            var smtp = new SmtpClient("smtp.gmail.com", 587);

            smtp.Credentials = new NetworkCredential(
                "josefuentesdeleon603@gmail.com",
                "chhenbdxshinwzwv"
            );

            smtp.EnableSsl = true;

            smtp.Send(mensaje);
        }
    }
}