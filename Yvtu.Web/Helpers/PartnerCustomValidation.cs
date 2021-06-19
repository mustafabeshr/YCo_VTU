using System.ComponentModel.DataAnnotations;
using Yvtu.Web.Dto;

namespace Yvtu.Web.Helpers
{
    public class PartnerCustomValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var partner = (CreatePartnerDto)validationContext.ObjectInstance;
            if (partner.Id == partner.PairMobile) return new ValidationResult("الرقم الاحتياطي يجب ان يكون غير الرقم الاساسي");
            return ValidationResult.Success;
        }
    }

    public class PartnerEditCustomValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var partner = (EditPartnerDto)validationContext.ObjectInstance;
            if (partner.Id == partner.PairMobile) return new ValidationResult("الرقم الاحتياطي يجب ان يكون غير الرقم الاساسي");
            return ValidationResult.Success;
        }
    }
}
