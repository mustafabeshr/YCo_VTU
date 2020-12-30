using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Yvtu.Core.Entities;

namespace Yvtu.Core.Queries
{
    public class NotificationQuery
    {
        [StringLength(9)]
        public string QPartnerId { get; set; }

        public List<Notification> Results { get; set; }
    }
}
