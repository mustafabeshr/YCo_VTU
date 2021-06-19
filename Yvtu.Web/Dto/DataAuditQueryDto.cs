using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Yvtu.Core.Entities;

namespace Yvtu.Web.Dto
{
    public class DataAuditQueryDto
    {
        public DataAuditQueryDto()
        {
            Activities = new List<Activity>();
        }
        public int Id { get; set; }
        public string ActivityId { get; set; }
        public bool IncludeDates { get; set; }
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        [StringLength(9)]
        public string CreatedById { get; set; }
        public int CreatedByAccount { get; set; }

        public List<Activity> Activities { get; set; }

        public List<DataAudit> Results { get; set; }
    }
}
