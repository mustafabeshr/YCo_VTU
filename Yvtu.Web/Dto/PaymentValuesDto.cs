using System.ComponentModel.DataAnnotations;

namespace Yvtu.Web.Dto
{
    public class PaymentValuesDto
    {
        [Required(ErrorMessage = "يجب ادخال المبلغ")]
        public double PayValue { get; set; }
        [Required(ErrorMessage = "يجب ادخال رقم معرف")]
        public double ProfileId { get; set; }
    }
}
