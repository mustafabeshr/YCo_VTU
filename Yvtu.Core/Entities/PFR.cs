using System;
using System.Collections.Generic;
using System.Text;

namespace Yvtu.Core.Entities
{
    public class PFR
    {
        public PFR()
        {
            CreatedBy = new AppUser();
        }
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ActivityTime { get; set; }
        public AppUser CreatedBy { get; set; }
        public string PartnerId { get; set; }
        public int PartnerAccount { get; set; }
        public double Amount { get; set; }
        public double Balance { get; set; }
        public string ActivityId { get; set; }
        public string ActivityName { get; set; }
        public int TransNo { get; set; }

    }
}
