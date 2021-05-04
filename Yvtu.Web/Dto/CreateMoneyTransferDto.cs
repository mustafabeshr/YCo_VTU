using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data.CustomeValidationAttribute;

namespace Yvtu.Web.Dto
{
    public class CreateMoneyTransferDto
    {
        public int Id { get; set; }
        public string OPType { get; set; }
        [Required(ErrorMessage = "يجب تحديد الجهة المراد نقل الرصيد اليها")]
        [StringLength(9, ErrorMessage = "يجب ان يكون طول الرقم 9 ارقام")]
        [RegularExpression(@"^70\d*", ErrorMessage = "رقم موبايل غير صحيح")]
        public string PartnerId { get; set; }
        
        public string PartnerName { get; set; }
        public string PartnerRoleName { get; set; }
        public int PartnerRoleId { get; set; }
        public double PartnerBalance { get; set; }
        [Required(ErrorMessage = "يجب تحديد طريقة الدفع")]
        public string PayTypeId { get; set; }
        public string PayTypeName { get; set; }
        [StringLength(30, ErrorMessage = "يجب ان يكون طول رقم الدفع بين 1 و 30 حرف", MinimumLength = 1)]
        public string PayNo { get; set; }
        [DataType(DataType.Date)]
        public DateTime PayDate { get; set; }
        [StringLength(80, ErrorMessage = "يجب ان يكون طول اسم البنك بين 1 و 80 حرف", MinimumLength = 1)]
        public string PayBank { get; set; }
        [RequiredAmountGreaterThanZero (ErrorMessage = "المبلغ غير صحيح او غير مسموح به")]
        public double Amount { get; set; }
        public string AmountName { get; set; }
        public double TaxPercent { get; set; }
        public double TaxAmount { get; set; }
        public double BonusPercent { get; set; }
        public double BounsAmount { get; set; }
        public double BounsTaxPercent { get; set; }
        public double BounsTaxAmount { get; set; }
        public double ReceivedAmount { get; set; }
        public double NetAmount { get; set; }
        public double FixedFactor { get; set; }
        public double RequestAmount { get; set; }
        public string BillNo { get; set; }
        public string RequestNo { get; set; }
        public string Note { get; set; }
        public string Error { get; set; }

        public virtual List<CommonCode> PayType { get; set; }
        public double CreatorBalance { get; set; }
        public string CreateorId { get; set; }
        public string CreateorName { get; set; }
        public int CreateorRoleId { get; set; }
        public string CreateorRoleName { get; set; }
    }
}
