using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Yvtu.Core.Entities;

namespace Yvtu.Web.Dto
{
    public class SMSOutBackDto
    {
        [StringLength(100)]
        public string Message { get; set; }
        [StringLength(9)]
        public string Receiver { get; set; }
        public bool IncludeDates { get; set; }
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public List<SMSOutBack> Results { get; set; }
    }
}
