using System;
using System.Collections.Generic;
using System.Text;

namespace Yvtu.Core.Entities
{
    public class SMSIn
    {
        public int Id { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Message { get; set; }
        public DateTime CreatedOn { get; set; }
        public int RefNo { get; set; }
        public string Lang { get; set; }
    }
}
