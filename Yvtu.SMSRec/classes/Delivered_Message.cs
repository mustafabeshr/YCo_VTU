using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.SMSRec
{
    partial class DeliverMessage
    {
        private readonly IAppDbContext db;
        private readonly IPartnerManager partnerManager;
        private readonly IPartnerActivityRepo partnerActivityRepo;
        private readonly ILogger<frm_Parent> _logger;

        public DeliverMessage(IAppDbContext db, IPartnerManager partnerManager, 
            IPartnerActivityRepo partnerActivityRepo, ILogger<frm_Parent> logger)
        {
            this.db = db;
            this.partnerManager = partnerManager;
            this.partnerActivityRepo = partnerActivityRepo;
            _logger = logger;
        }
        // Write your code in this class to process the request ...
        public RequestReturnValue Parse_Request(Delivered_Message ClientMessage, byte queueNo, out PartnerRequest parsedRequest)
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
                    
                    if (Tokens[0] == "ت" || Tokens[0].ToLower() == "t")
                    {
                        #region Money Transfer
                        if (Tokens.Length < 4)
                        {
                            ret.Ret_ID = -1;
                            ret.Ret_Message = "REQUEST_TOO_SHORT";
                            ret.Ret_Message_to_Client = "عذرا طلب نقل رصيد غير مكتمل ، يرجى التأكد و المحاولة مرة اخرى";
                            ret.Ret_Status = false;
                            parsedRequest.RequestId = 2;
                            return ret;
                        }
                        if (!Util.ValidRegEx(Tokens[1], SharedParams.AmountPattern))
                        {
                            ret.Ret_ID = -1;
                            ret.Ret_Message = "INVALID_AMOUNT";
                            ret.Ret_Message_to_Client = "المبلغ غير صحيح ، يرجى التاكد و المحاولة لاحقا";
                            ret.Ret_Status = false;
                            parsedRequest.RequestId = 2;
                            return ret;
                        }
                        if (!Util.ValidRegEx(Tokens[2], SharedParams.MobileNumberPattern))
                        {
                            ret.Ret_ID = -1;
                            ret.Ret_Message = "WRONG_MOBILE";
                            ret.Ret_Message_to_Client = "رقم المشترك غير صحيح ، يرجى التاكد و المحاولة لاحقا";
                            ret.Ret_Status = false;
                            parsedRequest.RequestId = 2;
                            return ret;
                        }
                        if (!Util.ValidRegEx(Tokens[3], SharedParams.PINCodePattern))
                        {
                            ret.Ret_ID = -1;
                            ret.Ret_Message = "INVALID_PIN_CODE";
                            ret.Ret_Message_to_Client = "الرقم السري غير صحيح ، يرجى التاكد و المحاولة لاحقا";
                            ret.Ret_Status = false;
                            parsedRequest.RequestId = 2;
                            return ret;
                        }
                        if (Tokens[2] == ClientMessage.Mobile_No)
                        {
                            ret.Ret_ID = -1;
                            ret.Ret_Message = "TransferToHimself";
                            ret.Ret_Message_to_Client = "عذرا لا يمكنك نقل رصيد الى نفسك";
                            ret.Ret_Status = false;
                            parsedRequest.RequestId = 2;
                            return ret;
                        }
                        var partnerResult = partnerManager.Validate(ClientMessage.Mobile_No);
                        
                        if (!partnerResult.Success)
                        {
                            ret.Ret_ID = -1;
                            ret.Ret_Message = "Not_Found";
                            ret.Ret_Message_to_Client = "عذرا ليس لديك حساب بالخدمة";
                            ret.Ret_Status = false;
                            parsedRequest.RequestId = 2;
                            return ret;
                        }
                        else if (partnerResult.Partner.Status.Id != 1)
                        {
                            ret.Ret_ID = -1;
                            ret.Ret_Message = "Wrong_State";
                            ret.Ret_Message_to_Client = "عذرا لايمكنك اجراء هذا الطلب ، حالة حسابك )" + partnerResult.Partner.Status.Name + "(";
                            ret.Ret_Status = false;
                            parsedRequest.RequestId = 3;
                            return ret;
                        }
                        var partner = partnerResult.Partner;
                        //if (partner.Balance < double.Parse(Tokens[1]))
                        //{
                        //    ret.Ret_ID = -1;
                        //    ret.Ret_Message = "NotEnoughBalance";
                        //    ret.Ret_Message_to_Client = "رصيدك غير كافي لاجراء عملية نقل رصيد";
                        //    ret.Ret_Status = false;
                        //    parsedRequest.RequestId = 2;
                        //    return ret;
                        //}
                        var targetPartnerResult = partnerManager.Validate(Tokens[2]);
                        if (!targetPartnerResult.Success)
                        {
                            ret.Ret_ID = -1;
                            ret.Ret_Message = "Another_Partner_Not_Found";
                            ret.Ret_Message_to_Client = "عذرا الرقم "+ Tokens[2] + "المراد نقل الرصيد اليه ليس لديه حساب";
                            ret.Ret_Status = false;
                            parsedRequest.RequestId = 2;
                            return ret;
                        }
                        var targetPartner = targetPartnerResult.Partner;
                        var permission = partnerActivityRepo.GetPartAct("MoneyTransfer.Create", partner.Role.Id, targetPartner.Role.Id);
                        if (permission == null)
                        {
                            ret.Ret_ID = -1;
                            ret.Ret_Message = "Unauthorized";
                            ret.Ret_Message_to_Client = "عذرا ليس لديك الصلاحية الكافية";
                            ret.Ret_Status = false;
                            parsedRequest.RequestId = 2;
                            return ret;
                        }
                        if (permission.Details == null)
                        {
                            ret.Ret_ID = -1;
                            ret.Ret_Message = "Unauthorized";
                            ret.Ret_Message_to_Client = "عذرا ليس لديك الصلاحية الكافية لنقل رصيد لهذه الجهة";
                            ret.Ret_Status = false;
                            parsedRequest.RequestId = 2;
                            return ret;
                        }
                        if (permission.Details[0].MinValue > 0 && int.Parse(Tokens[1]) < permission.Details[0].MinValue)
                        {
                            ret.Ret_ID = -1;
                            ret.Ret_Message = "AmountLessThanMinimum";
                            ret.Ret_Message_to_Client = "عذرا المبلغ المراد نقله اقل من الحد الادنى " + permission.Details[0].MinValue.ToString("N0");
                            ret.Ret_Status = false;
                            parsedRequest.RequestId = 2;
                            return ret;
                        }
                        if (permission.Details[0].MaxValue > 0 &&int.Parse(Tokens[1]) > permission.Details[0].MaxValue)
                        {
                            ret.Ret_ID = -1;
                            ret.Ret_Message = "AmountMoreThanMaximum";
                            ret.Ret_Message_to_Client = "عذرا المبلغ المراد نقله اكبر من الحد الاعلى  " + permission.Details[0].MaxValue.ToString("N0");
                            ret.Ret_Status = false;
                            parsedRequest.RequestId = 2;
                            return ret;
                        }
                        #region Client weather authonticated or not  
                        if (partner.Status.Id == 1)
                        {
                            var isCorrectPass = partnerManager.CheckPass(partner, Tokens[3]);
                            if (isCorrectPass)
                            {
                                #region Do Money Transfer
                                var moneyTransfer = new MoneyTransfer();
                                moneyTransfer.Partner = partnerManager.GetPartnerByAccount(targetPartner.Account);
                                moneyTransfer.PayType.Id = "cash";
                                //moneyTransfer.PayNo = "0";
                                //moneyTransfer.PayBank = "";
                                //moneyTransfer.PayDate = DateTime.MinValue;
                                moneyTransfer.CreatedBy = partnerManager.GetPartnerByAccount(partner.Account);
                                moneyTransfer.AccessChannel.Id = "sms";
                                moneyTransfer.Amount = double.Parse(Tokens[1]);
                                //moneyTransfer.BillNo = "0";
                                //moneyTransfer.RequestNo = "0";
                                moneyTransfer.RequestAmount = double.Parse(Tokens[1]);
                                //moneyTransfer.Note = "";
                                var result = new MoneyTransferRepo(db, partnerManager, partnerActivityRepo).Create(moneyTransfer);
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
                                    moneyTransfer.Partner.Balance += moneyTransfer.Amount;
                                    moneyTransfer.CreatedBy.Balance -= moneyTransfer.Amount;
                                    new NotificationRepo(db, partnerManager).SendNotification<MoneyTransfer>("MoneyTransfer.Create", 
                                        result.AffectedCount, moneyTransfer);
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
                                    _logger.LogError($"mobile={parsedRequest.MobileNo},id={parsedRequest.Id},result={result.AffectedCount},result error={result.Error}");
                                    ret.Ret_ID = -1;
                                    ret.Ret_Message = "UnExpectedError";
                                    ret.Ret_Message_to_Client = "عذرا لم تتم عملية نقل الرصيد لحدوث خطأ في السيرفر " + result.AffectedCount;
                                    ret.Ret_Status = false;
                                    return ret;
                                }
                                #endregion
                            } else
                            {
                                ret.Ret_ID = -1;
                                ret.Ret_Message = "WrongPass";
                                ret.Ret_Message_to_Client = "عذرا الرقم السري غير صحيح  ";
                                ret.Ret_Status = false;
                                parsedRequest.RequestId = 2;
                                return ret;
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
                            parsedRequest.RequestId = 3;
                            return ret;
                        }
                        if (!Util.ValidRegEx(Tokens[1], SharedParams.PINCodePattern))
                        {
                            ret.Ret_ID = -1;
                            ret.Ret_Message = "INVALID_PIN_CODE";
                            ret.Ret_Message_to_Client = "الرقم السري غير صحيح ، يرجى التاكد و المحاولة لاحقا";
                            ret.Ret_Status = false;
                            parsedRequest.RequestId = 3;
                            return ret;
                        }
                        var partnerResult = partnerManager.Validate(ClientMessage.Mobile_No);
                        
                        if (!partnerResult.Success)
                        {
                            ret.Ret_ID = -1;
                            ret.Ret_Message = "Not_Found";
                            ret.Ret_Message_to_Client = "عذرا ليس لديك حساب بالخدمة";
                            ret.Ret_Status = false;
                            parsedRequest.RequestId = 3;
                            return ret;
                        } else if (partnerResult.Partner.Status.Id != 1)
                        {
                            ret.Ret_ID = -1;
                            ret.Ret_Message = "Wrong_State";
                            ret.Ret_Message_to_Client = "عذرا لايمكنك اجراء هذا الطلب ، حالة حسابك )"+ partnerResult.Partner.Status.Name+ "(";
                            ret.Ret_Status = false;
                            parsedRequest.RequestId = 3;
                            return ret;
                        }
                        var partner = partnerResult.Partner;
                        var permission = partnerActivityRepo.GetPartAct("Balance.QueryBySMS", partner.Role.Id);
                        if (permission == null)
                        {
                            ret.Ret_ID = -1;
                            ret.Ret_Message = "Unauthorized";
                            ret.Ret_Message_to_Client = "عذرا ليس لديك الصلاحية الكافية";
                            ret.Ret_Status = false;
                            parsedRequest.RequestId = 3;
                            return ret;
                        }
                        var isCorrectPass = partnerManager.CheckPass(partner, Tokens[1]);
                        if (isCorrectPass)
                        {
                            var balance = partner.Balance - (partner.Reserved < 0 ? 0 : partner.Reserved);
                            ret.Ret_ID = 1;
                            ret.Ret_Status = true;
                            parsedRequest.Id = 1;
                            parsedRequest.RequestId = 3;
                            parsedRequest.RequestName = "Balance Query";
                            parsedRequest.Status = 1;
                            parsedRequest.MobileNo = partner.Id;
                            parsedRequest.ReplayDesc = "رصيد الشاحن الفوري هو " + balance.ToString("N0");


                            var result = new OutSMSRepo(db).Create(new SMSOut
                            {
                                Receiver = parsedRequest.MobileNo,
                                Message = "رصيد الشاحن الفوري الخاص بك هو " + balance.ToString("N0")
                            });

                            return ret;
                        }
                        else
                        {
                            ret.Ret_ID = -1;
                            ret.Ret_Message = "WrongPass";
                            ret.Ret_Message_to_Client = "عذرا الرقم السري غير صحيح  ";
                            ret.Ret_Status = false;
                            parsedRequest.RequestId = 3;
                            return ret;
                        }

                        #endregion
                    }
                    else if (Util.ContainsOnlyNumbers(ClientMessage.Message.Replace(" ",string.Empty)))
                    {
                        #region Recharge
                        if (Tokens.Length < 3)
                        {
                            ret.Ret_ID = -1;
                            ret.Ret_Message = "REQUEST_TOO_SHORT";
                            ret.Ret_Message_to_Client = "عذرا طلب شحن رصيد غير مكتمل ، يرجى التأكد و المحاولة مرة اخرى";
                            ret.Ret_Status = false;
                            parsedRequest.RequestId = 1;
                            return ret;
                        }
                        if (!Util.ValidRegEx(Tokens[0], SharedParams.AmountPattern))
                        {
                            ret.Ret_ID = -1;
                            ret.Ret_Message = "INVALID_AMOUNT";
                            ret.Ret_Message_to_Client = "المبلغ غير صحيح ، يرجى التاكد و المحاولة لاحقا";
                            ret.Ret_Status = false;
                            parsedRequest.RequestId = 1;
                            return ret;
                        }
                        if (!Util.ValidRegEx(Tokens[1], SharedParams.MobileNumberPattern))
                        {
                            ret.Ret_ID = -1;
                            ret.Ret_Message = "WRONG_MOBILE";
                            ret.Ret_Message_to_Client = "رقم المشترك غير صحيح ، يرجى التاكد و المحاولة لاحقا";
                            ret.Ret_Status = false;
                            parsedRequest.RequestId = 1;
                            return ret;
                        }
                        if (!Util.ValidRegEx(Tokens[2], SharedParams.PINCodePattern))
                        {
                            ret.Ret_ID = -1;
                            ret.Ret_Message = "INVALID_PIN_CODE";
                            ret.Ret_Message_to_Client = "الرقم السري غير صحيح ، يرجى التاكد و المحاولة لاحقا";
                            ret.Ret_Status = false;
                            parsedRequest.RequestId = 1;
                            return ret;
                        }
                        var partnerResult = partnerManager.Validate(ClientMessage.Mobile_No);
                        
                        if (!partnerResult.Success)
                        {
                            ret.Ret_ID = -1;
                            ret.Ret_Message = "Not_Found";
                            ret.Ret_Message_to_Client = "عذرا ليس لديك حساب بالخدمة";
                            ret.Ret_Status = false;
                            parsedRequest.RequestId = 1;
                            return ret;
                        }
                        else if (partnerResult.Partner.Status.Id != 1)
                        {
                            ret.Ret_ID = -1;
                            ret.Ret_Message = "Wrong_State";
                            ret.Ret_Message_to_Client = "عذرا لايمكنك اجراء هذا الطلب ، حالة حسابك )" + partnerResult.Partner.Status.Name + "(";
                            ret.Ret_Status = false;
                            parsedRequest.RequestId = 3;
                            return ret;
                        }
                        var partner = partnerResult.Partner;
                        if (partner.Balance < double.Parse(Tokens[0]))
                        {
                            ret.Ret_ID = -1;
                            ret.Ret_Message = "NotEnoughBalance";
                            ret.Ret_Message_to_Client = "رصيدك غير كافي لاجراء عملية شحن رصيد";
                            ret.Ret_Status = false;
                            parsedRequest.RequestId = 1;
                            return ret;
                        }
                        var permission = partnerActivityRepo.GetPartAct("Recharge.Create", partner.Role.Id, 9);
                        if (permission == null)
                        {
                            ret.Ret_ID = -1;
                            ret.Ret_Message = "Unauthorized";
                            ret.Ret_Message_to_Client = "عذرا ليس لديك الصلاحية الكافية";
                            ret.Ret_Status = false;
                            parsedRequest.RequestId = 1;
                            return ret;
                        }
                        if (permission.Details == null)
                        {
                            ret.Ret_ID = -1;
                            ret.Ret_Message = "Unauthorized";
                            ret.Ret_Message_to_Client = "عذرا ليس لديك الصلاحية الكافية لشحن رصيد لهذا الرقم";
                            ret.Ret_Status = false;
                            parsedRequest.RequestId = 1;
                            return ret;
                        }

                        var paymentAmounts = new AppGlobalSettingsRepo(db).GetSingle("PaymentAmount");
                        if (paymentAmounts != null && paymentAmounts.SettingValue == "list")
                        {
                            var amt = double.Parse(Tokens[0]);
                            var amtValues = new PaymentValuesRepo(db, partnerManager).GetAll();
                            if (amtValues != null)
                            {
                                if (!amtValues.Any(x => x.PayValue == amt))
                                {
                                    ret.Ret_ID = -1;
                                    ret.Ret_Message = "AmountNotInList";
                                    ret.Ret_Message_to_Client = "عذرا المبلغ المراد دفعه غير صحيح ، لمعرفة فئات المبالغ يرجى ارسال ف";
                                    ret.Ret_Status = false;
                                    parsedRequest.RequestId = 1;
                                    return ret;
                                }
                            }
                        }

                        if (permission.Details[0].MinValue > 0 && int.Parse(Tokens[0]) < permission.Details[0].MinValue)
                        {
                            ret.Ret_ID = -1;
                            ret.Ret_Message = "AmountLessThanMinimum";
                            ret.Ret_Message_to_Client = "عذرا المبلغ اقل من الحد الادنى " + permission.Details[0].MinValue.ToString("N0");
                            ret.Ret_Status = false;
                            parsedRequest.RequestId = 1;
                            return ret;
                        }
                        if (permission.Details[0].MaxValue > 0 && int.Parse(Tokens[0]) > permission.Details[0].MaxValue)
                        {
                            ret.Ret_ID = -1;
                            ret.Ret_Message = "AmountMoreThanMaximum";
                            ret.Ret_Message_to_Client = "عذرا المبلغ اكبر من الحد الاعلى  " + permission.Details[0].MaxValue.ToString("N0");
                            ret.Ret_Status = false;
                            parsedRequest.RequestId = 1;
                            return ret;
                        }
                        #region Client weather authonticated or not  
                        if (partner.Status.Id == 1)
                        {
                            var isCorrectPass = partnerManager.CheckPass(partner, Tokens[2]);
                            if (isCorrectPass)
                            {
                                #region Do Queue Recharge Request
                                var recharge = new RechargeCollection();
                                recharge.SubscriberNo = Tokens[1];
                                recharge.Amount = double.Parse(Tokens[0]);
                                recharge.PointOfSale = partner;
                                recharge.QueueNo = queueNo > 0 ? queueNo : 1;
                                recharge.AccessChannel.Id = "sms";
                                var result = new RechargeRepo(db, partnerManager).Create(recharge);
                                if (result.Success)
                                {
                                    ret.Ret_ID = result.AffectedCount;
                                    ret.Ret_Status = true;
                                    parsedRequest.Id = result.AffectedCount;
                                    parsedRequest.RequestId = 1;
                                    parsedRequest.RequestName = "Recharge";
                                    parsedRequest.MobileNo = partner.Id;
                                    parsedRequest.Shortcode = recharge.SubscriberNo;
                                    parsedRequest.ReplayDesc = recharge.Amount.ToString("N0");
                                    return ret;
                                }
                                else if (result.AffectedCount == -501)
                                {
                                    ret.Ret_ID = -1;
                                    ret.Ret_Message = "NotEnoughBalance";
                                    ret.Ret_Message_to_Client = "عذرا رصيدك غير كافي";
                                    ret.Ret_Status = false;
                                    return ret;
                                }
                                else if (result.AffectedCount == -512)
                                {
                                    ret.Ret_ID = -1;
                                    ret.Ret_Message = "PartnerIncorrectState";
                                    ret.Ret_Message_to_Client = "عذرا لايمكنك اجراء العملية بسبب الحالة";
                                    ret.Ret_Status = false;
                                    return ret;
                                }
                                else if (result.AffectedCount == -513)
                                {
                                    ret.Ret_ID = -1;
                                    ret.Ret_Message = "Unauthorized";
                                    ret.Ret_Message_to_Client = "عذرا ليس لديك الصلاحية الكافية";
                                    ret.Ret_Status = false;
                                    return ret;
                                }
                                else if (result.AffectedCount == -511)
                                {
                                    ret.Ret_ID = -1;
                                    ret.Ret_Message = "DontHaveAccount";
                                    ret.Ret_Message_to_Client = "عذرا ليس لديك حساب فعالا في الخدمة";
                                    ret.Ret_Status = false;
                                    return ret;
                                }
                                else if (result.AffectedCount == -515)
                                {
                                    ret.Ret_ID = -1;
                                    ret.Ret_Message = "AmountNotInList";
                                    ret.Ret_Message_to_Client = "عذرا المبلغ المراد دفعه غير صحيح ، لمعرفة فئات المبالغ يرجى ارسال ف";
                                    ret.Ret_Status = false;
                                    return ret;
                                }
                                #endregion
                            }
                            else
                            {
                                ret.Ret_ID = -1;
                                ret.Ret_Message = "WrongPass";
                                ret.Ret_Message_to_Client = "عذرا الرقم السري غير صحيح  ";
                                ret.Ret_Status = false;
                                parsedRequest.RequestId = 1;
                                return ret;
                            }
                        }
                        
                        #endregion
                        #endregion

                    } else if (ClientMessage.Message == "ف")
                    {
                        #region Query about payment values
                            var amountValues = new PaymentValuesRepo(db, partnerManager).GetAllPaymentValues();
                            if (amountValues != null)
                            {
                                var amountValuesString = String.Join(",", amountValues);
                                var msg = "فئات المبالغ المسموح بها" + Environment.NewLine + amountValuesString;
                                var result = new OutSMSRepo(db).Create(new SMSOut
                                {
                                    Receiver = parsedRequest.MobileNo,
                                    Message = msg
                                });
                                parsedRequest.RequestName = "Payment Values";
                                parsedRequest.Status = 1;
                                parsedRequest.MobileNo = ClientMessage.Mobile_No;
                                parsedRequest.ReplayDesc = msg;
                                ret.Ret_ID = 1;
                                ret.Ret_Status = true;
                                ret.Ret_Message = "OK";
                                ret.Ret_Message_to_Client = msg;
                                return ret;
                            }
                        #endregion
                    }
                    else
                    {
                        #region Unkown Request
                        ret.Ret_ID = -1;
                        ret.Ret_Message = "incorrect_request";
                        ret.Ret_Message_to_Client = "عذرا طلب غير معروف ";
                        ret.Ret_Status = false;
                        parsedRequest.RequestId = 1;
                        return ret;
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
                //ret.Ret_Message_to_Client = SharedParams.System_Messages["PROCESSING_FAULT"] + ch + ex.Message ;
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
