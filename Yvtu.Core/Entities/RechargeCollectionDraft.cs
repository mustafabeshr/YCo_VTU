using System;
using System.Collections.Generic;
using System.Text;

namespace Yvtu.Core.Entities
{
    public class RechargeCollectionDraft
    {
        public int Id { get; set; }
        public int MasterId { get; set; }
        public int QueueNo { get; set; }
        public string AccessChannelId { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
