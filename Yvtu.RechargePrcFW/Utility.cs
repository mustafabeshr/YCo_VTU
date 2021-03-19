using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Yvtu.RechargePrcFW
{
    class Utility
    {
        public static string GetKeyValueFromConfigFile(string p_name)
        {
            AppSettingsReader settingsReader = new AppSettingsReader();
            string key = (string)settingsReader.GetValue(p_name, typeof(String));
            return key;
        }

     

        public static string ConvertFromDoubleListToString(List<double> doubleList)
        {
            var result = string.Empty;
            foreach (var item in doubleList)
            {
                if (result.Length > 0)
                {
                    result += "|" + item.ToString("N0");
                }
                else
                {
                    result += item.ToString("N0");
                }
            }

            return result;
        }

     
        public static bool ValidRegEx(string value, string role)
        {
            Regex rg = new Regex(role);
            return rg.IsMatch(value);
        }
        public static bool ValidMobileNo(string m)
        {
            Regex rgx = new Regex("^77[0-9]{7}");
            return rgx.IsMatch(m);
        }
        public static bool ContainsOnlyNumbers(string value)
        {
            Match match = Regex.Match(value, "^[0-9]+$", RegexOptions.IgnoreCase);
            if (match.Success)
                return true;
            else
                return false;

        }
        public static string Encrypt(string toEncrypt, bool useHashing, string key)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            System.Configuration.AppSettingsReader settingsReader = new AppSettingsReader();
            // Get the key from config file

            //key = (string)settingsReader.GetValue("SecurityKey", typeof(String));
            //string key = "Mustafa Moh Beshr";
            //System.Windows.Forms.MessageBox.Show(key);
            //If hashing use get hashcode regards to your key
            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                //Always release the resources and flush data
                // of the Cryptographic service provide. Best Practice

                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes.
            //We choose ECB(Electronic code Book)
            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)

            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            //transform the specified region of bytes array to resultArray
            byte[] resultArray =
              cTransform.TransformFinalBlock(toEncryptArray, 0,
              toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor
            tdes.Clear();
            //Return the encrypted data into unreadable string format
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        public static string Decrypt(string cipherString, bool useHashing, string key)
        {
            byte[] keyArray;
            //get the byte code of the string

            byte[] toEncryptArray = Convert.FromBase64String(cipherString);

            System.Configuration.AppSettingsReader settingsReader =
                                                new AppSettingsReader();
            //Get your key from config file to open the lock!
            //key = (string)settingsReader.GetValue("SecurityKey", typeof(String));
            //string key = "Mustafa Moh Beshr";
            if (useHashing)
            {
                //if hashing was used get the hash code with regards to your key
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                //release any resource held by the MD5CryptoServiceProvider

                hashmd5.Clear();
            }
            else
            {
                //if hashing was not implemented get the byte code of the key
                keyArray = UTF8Encoding.UTF8.GetBytes(key);
            }

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes. 
            //We choose ECB(Electronic code Book)

            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(
                                 toEncryptArray, 0, toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor                
            tdes.Clear();
            //return the Clear decrypted TEXT
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
       
        public static string GetRandom8Digits()
        {
            var bytes = new byte[4];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            uint random = BitConverter.ToUInt32(bytes, 0) % 100000000;
            return String.Format("{0:D8}", random);
        }
        public static string GetRandom4Digits()
        {
            var bytes = new byte[4];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            uint random = BitConverter.ToUInt32(bytes, 0) % 10000;
            return String.Format("{0:D4}", random);
        }
      
        private static string SHA1(string phrase)
        {
            byte[] hash;
            using (var sha1 = new System.Security.Cryptography.SHA1CryptoServiceProvider())
            {
                hash = sha1.ComputeHash(Encoding.Unicode.GetBytes(phrase));
                var sb = new StringBuilder();
                foreach (byte b in hash) sb.AppendFormat("{0:x2}", b);
                return sb.ToString();
            }
        }
     
        public static void WritingLOGToFILE(string strline)
        {

            string folderpath = AppDomain.CurrentDomain.BaseDirectory + @"DetailsLOG\" + DateTime.Now.ToString("yyyyMMdd");
            bool exists = System.IO.Directory.Exists(folderpath);
            if (!exists) System.IO.Directory.CreateDirectory(folderpath);
            folderpath = folderpath + @"\OpLog" + "_" + "hr" + DateTime.Now.ToString("HH") + ".txt";
            string lc = strline.Substring(strline.Length - 1, 1);
            if (!File.Exists(folderpath))
            {
                using (StreamWriter sw = File.CreateText(folderpath))
                {
                    //if ( lc== @"\n")
                    sw.WriteLine(DateTime.Now.ToString("yyyyMMddHHmmssfff") + " " + strline);
                    //else
                    //   sw.Write(strline);
                }
            }
            else
            {
                StreamWriter sw = File.AppendText(folderpath);
                sw.WriteLine(strline);
                sw.Close();
            }
        }
        public static int GenerateNewPinCode()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }
        #region ...solve redundancy message
        public static bool isRedundancyMessage(string mobileNo, string text, DateTime reqTime)
        {
            return false;
        //    try
        //    {
        //        string strsql = "select * from " + SharedParams.DBName + ".sms_request " + Environment.NewLine
        //                        + " where client_message='" + text + "' and c_mobile= '" + mobileNo +
        //                        "' and req_time <= '" + reqTime.AddSeconds(3).ToString("yyyy-MM-dd HH:mm:ss") +
        //                        "' and req_time >= '" + reqTime.AddSeconds(-SharedParams.redundancyPeriod).ToString("yyyy-MM-dd HH:mm:ss") + "' and command_id<>98";
        //        DataTable dtMessage = DB.GetDataTable(strsql);
        //        if (!object.ReferenceEquals(dtMessage, null))
        //        {
        //            if (dtMessage.Rows.Count > 1)
        //            {
        //                return true;
        //            }
        //            else
        //            {
        //                return false;
        //            }

        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        }
        #endregion

    }
}
