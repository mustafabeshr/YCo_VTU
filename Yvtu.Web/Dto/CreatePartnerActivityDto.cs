using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Range(0, 100, ErrorMessage = "نسبة البونص غير صحيحة")]
        public double BonusPercent { get; set; }
        [Required(ErrorMessage = "يجب تحديد نسبة ضريبة البونص")]
        [Range(0, 100, ErrorMessage = "نسبة ضريبة البونص غير صحيحة")]
        public double BonusTaxPercent { get; set; }
        [Required(ErrorMessage = "يجب تحديد نسبة الضريبة")]
        [Range(0, 100, ErrorMessage = "نسبة الضريبة غير صحيحة")]
        public double TaxPercent { get; set; }
        [Required(ErrorMessage = "يجب تحديد اقصى مدة يمكن الاستعلام خلالها")]
        public string MaxQueryDurationId { get; set; }
        [Required(ErrorMessage = "يجب تحديد مدى الاستعلام")]
        public string ScopeId { get; set; }
        [Range(0, 1000, ErrorMessage = "اقصى عدد السجلات غير صحيح")]
        public int MaxQueryRowsNo { get; set; }
        public bool OnlyPartnerChildren { get; set; }
        public string Error { get; set; }

        public virtual List<Role> FromRoles { get; set; }
        public virtual List<Role> ToRoles { get; set; }
        public virtual List<Activity> Activities { get; set; }
        public virtual List<CommonCode> MaxQueryDuration { get; set; }
        public virtual List<CommonCode> Scopes { get; set; }

    }

    public class CreatePartnerActivity2Dto
    {
        public CreatePartnerActivity2Dto()
        {
            FromRole = new Role();
            Activity = new Activity();
            Details = new List<CreatePartnerActivityDetailDto>();
        }
        public int Id { get; set; }
        public Activity Activity { get; set; }
        public Role FromRole { get; set; }

        [Required(ErrorMessage = "يجب تحديد الاجراء")]
        public string ActivityId { get; set; }

        [Required(ErrorMessage = "يجب تحديد نوع الجهة المصدر")]
        public int? FromRoleId { get; set; }
        [Required(ErrorMessage = "يجب تحديد اقصى مدة يمكن الاستعلام خلالها")]
        public string MaxQueryDurationId { get; set; }
        [Required(ErrorMessage = "يجب تحديد مدى الاستعلام")]
        public string ScopeId { get; set; }
        [Range(0, 1000, ErrorMessage = "اقصى عدد السجلات غير صحيح")]
        public int MaxQueryRowsNo { get; set; }
        public bool OnlyPartnerChildren { get; set; }
        public string Error { get; set; }

        public virtual List<Role> FromRoles { get; set; }
        public virtual List<Activity> Activities { get; set; }
        public virtual List<CommonCode> MaxQueryDuration { get; set; }
        public virtual List<CommonCode> Scopes { get; set; }

        public List<CreatePartnerActivityDetailDto> Details { get; set; }

    }
    public class CreatePartnerActivityDetailDto
    {
        public CreatePartnerActivityDetailDto()
        {
            ToRole = new Role();
            CreatedBy = new AppUser();
        }
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string ActivityId { get; set; }
        public string ActivityName { get; set; }
        public int FromRoleId { get; set; }
        public string FromRoleName { get; set; }
        [Required(ErrorMessage = "يجب تحديد نوع الجهة المستفيدة")]
        public int ToRoleId { get; set; }
        public Role ToRole { get; set; }
        public bool CheckBalanceRequired { get; set; }
        [Required(ErrorMessage = "يجب تحديد أعلى قيمة")]
        [Range(0, 10000000, ErrorMessage = "القيمة الصحيحة هي أقل من عشرة مليون")]
        public int MaxValue { get; set; }
        [Required(ErrorMessage = "يجب تحديد إقل قيمة")]
        [Range(0, 1000000, ErrorMessage = "القيمة الصحيحة هي بين أقل من مليون")]
        public int MinValue { get; set; }
        [Required(ErrorMessage = "يجب تحديد نسبة ضريبة العمولة")]
        [Range(0, 100, ErrorMessage = "نسبة ضريبة العمولة غير صحيحة")]
        public double BonusPercent { get; set; }
        [Required(ErrorMessage = "يجب تحديد نسبة العمولة")]
        [Range(0, 100, ErrorMessage = "نسبة العمولة غير صحيحة")]
        public double BonusTaxPercent { get; set; }
        [Required(ErrorMessage = "يجب تحديد نسبة الضريبة")]
        [Range(0, 100, ErrorMessage = "نسبة الضريبة غير صحيحة")]
        public double TaxPercent { get; set; }
        [Required(ErrorMessage = "يجب تحديد النسبة الثابتة")]
        public double FixedFactor { get; set; }
        public AppUser CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastEditOn { get; set; }
        public string Error { get; set; }
        public virtual List<Role> ToRoles { get; set; }
    }
}
