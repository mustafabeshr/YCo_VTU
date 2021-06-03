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
        public async Task<RechargeCollection> DoRecharge(RechargeCollection recharge, string endpoint, string apiUser, 
            string apiPassword, string remoteAddress, string successCode)
        {
            //Thread.Sleep(2000);
           
            int resultCode;
            string transNo = recharge.ApiTransaction.ToString();
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
            else
            {
                profileId = Convert.ToInt64(recharge.Amount) * 100;
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
            
            recharge.Status.Id = resultCode == int.Parse(successCode) ? 1 : 2;
            recharge.RefNo = resultCode.ToString();
            recharge.RefMessage = resultDesc;
            recharge.RefTransNo = transNo;
            recharge.RefTime = DateTime.Now;
            
            //watch = System.Diagnostics.Stopwatch.StartNew();
            return recharge;
            
            
            //watch.Stop();
            //elapsedMs = watch.ElapsedMilliseconds;
            //return JsonSerializer.Serialize( new  {
            //                   resultCode = resultCode == int.Parse(successCode) ? 0 : resultCode,
            //                   resultDesc = resultDesc,
            //                   sequence = transNo,
            //                   payId = recharge.Id,
            //                   duration =  elapsedMs });
        }
    }
}
