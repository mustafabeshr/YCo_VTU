using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Yvtu.Infra.Data.CustomeValidationAttribute
{
    public class DateMustBeGreaterThanNow : ValidationAttribute
    {
        /// <summary>
        /// Designed for dropdowns to ensure that a selection is valid and not the dummy "SELECT" entry
        /// </summary>
        /// <param name="value">The integer value of the selection</param>
        /// <returns>True if value is greater than zero</returns>
        public override bool IsValid(object value)
        {
            // return true if value is a non-null number > 0, otherwise return false
            if (value == null) return false;
            DateTime dateValue = (DateTime)value;
            return dateValue.Subtract(DateTime.Now).TotalSeconds > 0;
        }
    }
}
