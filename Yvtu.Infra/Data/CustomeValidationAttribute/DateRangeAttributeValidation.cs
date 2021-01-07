using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Yvtu.Infra.Data.CustomeValidationAttribute
{
    public class DateRangeAttributeValidation : RangeAttribute
    {
        public DateRangeAttributeValidation()
       : base(typeof(DateTime), DateTime.Now.AddYears(-2).ToShortDateString(), DateTime.Now.ToShortDateString()) { }
    }
}
