using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Yvtu.Core.Entities;

namespace Yvtu.Infra.Data.Interfaces
{
    public interface IDataAuditRepo
    {
        OpertionResult Create(DataAudit data);
        List<DataAudit> GetAuditig(int partnerAccount, string partnerId, string activityId, string actionId, DateTime startDate, DateTime endDate);
    }
}
