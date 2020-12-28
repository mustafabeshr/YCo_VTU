using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Yvtu.Core.Entities;

namespace Yvtu.Web.Dto
{
    public class ConfiscationDto
    {
        public ConfiscationDto()
        {
            Partner = new Partner();
        }
        public int Id { get; set; }
        [StringLength(9, ErrorMessage = "يجب ان يكون طول الرقم 9 ")]
        [Required(ErrorMessage = "يجب تحديد الجهة")]
        public string PartnerId { get; set; }
        public Partner Partner { get; set; }
        public double Amount { get; set; }
        public double Reserved { get; set; }
        [Required(ErrorMessage = "يجب كتابة الملاحظات")]
        [StringLength(150, ErrorMessage = "يجب ان يكون طول الملاحظة من 10 الى 150 حرف", MinimumLength = 10)]
        public string Note { get; set; }
    }
}
