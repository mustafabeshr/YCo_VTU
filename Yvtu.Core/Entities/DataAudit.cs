using System;
using System.Collections.Generic;
using System.Text;

namespace Yvtu.Core.Entities
{
    public class DataAudit
    {
        public DataAudit()
        {
            Activity = new Activity();
            Action = new CommonCode();
        }
        public int Id { get; set; }
        public string PartnerId { get; set; }
        public int PartnerAccount { get; set; }
        public string PartnerName { get; set; }
        public DateTime CreatedOn { get; set; }
        public Activity Activity { get; set; }
        public CommonCode Action { get; set; }
        public string Note { get; set; }
        public string SystemNote { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string Error { get; set; }
        public bool Success { get; set; }
    }
}
