using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Yvtu.Core.Entities;

namespace Yvtu.Infra.Data.Interfaces
{
    public interface IPartnerActivityRepo
    {
        List<PartnerActivity> GetAllList();
        PartnerActivity GetPartAct(string actId, string fromRoleId, string toRoleId);
        Task<OpertionResult> CreateAsync(PartnerActivity partnerActivity);
    }
}
