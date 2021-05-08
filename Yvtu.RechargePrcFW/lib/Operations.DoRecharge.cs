using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yvtu.RechargePrcFW.lib
{
    public partial class Operations
    {
        public string resultDisplayMsg { get; set; }

        public Operations()
        {
        }
        public RechargeResponseDto DoRecharge(GrappedRecharge recharge)
        {
            //Thread.Sleep(2000);
            #region Local test
            var result = new RechargeResponseDto();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            double elapsedMs = watch.ElapsedMilliseconds;
            var removeResult = new RechargeRepo().RemoveRechargeDraft(recharge.Id);
            if (removeResult)
            {
                //result.ResultCode = SharedParams.SuccessPaymentCode;
                //result.ResultDesc = "success";
                //System.Threading.Thread.Sleep(1000);
                #region Call OSC recharge service

                //Start Send Request------------------------------ -
                using (var payService = new RechargePrcFWFW.bank.CBSInterfaceAccountMgrService())
                {

                    payService.Url = SharedParams.OCSEndpoint;
                    RechargePrcFWFW.bank.PaymentRequestMsg msg = new RechargePrcFWFW.bank.PaymentRequestMsg();
                    RechargePrcFWFW.bank.RequestHeader reqheader = new RechargePrcFWFW.bank.RequestHeader();

                    reqheader.SessionEntity = new RechargePrcFWFW.bank.SessionEntityType();
                    reqheader.SessionEntity.Name = SharedParams.OCSApiUser;
                    reqheader.SessionEntity.Password = SharedParams.OCSApiPassword;
                    reqheader.TransactionId = $"VTU{recharge.MasterId}";
                    reqheader.SequenceId = "1";
                    reqheader.CommandId = "Payment";
                    reqheader.Version = "1";
                    reqheader.SerialNo = $"VTU{recharge.MasterId}";
                    reqheader.RequestType = RechargePrcFWFW.bank.RequestHeaderRequestType.Event;
                    reqheader.SessionEntity.RemoteAddress = SharedParams.PaymentRemoteAddress;
                    msg.RequestHeader = reqheader;
                    msg.PaymentRequest = new RechargePrcFWFW.bank.PaymentRequest();
                    msg.PaymentRequest.SubscriberNo = recharge.SubscriberNo;
                    msg.PaymentRequest.PaymentAmt = Convert.ToInt64(recharge.Amount) * 100;
                    msg.PaymentRequest.PaymentMode = "1000";
                    msg.PaymentRequest.TransactionCode = $"VTU{recharge.MasterId}";
                    msg.PaymentRequest.RefillProfileID = recharge.Amount.ToString();

                    try
                    {
                        var requestResult = payService.Payment(msg);

                        if (requestResult != null)
                        {
                            result.ResultCode = requestResult.ResultHeader.ResultCode;
                            result.ResultDesc = requestResult.ResultHeader.ResultDesc;
                            result.TransNo = requestResult.PaymentResult.InternalSerialNo;
                        }
                        else
                        {
                            result.ResultCode = "-1001";
                            result.ResultDesc = "OCS return null";
                        }
                    }
                    catch (Exception ex)
                    {
                        result.ResultCode = "-1000";
                        result.ResultDesc = ex.GetType().Name + "-" + ex.Message;
                    }
                }

                #endregion

                //}
                // End Send Request -------------------------------
                watch.Stop();
               
                var collection = new RechargeCollection();
                collection.Id = recharge.MasterId;
                collection.Status = result.ResultCode == SharedParams.SuccessPaymentCode ? 1 : 2;
                collection.RefNo = result.ResultCode.ToString();
                collection.RefMessage = result.ResultDesc;
                collection.RefTransNo = result.TransNo;
                collection.RefTime = DateTime.Now;
                collection.DebugInfo = result.ResultDesc + " OCS(" + elapsedMs + ")";
                resultDisplayMsg = collection.DebugInfo;
                elapsedMs = 0;
                watch = System.Diagnostics.Stopwatch.StartNew();
                var dbResult = new RechargeRepo().UpdateWithBalance(collection);
                if (collection.Status == 1)
                {
                   new Notifications().SendRecharge("Recharge.Create", recharge);
                }
                else
                {
                    var POSBalance = new RechargeRepo().GetBalance(recharge.PointOfSaleAccount);
                    var sendResult = new SMSOutRepo().Create(
                        new Entities.SMSOut()
                        {
                            Message = $"فشلت عملية شحن رصيد للرقم {recharge.SubscriberNo} بمبلغ {recharge.Amount.ToString("N2")} ر.ي رصيدك {POSBalance.ToString("N2")}",
                            Receiver = recharge.PointOfSaleId,
                            Sender = "4444"
                        });
                }
                watch.Stop();
                elapsedMs = watch.ElapsedMilliseconds;
                resultDisplayMsg += " DB(" + elapsedMs + ")";
                return result;

                #endregion
            }
            else
            {
                return new RechargeResponseDto()
                {
                    Duration = Convert.ToInt16(Math.Truncate(elapsedMs)),
                    ResultCode = "-10000",
                    ResultDesc = "Could not remove draft"
                };
            }

        }
    }
}
