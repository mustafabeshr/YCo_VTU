using System;
using System.Collections.Generic;
using System.Text;

namespace Yvtu.Core.Entities
{
    public class MessageTemplate
    {
        public MessageTemplate()
        {
            CreatedBy = new AppUser();
            Activities = new List<Activity>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public AppUser CreatedBy { get; set; }
        public DateTime  CreatedOn { get; set; }
        public DateTime  LastUpdatedOn { get; set; }

        public virtual List<Activity> Activities { get; set; }

        public override string ToString()
        {
            return $"Id={Id}" + Environment.NewLine +
                $"Title={Title}" + Environment.NewLine +
                $"Message={Message}" + Environment.NewLine +
                $"Created By={CreatedBy.Id} - {CreatedBy.Name} - {CreatedBy.Account}" + Environment.NewLine +
                $"Created On={CreatedOn.ToString("yyyy/MM/dd H:mm:ss")}" + Environment.NewLine +
                $"Last Updated On={LastUpdatedOn.ToString("yyyy/MM/dd H:mm:ss")}" + Environment.NewLine;
        }

    }

    public class MessageDictionary
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }

    }
}
