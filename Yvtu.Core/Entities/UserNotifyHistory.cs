using System;
using System.Collections.Generic;
using System.Text;

namespace Yvtu.Core.Entities
{
    public class UserNotifyHistory
    {
            public UserNotifyHistory()
            {
                Status = new CommonCode();
                CreatedBy = new AppUser();
                Partner = new AppUser();
                Priority = new CommonCode();
            }
            public int Id { get; set; }
            public int UserNotifyId { get; set; }
            public AppUser Partner { get; set; }
            public DateTime CreatedOn { get; set; }
            public string Content { get; set; }
        public string Subject { get; set; }
        public CommonCode Priority { get; set; }
            public CommonCode Status { get; set; }
            public DateTime StatusOn { get; set; }
            public AppUser CreatedBy { get; set; }
            public DateTime ExpireOn { get; set; }
            public DateTime HistoryOn { get; set; }
    }
}
