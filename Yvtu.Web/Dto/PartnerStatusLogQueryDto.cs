using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Yvtu.Core.Entities;

namespace Yvtu.Web.Dto
{
    public class PartnerStatusLogQueryDto
    {
        [StringLength(9)]
        public string CreatedById { get; set; }
        public int CreatedByAccount { get; set; }
        [StringLength(9)]
        public string PartnerId { get; set; }
        public int PartnerAccount { get; set; }
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public virtual List<PartnerStatusLog> results { get; set; }
    }
}
