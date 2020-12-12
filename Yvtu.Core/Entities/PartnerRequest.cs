using System;
using System.Collections.Generic;
using System.Text;

namespace Yvtu.Core.Entities
{
    public class PartnerRequest
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public string MobileNo { get; set; }
        public int RequestId { get; set; }
        public string RequestName { get; set; }
        public string Shortcode { get; set; }
        public string Content { get; set; }
        public DateTime ReplayTime { get; set; }
        public string ReplayDesc { get; set; }
        public int Status { get; set; }
        public int QueueNo { get; set; }
        public string Error { get; set; }
        public string AccessChannel { get; set; }


    }
}
