﻿using System;
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
        PartnerActivity GetPartAct(int id);
        List<PartnerActivity> GetListByActivity(string activityId);
        List<PartnerActivity> GetListByActivityWithFromRole(string activityId, int fromRoleId);
        OpertionResult Create(PartnerActivity partnerActivity);
        OpertionResult CreateDetail(PartnerActivityDetail model);
        OpertionResult Edit(PartnerActivity partnerActivity);
        bool Delete(int id);
    }
}
