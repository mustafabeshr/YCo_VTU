using System;
using System.Collections.Generic;
using System.Text;

namespace Yvtu.Core.Entities
{
    public class SMSOne
    {
        public SMSOne()
        {
            CreatedBy = new AppUser();
        }
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public AppUser CreatedBy { get; set; }
        public string Receiver { get; set; }
        public string Shortcode { get; set; }
        public string Message { get; set; }
        public string Note { get; set; }
    }
}
