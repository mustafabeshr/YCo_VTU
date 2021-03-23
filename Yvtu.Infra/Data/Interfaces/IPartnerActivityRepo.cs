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
        PartnerActivity GetPartAct(string actId, int fromRoleId, int toRoleId);
        PartnerActivity GetPartAct(string actId, int fromRoleId);
        PartnerActivity GetPartAct(int id);
        List<PartnerActivity> GetListByActivity(string activityId);
        List<PartnerActivity> GetListByFrom(int fromId);
        List<PartnerActivity> GetListByActivityWithFromRole(string activityId, int fromRoleId);
        List<PartnerActivityDetail> GetDetails(int id, bool withMaster = false);
        List<PartnerActivityDetail> GetDetails(int id, int toRoleId, bool withMaster = false);
        PartnerActivityDetail GetDetail(int id, int parentId, bool withMaster = false);
        PartnerActivityDetail GetDetail(string actId, int fromRoleId, int toRoleId, bool withMaster = false);
        OpertionResult Create(PartnerActivity partnerActivity);
        OpertionResult CreateDetail(PartnerActivityDetail model);
        OpertionResult Edit(PartnerActivity partnerActivity);
        OpertionResult UpdateDetail(PartnerActivityDetail model);
        bool Delete(int id);
        bool DeleteDetail(int id, int parentId);
    }
}
