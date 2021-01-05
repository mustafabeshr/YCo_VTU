using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Yvtu.Core.Entities;

namespace Yvtu.Web.Dto
{
    public class CreateBackgroundServiceDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "يجب تحديد مصدر البيانات")]
        public string Source { get; set; }
        [StringLength(9, ErrorMessage = "يجب ان يكون طول الرقم 9 ارقام")]
        [RegularExpression(@"^70\d*", ErrorMessage = "رقم موبايل غير صحيح")]
        public string PartnerId { get; set; }
        public string PartnerName { get; set; }
        public int PartnerAccount { get; set; }
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        [Required(ErrorMessage = "يجب تحديد وقت التفعيل")]
        [DataType(DataType.Date)]
        public DateTime ActiveTime { get; set; }
        [StringLength(150, ErrorMessage = "يجب ان يكون طول الملاحظات اقل من 150 حرفا")]
        public string Note { get; set; }

        public List<CommonCode> Sources { get; set; }
    }
}

