using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Yvtu.Core.Entities;

namespace Yvtu.Web.Dto
{
    public class SMSInQueryDto
    {
        [StringLength(9)]
        public string Sender { get; set; }
        [StringLength(6)]
        public string Receiver { get; set; }
        [StringLength(50)]
        public string Message { get; set; }
        public bool IncludeDates { get; set; }
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        public List<SMSIn> Results { get; set; }
    }
}
