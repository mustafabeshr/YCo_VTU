using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yvtu.Core.Entities;
using Yvtu.Core.Queries;
using Yvtu.Infra.Data;
using Yvtu.Infra.Data.Interfaces;
using Yvtu.RechargePrc.Dtos;
using System.ServiceModel;

namespace Yvtu.RechargePrc.Ops
{
    public partial class Operations
    {
        public string resultDisplayMsg { get; set; }
        private readonly IAppDbContext db;

        public Operations(IAppDbContext db)
        {
            this.db = db;
        }
        public async Task<RechargeResponseDto> DoRecharge(GrappedRecharge recharge)
        {
            //Thread.Sleep(2000);
            #region Local test
            var result = new RechargeResponseDto();

            var watch = System.Diagnostics.Stopwatch.StartNew();
            BasicHttpBinding httpBinding = new BasicHttpBinding();
            EndpointAddress p = new EndpointAddress(SharedParams.OCSEndpoint);
           
            //Start Send Request------------------------------ -
            var payService = new CBSService.CBSInterfaceAccountMgrClient(httpBinding,p);
            {
               /// payService = SharedParams.OCSEndpoint;
                CBSService.PaymentRequestMsg msg = new CBSService.PaymentRequestMsg();
                CBSService.RequestHeader reqheader = new CBSService.RequestHeader();

                reqheader.SessionEntity = new CBSService.SessionEntityType();
                reqheader.SessionEntity.Name = SharedParams.OCSApiUser;
                reqheader.SessionEntity.Password = SharedParams.OCSApiPassword;
                reqheader.TransactionId = "";
                reqheader.SequenceId = "1";
                reqheader.CommandId = "Payment";
                reqheader.Version = "1";
                reqheader.SerialNo = "";
                reqheader.RequestType = CBSService.RequestHeaderRequestType.Event;
                reqheader.SessionEntity.RemoteAddress = SharedParams.PaymentRemoteAddress;
                msg.RequestHeader = reqheader;
                msg.PaymentRequest = new CBSService.PaymentRequest();
                msg.PaymentRequest.SubscriberNo = recharge.SubscriberNo;
                msg.PaymentRequest.PaymentAmt = Convert.ToInt64(recharge.Amount) * 100;
                msg.PaymentRequest.PaymentMode = "1000";
                msg.PaymentRequest.TransactionCode = $"VTU{recharge.Id}";
                msg.PaymentRequest.RefillProfileID = recharge.Amount.ToString();


                try
                {
                    var requestResult = await payService.PaymentAsync(msg);

                    if (requestResult != null)
                    {
                        result.ResultCode = requestResult.PaymentResultMsg.ResultHeader.ResultCode;
                        result.ResultDesc = requestResult.PaymentResultMsg.ResultHeader.ResultDesc;
                        result.TransNo = requestResult.PaymentResultMsg.PaymentResult.InternalSerialNo;
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
            //}
            // End Send Request -------------------------------
            //await HTTPClientWrapper<RechargeResponseDto>.Get("");
            watch.Stop();
            double elapsedMs = watch.ElapsedMilliseconds;
            var collection = new RechargeCollection();
            collection.Id = recharge.MasterId;
            collection.Status.Id = result.ResultCode == SharedParams.SuccessPaymentCode ? 1 : 2;
            collection.RefNo = result.ResultCode.ToString();
            collection.RefMessage = result.ResultDesc;
            collection.RefTransNo = result.TransNo;
            collection.RefTime = DateTime.Now;
            collection.DebugInfo = result.ResultDesc + " OCS(" + elapsedMs + ")";
            resultDisplayMsg = collection.DebugInfo;
            elapsedMs = 0;
            watch = System.Diagnostics.Stopwatch.StartNew();
            var dbResult = new RechargeRepo(db, null).UpdateWithBalance(collection);
            watch.Stop();
            elapsedMs = watch.ElapsedMilliseconds;
            resultDisplayMsg += " DB(" + elapsedMs + ")";
            return result;

            #endregion

        }
    }
}
