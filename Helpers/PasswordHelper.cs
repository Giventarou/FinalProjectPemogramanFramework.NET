using System;
using System.Security.Cryptography;

namespace DailyFinanceApp
{
    // Mengacak password sebelum disimpan, dan mencocokkannya saat login.
    public static class PasswordHelper
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 10000;

        // Hasilnya berbentuk "salt:hash" (dua-duanya dalam Base64)
        public static string Hash(string password)
        {
            byte[] salt = new byte[SaltSize];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
            {
                byte[] hash = pbkdf2.GetBytes(HashSize);
                return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
            }
        }

        public static bool Verify(string password, string stored)
        {
            if (string.IsNullOrEmpty(stored)) return false;

            string[] parts = stored.Split(':');
            if (parts.Length != 2) return false;

            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] expected = Convert.FromBase64String(parts[1]);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
            {
                byte[] actual = pbkdf2.GetBytes(HashSize);

                if (actual.Length != expected.Length) return false;

                int diff = 0;
                for (int i = 0; i < actual.Length; i++)
                {
                    diff |= actual[i] ^ expected[i];
                }
                return diff == 0;
            }
        }
    }
}