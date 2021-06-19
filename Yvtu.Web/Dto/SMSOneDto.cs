using System.ComponentModel.DataAnnotations;

namespace Yvtu.Web.Dto
{
    public class SMSOneDto
    {
        public int Id { get; set; }
        [StringLength(9, ErrorMessage = "يجب ان يكون طول الرقم 9 ")]
        [Required(ErrorMessage = "يجب تحديد الرقم المستقبل")]
        [RegularExpression(@"^70\d*", ErrorMessage = "رقم موبايل غير صحيح")]
        public string Receiver { get; set; }
        [Required(ErrorMessage = "يجب كتابة نص الرسالة")]
        [StringLength(180, ErrorMessage = "يجب ان يكون طول نص الرسالة من 10 الى 180 حرف", MinimumLength = 10)]
        public string Message { get; set; }
        //[Required(ErrorMessage = "يجب كتابة الملاحظات")]
        [StringLength(150, ErrorMessage = "يجب ان يكون طول الملاحظات من 10 الى 150 حرف", MinimumLength = 10)]
        public string Note { get; set; }
    }
}
