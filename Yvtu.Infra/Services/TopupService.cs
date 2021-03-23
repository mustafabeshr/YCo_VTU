using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.Infra.Services
{
    public class TopupService
    {
        private readonly IAppDbContext _db;
        public TopupService(IAppDbContext db)
        {
            this._db = db;
        }
        public async Task<string> DoRecharge(RechargeCollection recharge, string endpoint, string apiUser, 
            string apiPassword, string remoteAddress, string successCode)
        {
            //Thread.Sleep(2000);
            #region Local test

            #endregion Local test
            var watch = System.Diagnostics.Stopwatch.StartNew();
            BasicHttpBinding httpBinding = new BasicHttpBinding();
            EndpointAddress p = new EndpointAddress(endpoint);

            //Start Send Request------------------------------ -
            var payService = new OCSService.CBSInterfaceAccountMgrClient(httpBinding, p);
            int resultCode;
            string transNo;
            string resultDesc;
            {
                /// payService = SharedParams.OCSEndpoint;
                OCSService.PaymentRequestMsg msg = new OCSService.PaymentRequestMsg();
                OCSService.RequestHeader reqheader = new OCSService.RequestHeader();

                reqheader.SessionEntity = new OCSService.SessionEntityType();
                reqheader.SessionEntity.Name = apiUser;
                reqheader.SessionEntity.Password = apiPassword;
                reqheader.TransactionId = "";
                reqheader.SequenceId = "1";
                reqheader.CommandId = "Payment";
                reqheader.Version = "1";
                reqheader.SerialNo = "";
                reqheader.RequestType = OCSService.RequestHeaderRequestType.Event;
                reqheader.SessionEntity.RemoteAddress = remoteAddress;
                msg.RequestHeader = reqheader;
                msg.PaymentRequest = new OCSService.PaymentRequest();
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
                        resultCode = int.Parse(requestResult.PaymentResultMsg.ResultHeader.ResultCode);
                        resultDesc = requestResult.PaymentResultMsg.ResultHeader.ResultDesc;
                        transNo = requestResult.PaymentResultMsg.PaymentResult.InternalSerialNo;
                    }
                    else
                    {
                        resultCode = -1001;
                        resultDesc = "OCS return null";
                        transNo = "0";
                    }
                }
                catch (Exception ex)
                {
                    resultCode = -1000;
                    resultDesc = ex.GetType().Name + "-" + ex.Message;
                    transNo = "0";
                }
            }
            //}
            // End Send Request -------------------------------
            watch.Stop();
            double elapsedMs = watch.ElapsedMilliseconds;
            var collection = new RechargeCollection();
            collection.Id = recharge.Id;
            collection.Status.Id = resultCode == int.Parse(successCode) ? 1 : 2;
            collection.RefNo = resultCode.ToString();
            collection.RefMessage = resultDesc;
            collection.RefTransNo = transNo;
            collection.RefTime = DateTime.Now;
            collection.DebugInfo = resultDesc + " OCS(" + elapsedMs + ")";
            elapsedMs = 0;
            //watch = System.Diagnostics.Stopwatch.StartNew();
            var dbResult = new RechargeRepo(_db, null).UpdateWithBalance(collection);
            //watch.Stop();
            //elapsedMs = watch.ElapsedMilliseconds;
            return JsonSerializer.Serialize( new  {
                               resultCode = resultCode == int.Parse(successCode) ? 0 : resultCode,
                               resultDesc = resultDesc,
                               Sequence = transNo,
                               Duration =  elapsedMs });
        }
    }
}
