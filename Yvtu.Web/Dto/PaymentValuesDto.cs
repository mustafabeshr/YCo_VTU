using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Yvtu.Web.Dto
{
    public class PaymentValuesDto
    {
        [Required(ErrorMessage = "يجب ادخال المبلغ")]
        public double PayValue { get; set; }
    }
}
