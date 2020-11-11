using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Yvtu.Infra.Data
{
    public class Utility
    {
        public static string GenerateNewCode(int CodeLength)
        {
            Random random = new Random();
            StringBuilder output = new StringBuilder();

            for (int i = 0; i < CodeLength; i++)
            {
                output.Append(random.Next(0, 9));
            }
            return output.ToString();
        }
    }


    public static class Pbkdf2Hasher
    {
        public static string ComputeHash(string password, byte[] salt)
        {
            return Convert.ToBase64String(
              KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
              )
            );
        }

        public static byte[] GenerateRandomSalt()
        {
            byte[] salt = new byte[128 / 8];

            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(salt);

            return salt;
        }
    }
}
