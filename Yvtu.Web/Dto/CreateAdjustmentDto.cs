using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Yvtu.Core.Entities;

namespace Yvtu.Web.Dto
{
    public class CreateAdjustmentDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "يجب تحديد رقم عملية نقل الرصيد ")]
        public int MoneyTransferId { get; set; }
        public MoneyTransfer OriginTrans { get; set; }
        public Partner SrcPartner { get; set; }
        public Partner DestPartner { get; set; }
        [Required(ErrorMessage = "يجب ادخال المبلغ")]
        public double Amount { get; set; }
        public double TaxPercent { get; set; }
        public double TaxAmount { get; set; }
        public double BonusPercent { get; set; }
        public double BounsAmount { get; set; }
        public double BounsTaxPercent { get; set; }
        public double BounsTaxAmount { get; set; }
        public double FixedFactor { get; set; }
        public double ReceivedAmount { get; set; }
        public double NetAmount { get; set; }
        public string Note { get; set; }
        public int PeriodDays { get; set; }
        public int SettingsPeriodDays { get; set; }
        public string Error { get; set; }
    }
}
