using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Yvtu.RechargePrcFW.lib
{
    public class Notifications
    {
        public void SendRecharge(string actId, GrappedRecharge data)
        {
            var POSBalance = new RechargeRepo().GetBalance(data.PointOfSaleAccount);
            var messages = new MessageTemplateRepo().GetMessages(actId);
            if (messages != null && messages.Count > 0)
            {
                foreach (var msgObj in messages)
                {
                    var msg = msgObj.Message;
                    msg = msg.Replace("{amount}", data.Amount.ToString("N2"));
                    msg = msg.Replace("{mobile.1}", data.PointOfSaleId);
                    msg = msg.Replace("{mobile.2}", data.SubscriberNo);
                    msg = msg.Replace("{balance.1}", POSBalance.ToString("N2"));

                    var sendResult = new SMSOutRepo().Create(
                        new Entities.SMSOut()
                        {
                            Message = msg,
                            Receiver = (msgObj.ToWho == 1 ? data.PointOfSaleId : data.SubscriberNo),
                            Sender = "4444"
                        });
                }
            }
        }
    }
}
