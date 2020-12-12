using System;
using System.Collections.Generic;
using System.Text;

namespace Yvtu.Core.Entities
{
    public class SMSOut
    {
        public int Id { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Message { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
