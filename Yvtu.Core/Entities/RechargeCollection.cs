using System;
using System.Collections.Generic;
using System.Text;

namespace Yvtu.Core.Entities
{
    public class RechargeCollection
    {
        public RechargeCollection()
        {
            PointOfSale = new Partner();
            AccessChannel = new CommonCode();
            Status = new PairValue();
        }
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public string SubscriberNo { get; set; }
        public double Amount { get; set; }
        public Partner PointOfSale { get; set; }
        public CommonCode AccessChannel { get; set; }
        public PairValue Status { get; set; }
        public DateTime StatusTime { get; set; }
        public int QueueNo { get; set; }
        public string RefNo { get; set; }
        public string RefMessage { get; set; }
        public string RefTransNo { get; set; }
        public DateTime RefTime { get; set; }
        public string DebugInfo { get; set; }
    }

    public partial class ToExcelSchema
    {
        public class RechargeCollection 
        {
            public int Id { get; set; }
            public DateTime CreatedOn { get; set; }
            public string SubscriberNo { get; set; }
            public double Amount { get; set; }
            public string POSId { get; set; }
            public string POSName { get; set; }
            public string POSRoleName { get; set; }
            public int POSAccount { get; set; }
            public double POSBalance { get; set; }
            public string AccessChannel { get; set; }
            public string Status { get; set; }
            public DateTime StatusTime { get; set; }
            public int QueueNo { get; set; }
            public string RefNo { get; set; }
            public string RefMessage { get; set; }
            public string RefTransNo { get; set; }
            public DateTime RefTime { get; set; }
            public string DebugInfo { get; set; }
        }
    }
}

