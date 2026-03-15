using System.Security.Cryptography;
using System.Text;

namespace ColegioPrivado.Security
{
    public class PasswordHelper
    {
        public static string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }

            return Convert.ToBase64String(saltBytes);
        }

        // MÉTODO ORIGINAL
        public static string GenerateHash(string password, string salt)
        {
            string passwordSalt = password + salt;

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(passwordSalt);
                byte[] hash = sha256.ComputeHash(bytes);

                return Convert.ToBase64String(hash);
            }
        }

        // MÉTODO COMPATIBLE CON LOGINCONTROLLER
        public static string HashPassword(string password, string salt)
        {
            return GenerateHash(password, salt);
        }

        public static bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
        {
            string hash = GenerateHash(enteredPassword, storedSalt);

            return hash == storedHash;
        }
    }
}