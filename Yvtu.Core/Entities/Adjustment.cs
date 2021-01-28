using System;
using System.Collections.Generic;
using System.Text;

namespace Yvtu.Core.Entities
{
    public class Adjustment
    {
        public Adjustment()
        {
            CreatedBy = new AppUser();
            SrcPartner = new Partner();
            DestPartner = new Partner();
            AccessChannel = new CommonCode();
            MoneyTransfer = new MoneyTransfer();
        }
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public AppUser CreatedBy { get; set; }
        public int MoneyTransferId { get; set; }
        public MoneyTransfer MoneyTransfer { get; set; }
        public Partner SrcPartner { get; set; }
        public Partner DestPartner { get; set; }
        public double Amount { get; set; }
        public double TaxPercent { get; set; }
        public double TaxAmount { get; set; }
        public double BonusPercent { get; set; }
        public double BounsAmount { get; set; }
        public double BounsTaxPercent { get; set; }
        public double BounsTaxAmount { get; set; }
        public double ReceivedAmount { get; set; }
        public double NetAmount { get; set; }
        public CommonCode AccessChannel { get; set; }
        public string Note { get; set; }
        public int PeriodDays { get; set; }
        public int SettingsPeriodDays { get; set; }
    }
}
