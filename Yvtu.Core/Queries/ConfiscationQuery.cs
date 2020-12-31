using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Yvtu.Core.Queries
{
    public class ConfiscationQuery
    {
        public int QId { get; set; }
        [StringLength(9)]
        public string QPartnerId { get; set; }
        public int QPartnerAccount { get; set; }
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

    }
}
