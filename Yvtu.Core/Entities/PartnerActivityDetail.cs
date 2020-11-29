using System;
using System.Collections.Generic;
using System.Text;

namespace Yvtu.Core.Entities
{
    public class PartnerActivityDetail
    {
        public PartnerActivityDetail()
        {
            ToRole = new Role();
            CreatedBy = new AppUser();
        }
        public int Id { get; set; }
        public int ParentId  { get; set; }
        public Role ToRole { get; set; }
        public bool CheckBalanceRequired { get; set; }
        public int MaxValue { get; set; }
        public int MinValue { get; set; }
        public double BonusPercent { get; set; }
        public double BonusTaxPercent { get; set; }
        public double TaxPercent { get; set; }
        public AppUser CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastEditOn { get; set; }

        public virtual PartnerActivity Parent { get;set; }
    }
}
