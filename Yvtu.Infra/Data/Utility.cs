﻿using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

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


        public static bool ValidYMobileNo(string m)
        {
            Match match = Regex.Match(m, @"^70\d*", RegexOptions.IgnoreCase);
            if (match.Success)
                if (m.Length == 9)
                    return true;
                else
                    return false;
            else
                return false;

        }

        public static string RemoveSpecialChar(string value)
        {
            string NewValue = Regex.Replace(value, @"/[|!#$%&()=?»«@£§€{};<'>_""]", "");
            return NewValue;
        }

        public static bool IsValidDate(string dateValue)
        {
            DateTime Temp;

            if (DateTime.TryParseExact(dateValue, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out Temp)) return true;

            return false;
        }
    }

   public class MonyToString
    {
        //مصفوفات الكلمات
        string[] ahad = { "", "واحد", "إثنين", "ثلاثة", "أربعة", "خمسة", "ستة", "سبعة", " ثمانية", " تسعة", " عشرة", " أحد", " اثنى" };
        string[] ahad2 = { "", "واحد", "إثنين", "ثلاثة", "أربعة", "خمسة", "ستة", "سبعة", "ثمانية", "تسعة", " عشر", " أحد", " اثنى" };
        string[] asharat = { "", "واحد", "عشرون", "ثلاثون", "أربعون", "خمسون", "ستون", "سبعون", "ثمانون", "تسعون" };
        string[] meat = { "", "مائة", "مائتين", "ثلاثمائة", "أربعمائة", "خمسمائة", "ستمائة", "سبعمائة", "ثمانمائة", "تسعمائة" };
        string[] melion = { "", " مليون", " مليونان", " ملايين" };
        string[] alf = { "", " ألف", " ألفين", " آلاف" };
        string[] bcur = { " ريال", " ريالان", " ريالات" };

        public string NumToStr(double P_Num)
        {
            double rv;
            string accum = "";
            //الملايين
            rv = (int)(P_Num / 1000000);

            if (rv > 2)
                accum = NumToStr1(rv, accum);

            if (rv >= 3 && rv < 10)
                accum = accum + melion[3];
            else if (rv == 2)
                accum = accum + melion[2];
            else if ((rv == 1) || (rv >= 10 && rv <= 999))
                accum = accum + melion[1];
            //الآلاف
            rv = P_Num - (int)(P_Num / 1000000) * 1000000;
            rv = (int)(rv / 1000);
            if ((P_Num != ((int)(P_Num / 1000000)) * 1000000) && (P_Num > 1000000))
                accum = accum + " و";
            if (rv > 2)
                accum = NumToStr1(rv, accum);
            if (rv >= 3 && rv < 10)
                accum = accum + alf[3];
            else if (rv == 2)
                accum = accum + alf[2];
            else if ((rv == 1) || (rv >= 10 && rv <= 999))
                accum = accum + alf[1];
            //الباقي
            rv = P_Num - ((int)(P_Num / 1000)) * 1000;
            rv = (int)(rv + 0.0001);

            if ((P_Num != ((int)(P_Num / 1000)) * 1000) && (P_Num > 1000) && (rv != 0))
                accum = accum + " و ";

            if ((rv >= 2) && (P_Num != 2))
                accum = NumToStr1(rv, accum);

            if (P_Num > 0.999)
            {
                if ((P_Num < 11) && (rv > 2))
                    accum = accum + bcur[2];
                else if (P_Num == 2)
                    accum = accum + bcur[1];
                else
                    accum = accum + bcur[0];
            }

            //الافلاس 
            rv = P_Num - ((int)(P_Num + 0.0001)) + 0.0001;
            rv = (int)(rv * 1000);
            rv = rv / 10;

            if ((rv >= 1) && (P_Num > 0.99))
                accum = accum + " و";

            if (rv > 2.9)
                accum = NumToStr1(rv, accum);

            if (rv >= 1)
            {
                if ((rv == 2))
                    accum = accum + " فلسين";
                else if ((rv < 11) && (rv > 2.9))

                    accum = accum + " افلاس";
                else
                    accum = accum + " فلس";
            }
            return accum;
        }

        //******************* NumToStr1 *************************
        // used by NmToStr
        public string NumToStr1(double rv, string accum)
        {
            int b, c;
            if (rv >= 100)
            {
                b = (int)(rv / 100);
                accum = accum + meat[b];
            }

            b = (int)(rv - ((int)(rv / 100) * 100));
            if ((b != 0) && (rv > 99))
                accum = accum + " و";

            c = b - ((int)(b / 10) * 10);
            if ((b < 13) && (b != 0))
                accum = accum + ahad[b];

            if ((b > 12) && (c != 0))
                accum = accum + ahad2[c];
            if ((b > 10) && (b < 20))
                accum = accum + ahad2[10];

            if (b > 19)
            {
                if (c != 0)
                    accum = accum + " و";
                accum = accum + asharat[b / 10];
            }
            return accum;
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

    public static class ObjectCopier
    {
        /// <summary>
        /// Perform a deep Copy of the object, using Json as a serialization method. NOTE: Private members are not cloned using this method.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T CloneJson<T>(this T source)
        {
            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            // initialize inner objects individually
            // for example in default constructor some list property initialized with some values,
            // but in 'source' these items are cleaned -
            // without ObjectCreationHandling.Replace default constructor values will be added to result
            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings);
        }
    }

    public static class ReflectionHelper
    {
        public static dynamic GetPropValue(this Object obj, String propName)
        {
            string[] nameParts = propName.Split('.');
            if (nameParts.Length == 1)
            {
                var found = obj.GetType().GetProperty(propName);
                if (found != null)
                {
                    return obj.GetType().GetProperty(propName).GetValue(obj, null);
                }else
                {
                    return null;
                }
                
            }

            foreach (String part in nameParts)
            {
                if (obj == null) { return null; }

                Type type = obj.GetType();
                PropertyInfo info = type.GetProperty(part);
                if (info == null) { return null; }

                obj = info.GetValue(obj, null);
            }
            return obj;
        }

    }
    
    
}
