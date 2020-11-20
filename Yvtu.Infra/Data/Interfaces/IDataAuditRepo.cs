using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Yvtu.Core.Entities;

namespace Yvtu.Infra.Data.Interfaces
{
    public interface IDataAuditRepo
    {
        Task<OpertionResult> CreateAsync(DataAudit data);
        List<DataAudit> GetAuditig(string partnerId, string activityId, string actionId, DateTime startDate, DateTime endDate);
    }
}
