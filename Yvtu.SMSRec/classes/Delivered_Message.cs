using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Yvtu.Core.Entities;

namespace Yvtu.SMSRec
{
    partial class DeliverMessage
    {
        private readonly IRecDbContext db;

        public DeliverMessage(IRecDbContext db)
        {
            this.db = db;
        }
        // Write your code in this class to process the request ...
        public RequestReturnValue Parse_Request(Delivered_Message ClientMessage,out PartnerRequest parsedRequest)
        {
            RequestReturnValue ret = new RequestReturnValue();
            string ch = string.Empty;
            var reqPack = new PartnerRequest();
            reqPack.Content = ClientMessage.Message.Trim();
            reqPack.MobileNo = ClientMessage.Mobile_No;
            reqPack.QueueNo = ClientMessage.Channel;
            parsedRequest = reqPack;
            ClientMessage.Message = ClientMessage.Message.Trim();
            DateTime LockTimeExpire = DateTime.MinValue;
            try
            {
                #region check the min length of right request
                //if (ClientMessage.Message.Length < 5)
                //{
                //    ret.Ret_ID = -1;
                //    ret.Ret_Message = "REQUEST_TOO_SHORT";
                //    ret.Ret_Message_to_Client = SharedParams.System_Messages["REQUEST_TOO_SHORT"];
                //    ret.Ret_Status = false;
                //    return ret;
                //}
                //else
                #endregion
                {
                    if (ClientMessage.Message.EndsWith("#"))
                    {
                        ClientMessage.Message = ClientMessage.Message.Substring(0, ClientMessage.Message.Length - 1).Trim();
                    }
                    if (ClientMessage.Message.StartsWith("*"))
                    {
                        ClientMessage.Message = ClientMessage.Message.Substring(1, ClientMessage.Message.Length - 1).Trim();
                    }

                    ClientMessage.Message = Util.RemoveSpecialChar(ClientMessage.Message);

                    string[] Tokens = ClientMessage.Message.Split(SharedParams.TokensDelimiter);//Regex.Split(ClientMessage.Message, "*");
                    
                    for (int i=0;i<Tokens.Length;i++)
                    {
                        Tokens[i] = Tokens[i].Trim();
                    }
                    
                    if (Tokens[0] == "ت")
                    {
                        #region Money Transfer
                        if (Tokens.Length < 4)
                        {
                            ret.Ret_ID = -1;
                            ret.Ret_Message = "REQUEST_TOO_SHORT";
                            ret.Ret_Message_to_Client = "عذرا طلب نقل رصيد غير مكتمل ، يرجى التأكد و المحاولة مرة اخرى";
                            ret.Ret_Status = false;
                            return ret;
                        }
                        if (!Util.ValidRegEx(Tokens[1], SharedParams.AmountPattern))
                        {
                            ret.Ret_ID = -1;
                            ret.Ret_Message = "INVALID_AMOUNT";
                            ret.Ret_Message_to_Client = "المبلغ غير صحيح ، يرجى التاكد و المحاولة لاحقا";
                            ret.Ret_Status = false;
                            return ret;
                        }
                        if (!Util.ValidRegEx(Tokens[2], SharedParams.MobileNumberPattern))
                        {
                            ret.Ret_ID = -1;
                            ret.Ret_Message = "WRONG_MOBILE";
                            ret.Ret_Message_to_Client = "رقم المشترك غير صحيح ، يرجى التاكد و المحاولة لاحقا";
                            ret.Ret_Status = false;
                            return ret;
                        }
                        if (!Util.ValidRegEx(Tokens[3], SharedParams.PINCodePattern))
                        {
                            ret.Ret_ID = -1;
                            ret.Ret_Message = "INVALID_PIN_CODE";
                            ret.Ret_Message_to_Client = "الرقم السري غير صحيح ، يرجى التاكد و المحاولة لاحقا";
                            ret.Ret_Status = false;
                            return ret;
                        }
                        if (Tokens[2] == ClientMessage.Mobile_No)
                        {
                            ret.Ret_ID = -1;
                            ret.Ret_Message = "TransferToHimself";
                            ret.Ret_Message_to_Client = "عذرا لا يمكنك نقل رصيد الى نفسك";
                            ret.Ret_Status = false;
                            return ret;
                        }
                        var partner = new Repo.PartnerManager(db).GetActivePartner(ClientMessage.Mobile_No);
                        if (partner == null)
                        {
                            ret.Ret_ID = -1;
                            ret.Ret_Message = "Not_Found";
                            ret.Ret_Message_to_Client = "عذرا ليس لديك حساب بالخدمة";
                            ret.Ret_Status = false;
                            return ret;
                        }
                        if (partner.Balance < double.Parse(Tokens[1]))
                        {
                            ret.Ret_ID = -1;
                            ret.Ret_Message = "NotEnoughBalance";
                            ret.Ret_Message_to_Client = "رصيدك غير كافي لاجراء عملية نقل رصيد";
                            ret.Ret_Status = false;
                            return ret;
                        }
                        var targetPartner = new Repo.PartnerManager(db).GetActivePartner(Tokens[2]);
                        if (targetPartner == null)
                        {
                            ret.Ret_ID = -1;
                            ret.Ret_Message = "Another_Partner_Not_Found";
                            ret.Ret_Message_to_Client = "عذرا الرقم "+ Tokens[2] + "المراد نقل الرصيد اليه ليس لديه حساب";
                            ret.Ret_Status = false;
                            return ret;
                        }
                        var permission = new Repo.PartnerActivityRepo(db).GetPartAct("Money.Transfer", partner.Role.Id, targetPartner.Role.Id);
                        if (permission == null)
                        {
                            ret.Ret_ID = -1;
                            ret.Ret_Message = "Unauthorized";
                            ret.Ret_Message_to_Client = "عذرا ليس لديك الصلاحية الكافية";
                            ret.Ret_Status = false;
                            return ret;
                        }
                        if (permission.Details == null)
                        {
                            ret.Ret_ID = -1;
                            ret.Ret_Message = "Unauthorized";
                            ret.Ret_Message_to_Client = "عذرا ليس لديك الصلاحية الكافية لنقل رصيد لهذه الجهة";
                            ret.Ret_Status = false;
                            return ret;
                        }
                        if (permission.Details[0].MinValue > 0 && int.Parse(Tokens[1]) < permission.Details[0].MinValue)
                        {
                            ret.Ret_ID = -1;
                            ret.Ret_Message = "AmountLessThanMinimum";
                            ret.Ret_Message_to_Client = "عذرا المبلغ المراد نقله اقل من الحد الادنى " + permission.Details[0].MinValue.ToString("N0");
                            ret.Ret_Status = false;
                            return ret;
                        }
                        if (permission.Details[0].MaxValue > 0 &&int.Parse(Tokens[1]) > permission.Details[0].MaxValue)
                        {
                            ret.Ret_ID = -1;
                            ret.Ret_Message = "AmountMoreThanMaximum";
                            ret.Ret_Message_to_Client = "عذرا المبلغ المراد نقله اكبر من الحد الاعلى  " + permission.Details[0].MaxValue.ToString("N0");
                            ret.Ret_Status = false;
                            return ret;
                        }
                        #region Client weather authonticated or not  
                        if (partner.Status.Id == 1)
                        {
                            var isCorrectPass = new Repo.PartnerManager(db).CheckPass(partner, Tokens[3]);
                            if (isCorrectPass)
                            {
                                #region Do Money Transfer
                                var moneyTransfer = new MoneyTransfer();
                                moneyTransfer.Partner.Id = targetPartner.Id;
                                moneyTransfer.PayType.Id = "cash";
                                //moneyTransfer.PayNo = "0";
                                //moneyTransfer.PayBank = "";
                                //moneyTransfer.PayDate = DateTime.MinValue;
                                moneyTransfer.CreatedBy.Id = partner.Id;
                                moneyTransfer.AccessChannel.Id = "sms";
                                moneyTransfer.Amount = double.Parse(Tokens[1]);
                                //moneyTransfer.BillNo = "0";
                                //moneyTransfer.RequestNo = "0";
                                moneyTransfer.RequestAmount = double.Parse(Tokens[1]);
                                //moneyTransfer.Note = "";
                                var result = new Repo.MoneyTransferRepo(db).Create(moneyTransfer);
                                if (result.Success)
                                {
                                    ret.Ret_ID = result.AffectedCount;
                                    ret.Ret_Status = true;
                                    parsedRequest.Id = result.AffectedCount;
                                    parsedRequest.RequestId = 2;
                                    parsedRequest.RequestName = "Money Transfer";
                                    parsedRequest.MobileNo = partner.Id;
                                    parsedRequest.Shortcode = targetPartner.Id;
                                    parsedRequest.ReplayDesc = moneyTransfer.Amount.ToString("N0");
                                    return ret;
                                }
                                else if (result.AffectedCount == -500)
                                {
                                    ret.Ret_ID = -1;
                                    ret.Ret_Message = "Unauthorized";
                                    ret.Ret_Message_to_Client = "عذرا ليس لديك الصلاحية الكافية";
                                    ret.Ret_Status = false;
                                    return ret;
                                }
                                else if (result.AffectedCount == -501)
                                {
                                    ret.Ret_ID = -1;
                                    ret.Ret_Message = "NotEnoughBalance";
                                    ret.Ret_Message_to_Client = "عذرا الرصيد غير كافي";
                                    ret.Ret_Status = false;
                                    return ret;
                                }
                                else if (result.AffectedCount == -502)
                                {
                                    ret.Ret_ID = -1;
                                    ret.Ret_Message = "AmountLessThanMinimum";
                                    ret.Ret_Message_to_Client = "عذرا المبلغ المراد نقله اقل من الحد الادنى " + permission.Details[0].MinValue.ToString("N0");
                                    ret.Ret_Status = false;
                                    return ret;
                                }
                                else if (result.AffectedCount == -503)
                                {
                                    ret.Ret_ID = -1;
                                    ret.Ret_Message = "AmountMoreThanMaximum";
                                    ret.Ret_Message_to_Client = "عذرا المبلغ المراد نقله اكبر من الحد الاعلى  " + permission.Details[0].MaxValue.ToString("N0");
                                    ret.Ret_Status = false;
                                    return ret;
                                }
                                else if (result.AffectedCount == -506)
                                {
                                    ret.Ret_ID = -1;
                                    ret.Ret_Message = "NoActiveAccount";
                                    ret.Ret_Message_to_Client = "لم تتم عملية نقل رصيد بسبب حالة الحسابات ";
                                    ret.Ret_Status = false;
                                    return ret;
                                }
                                else if (result.AffectedCount == -507)
                                {
                                    ret.Ret_ID = -1;
                                    ret.Ret_Message = "BalanceError";
                                    ret.Ret_Message_to_Client = "لم تتم عملية نقل رصيد بسبب الارصدة ";
                                    ret.Ret_Status = false;
                                    return ret;
                                }
                                else if (result.AffectedCount < 0)
                                {
                                    ret.Ret_ID = -1;
                                    ret.Ret_Message = "UnExpectedError";
                                    ret.Ret_Message_to_Client = "عذرا لم تتم عملية نقل الرصيد لحدوث خطأ في السيرفر ";
                                    ret.Ret_Status = false;
                                    return ret;
                                }
                                #endregion
                            }
                        }
                        
                        #endregion
                        #endregion
                    }
                    else if (Tokens[0] == "ر" || Tokens[0].ToLower() == "b")
                    {
                        #region Balance Query
                        if (Tokens.Length < 2)
                        {
                            ret.Ret_ID = -1;
                            ret.Ret_Message = "REQUEST_TOO_SHORT";
                            ret.Ret_Message_to_Client = "عذرا لم يتم ارسال الرقم السري ، يرجى التأكد و المحاولة مرة اخرى";
                            ret.Ret_Status = false;
                            return ret;
                        }
                        if (!Util.ValidRegEx(Tokens[1], SharedParams.PINCodePattern))
                        {
                            ret.Ret_ID = -1;
                            ret.Ret_Message = "INVALID_PIN_CODE";
                            ret.Ret_Message_to_Client = "الرقم السري غير صحيح ، يرجى التاكد و المحاولة لاحقا";
                            ret.Ret_Status = false;
                            return ret;
                        }
                        var partner = new Repo.PartnerManager(db).GetActivePartner(ClientMessage.Mobile_No);
                        if (partner == null)
                        {
                            ret.Ret_ID = -1;
                            ret.Ret_Message = "Not_Found";
                            ret.Ret_Message_to_Client = "عذرا ليس لديك حساب بالخدمة";
                            ret.Ret_Status = false;
                            return ret;
                        }
                        var permission = new Repo.PartnerActivityRepo(db).GetPartAct("Balance.Query", partner.Role.Id);
                        if (permission == null)
                        {
                            ret.Ret_ID = -1;
                            ret.Ret_Message = "Unauthorized";
                            ret.Ret_Message_to_Client = "عذرا ليس لديك الصلاحية الكافية";
                            ret.Ret_Status = false;
                            return ret;
                        }
                        var isCorrectPass = new Repo.PartnerManager(db).CheckPass(partner, Tokens[1]);
                        if (isCorrectPass)
                        {
                            var balance = partner.Balance - (partner.Reserved < 0 ? 0 : partner.Reserved);
                            ret.Ret_ID = 1;
                            ret.Ret_Status = true;
                            parsedRequest.Id = 1;
                            parsedRequest.RequestId = 1;
                            parsedRequest.RequestName = "Balance Query";
                            parsedRequest.Status = 1;
                            parsedRequest.MobileNo = partner.Id;
                            parsedRequest.ReplayDesc = "رصيد الشاحن الفوري هو " + balance.ToString("N0");


                            var result = new Repo.OutSMSRepo(db).Create(new SMSOut
                            {
                                Receiver = parsedRequest.MobileNo,
                                Message = "رصيد الشاحن الفوري الخاص بك هو " + balance.ToString("N0")
                            });

                            return ret;
                        }

                        #endregion
                    }

                }
                parsedRequest = reqPack;
                return ret;
            }
            catch (Exception ex)
            {
                ret.Ret_ID = -1;
                ret.Ret_Message = "PROCESSING_FAULT";
                ret.Ret_Message_to_Client = SharedParams.System_Messages["PROCESSING_FAULT"] + ch + ex.Message ;
                ret.Ret_Status = false;
                parsedRequest = reqPack;
                return ret;
            }
        }

        string HowMuchLeftTime(DateTime futime)
        {
            string timemessage = string.Empty;
            double seconds=(futime - DateTime.Now).TotalSeconds;
            TimeSpan time = TimeSpan.FromSeconds(seconds);
            if (time.Hours>0)
            {
                if (time.Hours > 2 && time.Hours < 11)
                {
                    timemessage = time.Hours.ToString() + " ساعات ";
                }
                else if (time.Hours == 1)
                {
                    timemessage = " ساعة ";
                }
                else if (time.Hours==2)
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


}
