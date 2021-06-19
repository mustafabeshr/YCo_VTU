using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Yvtu.Core.Entities;

namespace Yvtu.Web.Dto
{
    public class ConfiscationQueryDto
    {
        public int Id { get; set; }
        [StringLength(9)]
        public string PartnerId { get; set; }
        public int PartnerAccount { get; set; }
        public bool IncludeDates { get; set; }
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        [StringLength(9)]
        public string CreatedById { get; set; }
        public int CreatedByAccount { get; set; }

        public List<Confiscation> Results { get; set; }
    }
}
