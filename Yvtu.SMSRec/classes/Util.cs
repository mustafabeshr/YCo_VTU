using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Yvtu.SMSRec
{
    class Util
    {
        public static string GetKeyValueFromConfigFile(string p_name)
        {
            AppSettingsReader settingsReader = new AppSettingsReader();
            string key = (string)settingsReader.GetValue(p_name, typeof(String));
            return key;
        }
        public static string RemoveSpecialChar(string value)
        {
            string NewValue = Regex.Replace(value, @"/[|!#$%&()=?»«@£§€{};<'>_""]", "");
            return NewValue;
        }
        public static Interface ReadInterfaceFromXmlFile(string filename)
        {
            Interface inter;
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            XmlSerializer serialize = new XmlSerializer(typeof(Interface));
            try
            {
                inter = (Interface)serialize.Deserialize(fs);
                return inter;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                fs.Close();
            }
        }
        public static void LoadInterfaceDictionary()
        {
            
            SharedParams.InterfaceDictionary = new Dictionary<byte, Interface>();
            List<string> theList = Directory.GetFiles(SharedParams.Interfaces_Path, "*.xml", SearchOption.AllDirectories).ToList();
            foreach (string item in theList)
            {
                    Interface inter = Util.ReadInterfaceFromXmlFile(item);
                    if (inter != null)
                    {
                        if (!SharedParams.InterfaceDictionary.ContainsKey(inter.Interface_No))
                        {
                          if (SharedParams.Interfaces_MaxCount < 15)
                           {
                            SharedParams.InterfaceDictionary.Add(inter.Interface_No, inter);
                            SharedParams.Interfaces_MaxCount += 1;
                           }
                        }
                    }
            }
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
        public static bool OnlyNumber(char c, bool AllowDecimal, object sender)
        {
            bool res = false;
            if (!AllowDecimal)

                if (!char.IsControl(c) && !char.IsDigit(c))
                    res = true;
                else
                    res = false;

            else
            {
                if (!char.IsControl(c) && !char.IsDigit(c) && (c != '.'))
                    res = true;
                else
                    res = false;


                // only allow one decimal point
                int s = (sender as TextBox).Text.IndexOf('.');
                if ((c == '.') && (s == -1))
                    res = false;

            }
            return res;
        }
        public static string GetRandom8Digits()
        {
            var bytes = new byte[4];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            uint random = BitConverter.ToUInt32(bytes, 0) % 100000000;
            return String.Format("{0:D8}", random);
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
      
    }
}
