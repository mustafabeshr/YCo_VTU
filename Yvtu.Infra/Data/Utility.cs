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


        public static string HowMuchLeftTime(DateTime futime)
        {
            string timemessage = string.Empty;
            double seconds = (futime - DateTime.Now).TotalSeconds;
            TimeSpan time = TimeSpan.FromSeconds(seconds);
            if (time.Hours > 0)
            {
                if (time.Hours > 2 && time.Hours < 11)
                {
                    timemessage = time.Hours.ToString() + " ساعات ";
                }
                else if (time.Hours == 1)
                {
                    timemessage = " ساعة ";
                }
                else if (time.Hours == 2)
                {
                    timemessage = " ساعتين ";
                }
                else
                {
                    timemessage = time.Hours.ToString() + " ساعة ";
                }
            }
            if (time.Minutes > 0)
            {
                if (time.Minutes > 2 && time.Minutes < 11)
                {
                    if (!string.IsNullOrEmpty(timemessage))
                    {
                        timemessage += " و " + time.Minutes.ToString() + " دقائق ";
                    }
                    else
                    {
                        timemessage = time.Minutes.ToString() + " دقائق ";
                    }
                }
                else if (time.Minutes == 1)
                {
                    if (!string.IsNullOrEmpty(timemessage))
                    {
                        timemessage += " و دقيقة";
                    }
                    else
                    {
                        timemessage = " دقيقة ";
                    }
                }
                else if (time.Minutes == 2)
                {
                    if (!string.IsNullOrEmpty(timemessage))
                    {
                        timemessage += " ودقيقتين ";
                    }
                    else
                    {
                        timemessage = " دقيقتين ";
                    }

                }
                else
                {
                    if (!string.IsNullOrEmpty(timemessage))
                    {
                        timemessage += " و " + time.Minutes.ToString() + " دقيقة ";
                    }
                    else
                    {
                        timemessage = time.Minutes.ToString() + " دقيقة ";
                    }
                }
            }
            if (time.Seconds > 0)
            {
                if (time.Seconds > 2 && time.Seconds < 11)
                {
                    if (!string.IsNullOrEmpty(timemessage))
                    {
                        timemessage += " و " + time.Seconds.ToString() + " ثواني ";
                    }
                    else
                    {
                        timemessage = time.Seconds.ToString() + " ثواني ";
                    }
                }
                else if (time.Seconds == 1)
                {
                    if (!string.IsNullOrEmpty(timemessage))
                    {
                        timemessage += " وثانية ";
                    }
                    else
                    {
                        timemessage = " ثانية ";
                    }

                }
                else if (time.Seconds == 2)
                {
                    if (!string.IsNullOrEmpty(timemessage))
                    {
                        timemessage += " وثانيتين ";
                    }
                    else
                    {
                        timemessage = " ثانيتين ";
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(timemessage))
                    {
                        timemessage += " و " + time.Seconds.ToString() + " ثانية ";
                    }
                    else
                    {
                        timemessage = time.Seconds.ToString() + " ثانية ";
                    }
                }
            }
            return timemessage;
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
