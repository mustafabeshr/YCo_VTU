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
        public double FixedFactor { get; set; }
        public AppUser CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastEditOn { get; set; }

        public virtual PartnerActivity Parent { get;set; }

        public override string ToString()
        {
            var content = "Id =" + Id + Environment.NewLine +
                "Parent Id =" + ParentId + Environment.NewLine +
                "Check Balance Required =" + CheckBalanceRequired + Environment.NewLine +
                "To Role Id =" + ToRole.Id + Environment.NewLine +
                "To Role Name =" + ToRole.Name + Environment.NewLine +
                "Min Value=" + MinValue.ToString("N0") + Environment.NewLine +
                "Max Value =" + MaxValue.ToString("N0") + Environment.NewLine +
                "Tax Per =" + TaxPercent.ToString("N2") + Environment.NewLine +
                "Bonus Per =" + BonusPercent.ToString("N2") + Environment.NewLine +
                "Bonus Tax Per =" + BonusTaxPercent.ToString("N2") + Environment.NewLine +
                "Fixed Factor =" + FixedFactor.ToString("N6") + Environment.NewLine +
                "Created On=" + CreatedOn.ToString("yyyy/MM/dd H:mm:ss") + Environment.NewLine +
                "Created By=" + CreatedBy.Id + Environment.NewLine +
                "Created By Account=" + CreatedBy.Account + Environment.NewLine +
                "Last Edit On=" + LastEditOn.ToString("yyyy/MM/dd H:mm:ss");
            return content;
        }
    }
}
