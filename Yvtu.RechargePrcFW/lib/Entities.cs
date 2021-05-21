using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yvtu.RechargePrcFW.lib
{
    public class Entities
    {
        public class MessageTemplate
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Message { get; set; }
            public int ToWho { get; set; }
        }
        public class SMSOut
        {
            public int Id { get; set; }
            public string Sender { get; set; }
            public string Receiver { get; set; }
            public string Message { get; set; }
        }

        public class PaymentValues
        {
            
            public int Seq { get; set; }
            public double PayValue { get; set; }
            public double ProfileId { get; set; }

            
        }
    }
}
