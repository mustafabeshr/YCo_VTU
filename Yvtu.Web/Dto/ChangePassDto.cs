using System.ComponentModel.DataAnnotations;

namespace Yvtu.Web.Dto
{
    public class ChangePassDto
    {
        public int PartnerAccount { get; set; }
        public string PartnerId { get; set; }
        [Required(ErrorMessage = "يجب ادخال الرقم السري القديم")]
        [DataType(DataType.Password)]

        public int? OldPass { get; set; }
        [Required(ErrorMessage = "يجب ادخال الرقم السري الجديد")]
        [DataType(DataType.Password)]
        //[RegularExpression("^[0-9]{4,6}$", ErrorMessage ="كلمة المرور تتكون من 4 الى 6 ارقام فقط")]
        public int? NewPass { get; set; }
        [Required(ErrorMessage = "يجب تاكيد الرقم السري الجديد")]
        [Compare("NewPass", ErrorMessage = "الرقم السري الجديد و القديم غير متطابق")]
        [DataType(DataType.Password)]
        public int? ConfirmNewPass { get; set; }
        public string Error { get; set; }
    }
}
