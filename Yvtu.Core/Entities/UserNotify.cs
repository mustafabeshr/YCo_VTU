using System;
using System.Collections.Generic;
using System.Text;

namespace Yvtu.Core.Entities
{
    public class UserNotify
    {
        public UserNotify()
        {
            Status = new CommonCode();
            CreatedBy = new AppUser();
            Priority = new CommonCode();
            NotifyToList = new List<UserNotifyTo>();
        }
        public int Id { get; set; }
        public int UserNotifyHistoryCount { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Content { get; set; }
        public string Subject { get; set; }
        public CommonCode Priority { get; set; }
        public CommonCode Status { get; set; }
        public DateTime StatusOn { get; set; }
        public AppUser CreatedBy { get; set; }
        public DateTime ExpireOn { get; set; }

        public List<UserNotifyTo> NotifyToList { get; set; }
    }

    public class UserNotifyTo
    {
        public UserNotifyTo()
        {
            Role = new Role();
        }
        public int Id { get; set; }
        public int UserNotifyId { get; set; }
        public Role Role { get; set; }

    }
}
