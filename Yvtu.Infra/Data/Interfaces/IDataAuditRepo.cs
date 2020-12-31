using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Yvtu.Core.Entities;

namespace Yvtu.Infra.Data.Interfaces
{
    public interface IDataAuditRepo
    {
        public class GetListParam
        {
            public string CreatorId { get; set; }
            public int CreatorAccount { get; set; }
            public string ActivityId { get; set; }
            public bool IncludeDates { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }
        OpertionResult Create(DataAudit data);
        List<DataAudit> GetAuditig(GetListParam param);
    }
}
