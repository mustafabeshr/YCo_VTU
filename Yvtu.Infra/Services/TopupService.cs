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
        private readonly IPartnerManager _partnerManager;

        public TopupService(IAppDbContext db, IPartnerManager partnerManager)
        {
            this._db = db;
            _partnerManager = partnerManager;
        }
        public async Task<string> DoRecharge(RechargeCollection recharge, string endpoint, string apiUser, 
            string apiPassword, string remoteAddress, string successCode)
        {
            //Thread.Sleep(2000);
            var watch = System.Diagnostics.Stopwatch.StartNew();
            int resultCode;
            string transNo = "0";
            string resultDesc;
            #region Local test
            //resultCode = int.Parse(successCode);
            //resultDesc = "success";
            //System.Threading.Thread.Sleep(2000);
            #endregion Local test
            var paymentValue = new PaymentValuesRepo(_db, _partnerManager).GetSingleOrDefault(recharge.Amount);
            double profileId = 0;
            if (paymentValue != null)
            {
                profileId = paymentValue.ProfileId;
            }
            BasicHttpBinding httpBinding = new BasicHttpBinding();
            EndpointAddress p = new EndpointAddress(endpoint);

            //Start Send Request------------------------------ -
            var payService = new OCSService.CBSInterfaceAccountMgrClient(httpBinding, p);
                /// payService = SharedParams.OCSEndpoint;
                OCSService.PaymentRequestMsg msg = new OCSService.PaymentRequestMsg();
                OCSService.RequestHeader reqheader = new OCSService.RequestHeader();

                reqheader.SessionEntity = new OCSService.SessionEntityType();
                reqheader.SessionEntity.Name = apiUser;
                reqheader.SessionEntity.Password = apiPassword;
                reqheader.TransactionId = $"VTU{recharge.Id}";
                reqheader.SequenceId = "1";
                reqheader.CommandId = "Payment";
                reqheader.Version = "1";
                reqheader.SerialNo = $"VTU{recharge.Id}";
                reqheader.RequestType = OCSService.RequestHeaderRequestType.Event;
                reqheader.SessionEntity.RemoteAddress = remoteAddress;
                msg.RequestHeader = reqheader;
                msg.PaymentRequest = new OCSService.PaymentRequest();
                msg.PaymentRequest.SubscriberNo = recharge.SubscriberNo;
                msg.PaymentRequest.PaymentAmt = Convert.ToInt64(recharge.Amount) * 100;
                msg.PaymentRequest.PaymentMode = "1000";
                msg.PaymentRequest.TransactionCode = $"VTU{recharge.Id}";
                msg.PaymentRequest.RefillProfileID = profileId.ToString();
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
            //watch = System.Diagnostics.Stopwatch.StartNew();
            var dbResult = new RechargeRepo(_db, null).UpdateWithBalance(collection);
            if (collection.Status.Id == 1)
            {
                new NotificationRepo(_db, _partnerManager).SendNotification("Recharge.Create", recharge.Id, recharge, 1);
            }
            
            //watch.Stop();
            //elapsedMs = watch.ElapsedMilliseconds;
            return JsonSerializer.Serialize( new  {
                               resultCode = resultCode == int.Parse(successCode) ? 0 : resultCode,
                               resultDesc = resultDesc,
                               sequence = transNo,
                               payId = recharge.Id,
                               duration =  elapsedMs });
        }
    }
}
