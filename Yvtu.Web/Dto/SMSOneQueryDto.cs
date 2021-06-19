using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Yvtu.Core.Entities;

namespace Yvtu.Web.Dto
{
    public class SMSOneQueryDto
    {
        public int Id { get; set; }
        [StringLength(100)]
        public string Message { get; set; }
        [StringLength(9)]
        public string Receiver { get; set; }
        public bool IncludeDates { get; set; }
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        [StringLength(9)]
        public string CreatedById { get; set; }
        public int CreatedByAccount { get; set; }

        public List<SMSOne> Results { get; set; }
    }
}
