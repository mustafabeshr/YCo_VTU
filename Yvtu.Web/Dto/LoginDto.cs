using System.ComponentModel.DataAnnotations;

namespace Yvtu.Web.Dto
{
    public class LoginDto
    {
        [Required(ErrorMessage = "يجب ادخال رقم الموبايل")]
        [MaxLength(9, ErrorMessage = "رقم الموبايل يجب ان يتكون من تسعة ارقام")]
        [RegularExpression(@"^70\d*", ErrorMessage = "رقم موبايل غير صحيح")]
        public string Id { get; set; }

        [Required(ErrorMessage = "يجب ادخال الرقم السري")]
        [MaxLength(4, ErrorMessage = "رقم الموبايل يجب ان يتكون من تسعة ارقام")]
        //[RegularExpression(@"^d*", ErrorMessage = "كلمة المرور غير صحيحة")]
        [DataType(DataType.Password)]
        public string Pwd { get; set; }

        public bool RememberMe { get; set; }

        public string Error { get; set; }
    }
}
