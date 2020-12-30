using System;
using System.Collections.Generic;
using System.Text;

namespace Yvtu.Core.Entities
{
    public class PartBasicInfo
    {
        public string Id { get; set; }
        public int Account { get; set; }
        public string Name { get; set; }
        public Role Role { get; set; }
        public double Balance { get; set; }
        public double Reserved { get; set; }
        public PartnerStatus Status { get; set; }
        public DateTime LastLoginOn { get; set; }
        public string Error { get; set; }
        public string RefPartnerId { get; set; }
    }
}
