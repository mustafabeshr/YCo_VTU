using System;
using System.Collections.Generic;
using System.Text;

namespace Yvtu.Core.Entities
{
    public class Notification
    {
        public Notification()
        {
            Partner = new AppUser();
            Activity = new Activity();
        }
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public AppUser Partner { get; set; }
        public string Message { get; set; }
        public Activity Activity { get; set; }
        public int RefNo { get; set; }
    }
}
