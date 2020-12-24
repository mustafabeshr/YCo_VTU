using System;
using System.Collections.Generic;
using System.Text;

namespace Yvtu.Core.Entities
{
    public class PartnerStatusLog
    {
        public PartnerStatusLog()
        {
            CreatedBy = new AppUser();
            Partner = new Partner();
            OldStatus = new PartnerStatus();
            NewStatus = new PartnerStatus();
        }
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public AppUser CreatedBy { get; set; }
        public Partner Partner { get; set; }
        public PartnerStatus OldStatus { get; set; }
        public PartnerStatus NewStatus { get; set; }
        public string Note { get; set; }
        public DateTime NewStatusExpireOn { get; set; }
    }
}
