using System;
using System.Collections.Generic;
using System.Text;

namespace Yvtu.Core.Entities
{
    public class MoneyTransfer
    {
        public MoneyTransfer()
        {
            Partner = new Partner();
            PayType = new CommonCode();
            AccessChannel = new CommonCode();
            CreatedBy = new Partner();
        }
        public int Id { get; set; }
        public Partner Partner { get; set; }
        public CommonCode PayType { get; set; }
        public string PayNo { get; set; }
        public DateTime PayDate { get; set; }
        public string PayBank { get; set; }
        public Partner CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public CommonCode AccessChannel { get; set; }
        public double Amount { get; set; }
        public double TaxPercent { get; set; }
        public double TaxAmount { get; set; }
        public double BonusPercent { get; set; }
        public double BounsAmount { get; set; }
        public double BounsTaxPercent { get; set; }
        public double BounsTaxAmount { get; set; }
        public double ReceivedAmount { get; set; }
        public double NetAmount { get; set; }
        public double RequestAmount { get; set; }
        public string BillNo { get; set; }
        public string RequestNo { get; set; }
        public string Note { get; set; }
        public bool Adjusted { get; set; }
        public int AdjustmentNo { get; set; }

    }
}
