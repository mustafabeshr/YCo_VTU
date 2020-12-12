using System;
using System.Data;
using System.Text;
using Yvtu.Core.Entities;

namespace Yvtu.SMSRec
{
    class Security
    {
        private readonly IRecDbContext db;

        public Security(IRecDbContext db)
        {
            this.db = db;
        }
        public  string IsAuthonticatedClient(Partner Client,string PINCode,out DateTime LockedExpireTime)
        {
            LockedExpireTime = DateTime.MinValue;
            return string.Empty;
            //try
            //{
            //    if (Client.Status.Id == 1)
            //    {
            //        var isCorrectPass = new Repo.PartnerManager(db).CheckPass(Client, PINCode);
            //        if (isCorrectPass)
            //        {
            //            LockedExpireTime = DateTime.MinValue;
            //            return "ok";
            //        }
            //    }
                
            //        string Result = string.Empty;
            //    DateTime LockExpireTime=DateTime.MinValue;
            //    bool pass = ClientPin(PINCode, Client_id);
            //    client.Client_Id = Client_id;
            //    if (client.getClientById())
            //    {
            //        #region Active status 
            //        if (client.Status=="active" )
            //        {
            //            if (client.PIN_Code == pass)
            //            {
            //                LockExpireTime = DateTime.MinValue;
            //                Result= "ok";
            //            }
            //            else
            //            {
            //                if (client.Attempts>=SharedParams.WrongPasswordMaxAttempts)
            //                {
            //                    if (client.Lock_Client())
            //                    {
            //                        LockExpireTime = WhenLockedTimeExpired(DateTime.Now);
            //                        client.RestAttempts();
            //                        Result = "WrongPasswordThenLocked";
            //                    }
            //                    else
            //                    {
            //                        LockExpireTime = DateTime.MinValue;
            //                        client.Inc_Attempts();
            //                        Result = "WrongPassword";
            //                    }
            //                }
            //                else
            //                {
            //                    //LockExpireTime = DateTime.MinValue;
            //                    client.Inc_Attempts();
            //                    Result = "WrongPassword";
            //                }
            //            }
            //        }
            //        #endregion

            //        #region Locked status 
            //        else if (client.Status == "locked")
            //        {
            //            LockExpireTime = WhenLockedTimeExpired(client.Lock_Time);
            //            if (LockExpireTime > DateTime.Now)
            //            {
            //                Result = "locked";
            //            }
            //            else
            //            {
            //                if (client.PIN_Code == pass)
            //                {
            //                    LockExpireTime = DateTime.MinValue;
            //                    client.Lock_Cancel();
            //                    Result = "ok";
            //                }
            //                else
            //                {
            //                    if (client.Attempts >= SharedParams.WrongPasswordMaxAttempts)
            //                    {
            //                        if (client.Lock_Client())
            //                        {
            //                            client.Lock_Time = DateTime.Now;
            //                            LockExpireTime = WhenLockedTimeExpired(client.Lock_Time);
            //                            client.RestAttempts();
            //                            Result = "WrongPasswordThenLocked";
            //                        }
            //                        else
            //                        {
            //                            LockExpireTime = DateTime.MinValue;
            //                            client.Inc_Attempts();
            //                            Result = "WrongPassword";
            //                        }
            //                    }
            //                    else
            //                    {
            //                        //LockExpireTime = DateTime.MinValue;
            //                        client.Inc_Attempts();
            //                        Result = "WrongPassword";
            //                    }
            //                }
            //            }
            //        }
            //        #endregion

            //        #region pause status 
            //        else if (client.Status == "pause")
            //        {
            //            Result = "pause";
            //        }
            //        #endregion
            //    }
            //    LockedExpireTime = LockExpireTime;
            //    return Result;
            //}
            //catch (Exception ex)
            //{
            //    LockedExpireTime = DateTime.MinValue;
            //    return "Error";
            //}
        }
        private  DateTime WhenLockedTimeExpired(DateTime LockTime)
        {
            int duration = int.Parse(SharedParams.LockedDuration.Substring(0, SharedParams.LockedDuration.Length - 1));
            string durationName = SharedParams.LockedDuration.Substring(SharedParams.LockedDuration.Length - 1, 1);
            switch (durationName.ToLower())
            {
                case "m":
                    LockTime = LockTime.AddMinutes(duration);
                    break;
                case "h":
                    LockTime = LockTime.AddHours(duration);
                    break;
                case "d":
                    LockTime = LockTime.AddDays(duration);
                    break;
                default:
                    LockTime = LockTime.AddHours(duration);
                    break;
            }

            return LockTime;
        }
      
        private  string SHA1(string phrase)
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
    }
}
