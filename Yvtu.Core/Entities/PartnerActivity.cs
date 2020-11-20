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
            MaxQueryDuration = new CommonCode();
            Scope = new CommonCode();
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
        public CommonCode MaxQueryDuration { get; set; }
        public CommonCode Scope { get; set; }
        public int MaxQueryRows { get; set; }
        public bool OnlyPartnerChildren { get; set; }
        public AppUser CreatedBy { get; set; }
        public DateTime  CreatedOn { get; set; }
        public DateTime  LastEditOn { get; set; }

        public override string ToString()
        {
            var content = "Id=" + Id + Environment.NewLine +
                "Activity Id=" + Activity.Id + Environment.NewLine +
                "Activity Name=" + Activity.Name + Environment.NewLine +
                "From Role Id=" + FromRole.Id + Environment.NewLine +
                "From Role Name=" + FromRole.Name + Environment.NewLine +
                "To Role Id=" + ToRole.Id + Environment.NewLine +
                "To Role Name=" + ToRole.Name + Environment.NewLine +
                "Check Balance=" + CheckBalanceRequired + Environment.NewLine +
                "Max Amount=" + MaxValue.ToString("N0") + Environment.NewLine +
                "Min Amount=" + MinValue.ToString("N0") + Environment.NewLine +
                "Tax=" + TaxPercent.ToString("N2") + Environment.NewLine +
                "Bonus=" + BonusPercent.ToString("N2") + Environment.NewLine +
                "Bouns Tax=" + BonusTaxPercent.ToString("N2") + Environment.NewLine +
                "Max Query Duration Id=" + MaxQueryDuration.Id + Environment.NewLine +
                "Max Query Duration Name=" + MaxQueryDuration.Name + Environment.NewLine +
                "Scope Id=" + Scope.Id + Environment.NewLine +
                "Scope Name=" + Scope.Name + Environment.NewLine +
                "Max Query Rows=" + MaxQueryRows + Environment.NewLine +
                "Created On=" + CreatedOn.ToString("yyyy/MM/dd H:mm:ss") + Environment.NewLine +
                "Created By=" + CreatedBy.Id + Environment.NewLine +
                "Last Edit On=" + LastEditOn.ToString("yyyy/MM/dd H:mm:ss");
            return content;
        }
    }
}
