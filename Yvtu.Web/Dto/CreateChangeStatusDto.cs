using System;
using System.ComponentModel.DataAnnotations;
using Yvtu.Core.Entities;

namespace Yvtu.Web.Dto
{
    public class CreateChangeStatusDto
    {
        public CreateChangeStatusDto()
        {
            Partner = new Partner();
            OldStatus = new PartnerStatus();
            NewStatus = new PartnerStatus();
        }
        public int Id { get; set; }
        [StringLength(9, ErrorMessage = "يجب ان يكون طول الرقم 9 ")]
        [Required(ErrorMessage = "يجب تحديد الجهة")]
        //[RegularExpression(@"^70\d*", ErrorMessage = "رقم موبايل غير صحيح")]
        public string PartnerId { get; set; }
        public Partner Partner { get; set; }
        public int OldStatusId { get; set; }
        public int NewStatusId { get; set; }
        public PartnerStatus OldStatus { get; set; }
        public PartnerStatus NewStatus { get; set; }
        [Required(ErrorMessage = "يجب كتابة الملاحظات")]
        [StringLength(100, ErrorMessage = "يجب ان يكون طول الملاحظة من 10 الى 100 حرف", MinimumLength = 10)]
        public string Note { get; set; }
        public DateTime NewStatusExpireOn { get; set; }
        public string Error { get; set; }
    }
}
