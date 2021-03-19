using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yvtu.RechargePrcFW.lib
{
    public class GrappedRecharge
    {
        public int Id { get; set; }
        public int MasterId { get; set; }
        public string SubscriberNo { get; set; }
        public double Amount { get; set; }
        public string PointOfSaleId { get; set; }
        public int PointOfSaleAccount { get; set; }
        public string AccessChannelId { get; set; }
        public int Status { get; set; }
        public DateTime StatusTime { get; set; }
        public int QueueNo { get; set; }
        public string RefNo { get; set; }
        public string RefMessage { get; set; }
        public string RefTransNo { get; set; }
        public DateTime RefTime { get; set; }
        public string DebugInfo { get; set; }
    }
}
