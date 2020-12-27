using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Yvtu.Core.Entities;

namespace Yvtu.Web.Dto
{
    public class EditPartnerDto
    {
        [StringLength(9)]
        public string Id { get; set; }
        public int Account { get; set; }

        [Required(ErrorMessage = "يجب ادخال الاسم")]
        [StringLength(100, ErrorMessage ="يجب ان يكون طول الاسم بين 10 و 100 حرف", MinimumLength = 10)]
        public string Name { get; set; }
        [StringLength(100, ErrorMessage = "يجب ان يكون طول الاسم التجاري بين 1 و 100 حرف", MinimumLength = 1)]
        public string BrandName { get; set; }
        public string PersonalIdTypeName { get; set; }

        [Required(ErrorMessage = "يجب تحديد نوع الهوية")]
        public int? PersonalIdType { get; set; }
        [Required(ErrorMessage = "يجب ادخال رقم الهوية")]
        [StringLength(100, ErrorMessage = "يجب ان يكون طول رقم الهوية بين 4 و 100", MinimumLength = 4)]
        public string PersonalIdNo { get; set; }
        [Required(ErrorMessage = "يجب تحديد تاريخ اصدار الهوية")]
        [DataType(DataType.Date)]
        public DateTime? PersonalIssued { get; set; }
        [Required(ErrorMessage = "يجب ادخال مكان اصدار الهوية")]
        [StringLength(100, ErrorMessage = "يجب ان يكون طول مكان اصدار الهوية بين 1 و 100 حرف", MinimumLength = 1)]
        public string PersonalIdPlace { get; set; }
        [Required(ErrorMessage = "يجب تحديد رقم موبايل احتياطي")]
        [MaxLength(9, ErrorMessage = "رقم الموبايل يجب ان يتكون من تسعة ارقام")]
        [RegularExpression(@"^70\d*", ErrorMessage = "رقم موبايل غير صحيح")]
        public string PairMobile { get; set; }
        public string CityName { get; set; }
        [Required(ErrorMessage = "يجب تحديد المدينة")]
        public int? CityId { get; set; }
        public string DistrictName { get; set; }
        [Required(ErrorMessage = "يجب تحديد المديرية")]
        public int? DistrictId { get; set; }
        [StringLength(120, ErrorMessage = "يجب ان يكون طول اسم الشارع بين 2 و 120 حرف", MinimumLength = 2)]
        public string Street { get; set; }
        [StringLength(120, ErrorMessage = "يجب ان يكون طول اسم الحي بين 2 و 120 حرف", MinimumLength = 2)]
        public string Zone { get; set; }

        [MaxLength(9, ErrorMessage = "رقم الموبايل يجب ان يتكون من تسعة ارقام")]
        [RegularExpression(@"^70\d*", ErrorMessage = "رقم موبايل غير صحيح")]
        public string MobileNo { get; set; }
        [StringLength(25, ErrorMessage = "يجب ان يكون طول رقم الثابت بين 8 و 25 رقما", MinimumLength = 8)]
        public string Fixed { get; set; }
        [StringLength(25, ErrorMessage = "يجب ان يكون طول رقم الفاكس بين 8 و 25 رقما", MinimumLength = 8)]
        public string Fax { get; set; }
        [StringLength(100, ErrorMessage = "يجب ان يكون طول الايميل بين 10 و 100 حرف", MinimumLength = 10)]
        public string Email { get; set; }
        [StringLength(150, ErrorMessage = "يجب ان لا يزيد طول المعلومات الاضافية عن 150 حرف")]
        public string ExtraAddressInfo { get; set; }
        public string IPAddress { get; set; }
        [Required(ErrorMessage = "يجب ادخال رقم الجهة المرجعية لهذا الحساب")]
        [MaxLength(9, ErrorMessage = "رقم الموبايل يجب ان يتكون من تسعة ارقام")]
        [RegularExpression(@"^70\d*", ErrorMessage = "رقم موبايل غير صحيح")]
        public string RefPartnerId { get; set; }

        public virtual List<IdType> IdTypes { get; set; }
        public virtual List<City> Cities { get; set; }
        public virtual List<District> Districts { get; set; }
    }
}
