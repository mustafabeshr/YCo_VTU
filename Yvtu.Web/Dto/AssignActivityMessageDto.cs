using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Yvtu.Core.Entities;

namespace Yvtu.Web.Dto
{
    public class AssignActivityMessageDto
    {
        public AssignActivityMessageDto()
        {
            Activity = new Activity();
            Message = new MessageTemplate();
            SendingTime = new List<CommonCode>();
        }
        [Required(ErrorMessage = "يجب تحديد الاجراء")]
        public string ActivityId { get; set; }
        public Activity Activity { get; set; }
        [Required(ErrorMessage = "يجب تحديد الرسالة")]
        public int MessageId { get; set; }
        public MessageTemplate Message { get; set; }
        [Required(ErrorMessage = "يجب تحديد توقيت الارسال")]
        public string SendingTimeId { get; set; }
        public List<CommonCode> SendingTime { get; set; }
        public int MessageOrder { get; set; }

        public virtual List<SelectedMessages> Messages { get; set; }
        public virtual List<ActivityMessage> ActivityMessages { get; set; }
    }

    public class SelectedMessages
    {
        public MessageTemplate Message { get; set; }
        public string SendingTimeId { get; set; }
        public List<CommonCode> SendingTime { get; set; }
    }
}
