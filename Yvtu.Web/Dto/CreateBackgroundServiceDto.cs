using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data.CustomeValidationAttribute;

namespace Yvtu.Web.Dto
{
    public class CreateBackgroundServiceDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "يجب ادخال وصف الطلب")]
        [StringLength(100, ErrorMessage = "يجب ان يكون طول الوصف بين 10 و 100 حرفا", MinimumLength = 10)]
        public string Name { get; set; }
        [Required(ErrorMessage = "يجب تحديد مصدر البيانات")]
        public string Source { get; set; }
        [StringLength(9, ErrorMessage = "يجب ان يكون طول الرقم 9 ارقام")]
        [RegularExpression(@"^70\d*", ErrorMessage = "رقم موبايل غير صحيح")]
        public string PartnerId { get; set; }
        public string PartnerName { get; set; }
        public int PartnerAccount { get; set; }
        [DataType(DataType.Date)]
        [DateRangeAttributeValidation(ErrorMessage = "تاريخ خاطئ")]
        public DateTime? StartDate { get; set; }
        [DataType(DataType.Date)]
        [DateRangeAttributeValidation(ErrorMessage = "تاريخ خاطئ")]
        public DateTime? EndDate { get; set; }
        [Required(ErrorMessage = "يجب تحديد وقت التفعيل")]
        [DateMustBeGreaterThanNow(ErrorMessage = "وقت التفعيل غير صحيح")]
        [DataType(DataType.DateTime)]
        public DateTime? ActiveTime { get; set; }
        [StringLength(150, ErrorMessage = "يجب ان يكون طول الملاحظات اقل من 150 حرفا")]
        public string Note { get; set; }

        public List<CommonCode> Sources { get; set; }
    }
}

