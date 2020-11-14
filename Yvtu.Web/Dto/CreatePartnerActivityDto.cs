using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Yvtu.Core.Entities;

namespace Yvtu.Web.Dto
{
    public class CreatePartnerActivityDto
    {
        public CreatePartnerActivityDto()
        {
            FromRole = new Role();
            ToRole = new Role();
            Activity = new Activity();
        }
        public Activity Activity { get; set; }
        public Role FromRole { get; set; }
        public Role ToRole { get; set; }

        public int Id { get; set; }
        [Required(ErrorMessage = "يجب تحديد الاجراء")]
        public string ActivityId { get; set; }
       
        [Required(ErrorMessage = "يجب تحديد نوع الجهة المصدر")]
        public int? FromRoleId { get; set; }
       
        [Required(ErrorMessage = "يجب تحديد نوع الجهة المستهدفة")]
        public int? ToRoleId { get; set; }
        
        public bool CheckBalanceRequired { get; set; }
        [Required(ErrorMessage = "يجب تحديد أعلى قيمة")]
        [Range(0, 10000000, ErrorMessage = "القيمة الصحيحة هي أقل من عشرة مليون")]
        public int MaxValue { get; set; }
        [Required(ErrorMessage = "يجب تحديد إقل قيمة")]
        
        [Range(0, 1000000, ErrorMessage = "القيمة الصحيحة هي بين أقل من مليون")]
        
        public int MinValue { get; set; }
        [Required(ErrorMessage = "يجب تحديد نسبة البونص")]
        [Range(0, 100 , ErrorMessage ="نسبة البونص غير صحيحة") ]
        public double BonusPercent { get; set; }
        [Required(ErrorMessage = "يجب تحديد نسبة ضريبة البونص")]
        [Range(0, 100, ErrorMessage = "نسبة ضريبة البونص غير صحيحة")]
        public double BonusTaxPercent { get; set; }
        [Required(ErrorMessage = "يجب تحديد نسبة الضريبة")]
        [Range(0, 100, ErrorMessage = "نسبة الضريبة غير صحيحة")]
        public double TaxPercent { get; set; }
        public string MaxQueryDurationId { get; set; }
        public string ScopeId { get; set; }
        [Range(0, 1000, ErrorMessage = "اقصى عدد السجلات غير صحيح")]
        public int MaxQueryRowsNo { get; set; }
        public bool OnlyPartnerChildren { get; set; }

        public virtual List<Role> FromRoles { get; set; }
        public virtual List<Role> ToRoles { get; set; }
        public virtual List<Activity> Activities { get; set; }
        public virtual List<CommonCode> MaxQueryDuration { get; set; }
        public virtual List<CommonCode> Scopes { get; set; }

    }
}
