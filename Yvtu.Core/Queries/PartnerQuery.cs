using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Yvtu.Core.Entities;

namespace Yvtu.Core.Queries
{
    public class PartnerQuery
    {
        public PartnerQuery()
        {
            Paging = new Paging();
        }
        //[MaxLength(8)]
        public int? QAccount { get; set; }
        //[MaxLength(9)]
        [StringLength(9)]
        public string QPartnerId { get; set; }
        public string QCreatorId { get; set; }
        public int QCreatorAccount { get; set; }
        public int QCreatorRoleId { get; set; }
        //[MaxLength(100)]
        public string QPartnerName { get; set; }
        [StringLength(9)]
        public string QRefPartnerId { get; set; }
        public int? QRoleId { get; set; }
        public int? QStatusId { get; set; }
        public string Error { get; set; }
        public string Success { get; set; }
        public Paging Paging { get; set; }

        public virtual List<Role> Roles { get; set; }
        public virtual List<PartnerStatus> Statuses { get; set; }
        public virtual List<Partner> Partners { get; set; }
    }
}
