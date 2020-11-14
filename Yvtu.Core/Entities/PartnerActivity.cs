using System;
using System.Collections.Generic;
using System.Text;

namespace Yvtu.Core.Entities
{
    public class PartnerActivity
    {
        public PartnerActivity()
        {
            FromRole = new Role();
            ToRole = new Role();
            Activity = new Activity();
            CreatedBy = new AppUser();
        }
        public int Id { get; set; }
        public Activity Activity { get; set; }
        public Role FromRole { get; set; }
        public Role ToRole { get; set; }
        public bool CheckBalanceRequired { get; set; }
        public int MaxValue { get; set; }
        public int MinValue { get; set; }
        public double BonusPercent { get; set; }
        public double BonusTaxPercent { get; set; }
        public double TaxPercent { get; set; }
        public string MaxQueryDuration { get; set; }
        public string Scope { get; set; }
        public int MaxQueryRows { get; set; }
        public bool OnlyPartnerChildren { get; set; }
        public AppUser CreatedBy { get; set; }
        public DateTime  CreatedOn { get; set; }
        public DateTime  LastEditOn { get; set; }
    }
}
