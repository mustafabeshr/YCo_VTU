using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Yvtu.Core.Entities;

namespace Yvtu.Web.Dto
{
    public class PFRQueryDto
    {

        public int PartnerAccount { get; set; }
        public bool IncludeDates { get; set; }
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        [StringLength(9)]
        public string PartnerId { get; set; }
        public string PartnerName { get; set; }
        public string RoleName { get; set; }

        public virtual List<PFR> results { get; set; }
    }
}
