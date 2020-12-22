using System;
using System.Collections.Generic;
using System.Text;

namespace Yvtu.Core.Entities
{
    public class ActivityMessage
    {
        public ActivityMessage()
        {
            Activity = new Activity();
            Message = new MessageTemplate();
            SendingTime = new CommonCode();
            CreatedBy = new AppUser();
        }
        public Activity Activity { get; set; }
        public MessageTemplate Message { get; set; }
        public CommonCode SendingTime { get; set; }
        public AppUser CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int MessageOrder { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"Activity Id={Activity.Id}");
            sb.Append($"\nActivity Name={Activity.Name}");
            sb.Append($"\nMessage Id={Message.Id}");
            sb.Append($"\nMessage Title={Message.Title}");
            sb.Append($"\nMessage Text={Message.Message}");
            sb.Append($"\nMessage Order={MessageOrder}");
            sb.Append($"\nCreated By={CreatedBy.Id} - {CreatedBy.Name} - {CreatedBy.Account}");
            sb.Append($"\nCreated On={CreatedOn.ToString("yyyy/MM/dd H:mm:ss")}");
            return sb.ToString();
        }
    }
}
