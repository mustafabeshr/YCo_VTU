using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Yvtu.Core.Entities;

namespace Yvtu.Web.Dto
{
    public class AppBackgroundServiceQueryDto
    {
        public int Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        public bool IncludeDates { get; set; }
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        public string Source { get; set; }
        [StringLength(9)]
        public string CreatedById { get; set; }
        [MaxLength(9, ErrorMessage = "رقم الموبايل يجب ان يتكون من تسعة ارقام")]
        [RegularExpression(@"^70\d*", ErrorMessage = "رقم موبايل غير صحيح")]
        [Required(ErrorMessage = "يجب تحديد الجهة")]
        public string PartnerId { get; set; }
        public int ActionPartnerAccount { get; set; }
        [MaxLength(9, ErrorMessage = "رقم الموبايل يجب ان يتكون من تسعة ارقام")]
        [RegularExpression(@"^70\d*", ErrorMessage = "رقم موبايل غير صحيح")]
        public string ActionPartnerId { get; set; }
        public string Status { get; set; }

        public virtual List<CommonCode> Statuses { get; set; }
        public virtual List<CommonCode> Sources { get; set; }

        public List<AppBackgroundService> Results { get; set; }
    }
}
