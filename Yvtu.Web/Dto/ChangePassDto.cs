using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Yvtu.Web.Dto
{
    public class ChangePassDto
    {
        public int PartnerAccount { get; set; }
        public string PartnerId { get; set; }
        [Required(ErrorMessage ="يجب ادخال كلمة المرور القديمة")]
        [DataType(DataType.Password)]
        
        public int? OldPass { get; set; }
        [Required(ErrorMessage = "يجب ادخال كلمة المرور الجديدة")]
        [DataType(DataType.Password)]
        //[RegularExpression("^[0-9]{4,6}$", ErrorMessage ="كلمة المرور تتكون من 4 الى 6 ارقام فقط")]
        public int? NewPass { get; set; }
        [Required(ErrorMessage = "يجب تاكيد كلمة المرور الجديدة")]
        [Compare("NewPass", ErrorMessage = "كلمات المرور الجديدة غير متطابقة")]
        [DataType(DataType.Password)]
        public int? ConfirmNewPass { get; set; }
        public string Error { get; set; }
    }
}
