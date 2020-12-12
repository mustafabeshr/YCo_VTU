using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.SMSRec.Repo
{
    public class PartnerActivityRepo
    {
        private readonly IRecDbContext db;

        public PartnerActivityRepo(IRecDbContext db)
        {
            this.db = db;
        }

        public List<PartnerActivity> GetListByActivityWithFromRole(string activityId, int fromRoleId)
        {
            var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "activityId", OracleDbType = OracleDbType.Varchar2,  Value = activityId },
                 new OracleParameter{ ParameterName = "fromRoleId", OracleDbType = OracleDbType.Int32,  Value = fromRoleId }
            };
            var actDataTable = this.db.GetData("Select * from v_partner_activity  where act_id=:activityId and fromroleid=:fromRoleId order by fromroleid,fromroleorder ", parameters);

            var activities = new List<PartnerActivity>();
            if (actDataTable != null)
            {
                foreach (DataRow row in actDataTable.Rows)
                {
                    var partAct = new PartnerActivity();
                    partAct.Id = row["row_id"] == DBNull.Value ? 0 : int.Parse(row["row_id"].ToString());

                    partAct.Activity.Id = row["act_id"] == DBNull.Value ? string.Empty : row["act_id"].ToString();
                    partAct.Activity.Name = row["act_name"] == DBNull.Value ? string.Empty : row["act_name"].ToString();
                    partAct.Activity.Type = row["act_type"] == DBNull.Value ? string.Empty : row["act_type"].ToString();
                    partAct.Activity.Order = row["act_order"] == DBNull.Value ? 0 : int.Parse(row["act_order"].ToString());
                    partAct.Activity.Internal = row["internal_use"] == DBNull.Value ? false : row["internal_use"].ToString() == "1" ? true : false;

                    partAct.FromRole.Id = row["fromroleid"] == DBNull.Value ? 0 : int.Parse(row["fromroleid"].ToString());
                    partAct.FromRole.Name = row["fromrolename"] == DBNull.Value ? string.Empty : row["fromrolename"].ToString();
                    partAct.FromRole.IsActive = row["fromroleisactive"] == DBNull.Value ? false : row["fromroleisactive"].ToString() == "1" ? true : false;
                    partAct.FromRole.Weight = row["fromroleweight"] == DBNull.Value ? 0 : int.Parse(row["fromroleweight"].ToString());
                    partAct.FromRole.Order = row["fromroleorder"] == DBNull.Value ? byte.MinValue : byte.Parse(row["fromroleorder"].ToString());
                    partAct.FromRole.Code = row["fromordercode"] == DBNull.Value ? string.Empty : row["fromordercode"].ToString();

                    partAct.MaxQueryDuration.Id = row["queryduration"] == DBNull.Value ? string.Empty : row["queryduration"].ToString();
                    partAct.MaxQueryDuration.Name = row["queryduration_name"] == DBNull.Value ? string.Empty : row["queryduration_name"].ToString();
                    partAct.Scope.Id = row["act_scope"] == DBNull.Value ? string.Empty : row["act_scope"].ToString();
                    partAct.Scope.Name = row["act_scope_name"] == DBNull.Value ? string.Empty : row["act_scope_name"].ToString();
                    partAct.MaxQueryRows = row["maxrec"] == DBNull.Value ? 0 : int.Parse(row["maxrec"].ToString());
                    partAct.OnlyPartnerChildren = row["onlypartchildren"] == DBNull.Value ? false : row["onlypartchildren"].ToString() == "1" ? true : false;
                    partAct.CreatedOn = row["createdon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["createdon"].ToString());
                    partAct.LastEditOn = row["lastediton"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["lastediton"].ToString());
                    partAct.CreatedBy.Id = row["createdby"] == DBNull.Value ? string.Empty : row["createdby"].ToString();
                    partAct.CreatedBy.Account = row["createdbyacc"] == DBNull.Value ? -1 : int.Parse(row["createdbyacc"].ToString());
                    partAct.CreatedBy.Name = row["createdname"] == DBNull.Value ? string.Empty : row["createdname"].ToString();
                    partAct.Details = GetDetails(partAct.Id);

                    activities.Add(partAct);
                }
            }
            return activities;
        }

        public PartnerActivity GetPartAct(string actId, int fromRoleId)
        {
            var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "actId", OracleDbType = OracleDbType.Varchar2,  Value = actId },
                 new OracleParameter{ ParameterName = "fromRoleId", OracleDbType = OracleDbType.Int32,  Value = fromRoleId }
            };
            var actDataTable = this.db.GetData("Select * from v_partner_activity  where act_id=:actId and fromroleid=:fromRoleId ", parameters);
            var partAct = new PartnerActivity();
            if (actDataTable != null && actDataTable.Rows.Count > 0)
            {
                DataRow row = actDataTable.Rows[0];
                partAct.Id = row["row_id"] == DBNull.Value ? 0 : int.Parse(row["row_id"].ToString());

                partAct.Activity.Id = row["act_id"] == DBNull.Value ? string.Empty : row["act_id"].ToString();
                partAct.Activity.Name = row["act_name"] == DBNull.Value ? string.Empty : row["act_name"].ToString();
                partAct.Activity.Type = row["act_type"] == DBNull.Value ? string.Empty : row["act_type"].ToString();
                partAct.Activity.Order = row["act_order"] == DBNull.Value ? 0 : int.Parse(row["act_order"].ToString());
                partAct.Activity.Internal = row["internal_use"] == DBNull.Value ? false : row["internal_use"].ToString() == "1" ? true : false;

                partAct.FromRole.Id = row["fromroleid"] == DBNull.Value ? 0 : int.Parse(row["fromroleid"].ToString());
                partAct.FromRole.Name = row["fromrolename"] == DBNull.Value ? string.Empty : row["fromrolename"].ToString();
                partAct.FromRole.IsActive = row["fromroleisactive"] == DBNull.Value ? false : row["fromroleisactive"].ToString() == "1" ? true : false;
                partAct.FromRole.Weight = row["fromroleweight"] == DBNull.Value ? 0 : int.Parse(row["fromroleweight"].ToString());
                partAct.FromRole.Order = row["fromroleorder"] == DBNull.Value ? byte.MinValue : byte.Parse(row["fromroleorder"].ToString());
                partAct.FromRole.Code = row["fromordercode"] == DBNull.Value ? string.Empty : row["fromordercode"].ToString();

                partAct.MaxQueryDuration.Id = row["queryduration"] == DBNull.Value ? string.Empty : row["queryduration"].ToString();
                partAct.MaxQueryDuration.Name = row["queryduration_name"] == DBNull.Value ? string.Empty : row["queryduration_name"].ToString();
                partAct.Scope.Id = row["act_scope"] == DBNull.Value ? string.Empty : row["act_scope"].ToString();
                partAct.Scope.Name = row["act_scope_name"] == DBNull.Value ? string.Empty : row["act_scope_name"].ToString();
                partAct.MaxQueryRows = row["maxrec"] == DBNull.Value ? 0 : int.Parse(row["maxrec"].ToString());
                partAct.OnlyPartnerChildren = row["onlypartchildren"] == DBNull.Value ? false : row["onlypartchildren"].ToString() == "1" ? true : false;
                partAct.CreatedOn = row["createdon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["createdon"].ToString());
                partAct.LastEditOn = row["lastediton"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["lastediton"].ToString());
                partAct.CreatedBy.Id = row["createdby"] == DBNull.Value ? string.Empty : row["createdby"].ToString();
                partAct.CreatedBy.Account = row["createdbyacc"] == DBNull.Value ? -1 : int.Parse(row["createdbyacc"].ToString());
                partAct.CreatedBy.Name = row["createdname"] == DBNull.Value ? string.Empty : row["createdname"].ToString();
                partAct.Details = GetDetails(partAct.Id, false);
                return partAct;
            }
            else
            {
                return null;
            }

        }

        public PartnerActivity GetPartAct(string actId, int fromRoleId, int toRoleId)
        {
            var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "actId", OracleDbType = OracleDbType.Varchar2,  Value = actId },
                 new OracleParameter{ ParameterName = "fromRoleId", OracleDbType = OracleDbType.Int32,  Value = fromRoleId }
            };
            var actDataTable = this.db.GetData("Select * from v_partner_activity  where act_id=:actId and fromroleid=:fromRoleId ", parameters);
            var partAct = new PartnerActivity();
            if (actDataTable != null && actDataTable.Rows.Count > 0)
            {
                DataRow row = actDataTable.Rows[0];
                partAct.Id = row["row_id"] == DBNull.Value ? 0 : int.Parse(row["row_id"].ToString());

                partAct.Activity.Id = row["act_id"] == DBNull.Value ? string.Empty : row["act_id"].ToString();
                partAct.Activity.Name = row["act_name"] == DBNull.Value ? string.Empty : row["act_name"].ToString();
                partAct.Activity.Type = row["act_type"] == DBNull.Value ? string.Empty : row["act_type"].ToString();
                partAct.Activity.Order = row["act_order"] == DBNull.Value ? 0 : int.Parse(row["act_order"].ToString());
                partAct.Activity.Internal = row["internal_use"] == DBNull.Value ? false : row["internal_use"].ToString() == "1" ? true : false;

                partAct.FromRole.Id = row["fromroleid"] == DBNull.Value ? 0 : int.Parse(row["fromroleid"].ToString());
                partAct.FromRole.Name = row["fromrolename"] == DBNull.Value ? string.Empty : row["fromrolename"].ToString();
                partAct.FromRole.IsActive = row["fromroleisactive"] == DBNull.Value ? false : row["fromroleisactive"].ToString() == "1" ? true : false;
                partAct.FromRole.Weight = row["fromroleweight"] == DBNull.Value ? 0 : int.Parse(row["fromroleweight"].ToString());
                partAct.FromRole.Order = row["fromroleorder"] == DBNull.Value ? byte.MinValue : byte.Parse(row["fromroleorder"].ToString());
                partAct.FromRole.Code = row["fromordercode"] == DBNull.Value ? string.Empty : row["fromordercode"].ToString();

                partAct.MaxQueryDuration.Id = row["queryduration"] == DBNull.Value ? string.Empty : row["queryduration"].ToString();
                partAct.MaxQueryDuration.Name = row["queryduration_name"] == DBNull.Value ? string.Empty : row["queryduration_name"].ToString();
                partAct.Scope.Id = row["act_scope"] == DBNull.Value ? string.Empty : row["act_scope"].ToString();
                partAct.Scope.Name = row["act_scope_name"] == DBNull.Value ? string.Empty : row["act_scope_name"].ToString();
                partAct.MaxQueryRows = row["maxrec"] == DBNull.Value ? 0 : int.Parse(row["maxrec"].ToString());
                partAct.OnlyPartnerChildren = row["onlypartchildren"] == DBNull.Value ? false : row["onlypartchildren"].ToString() == "1" ? true : false;
                partAct.CreatedOn = row["createdon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["createdon"].ToString());
                partAct.LastEditOn = row["lastediton"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["lastediton"].ToString());
                partAct.CreatedBy.Id = row["createdby"] == DBNull.Value ? string.Empty : row["createdby"].ToString();
                partAct.CreatedBy.Account = row["createdbyacc"] == DBNull.Value ? -1 : int.Parse(row["createdbyacc"].ToString());
                partAct.CreatedBy.Name = row["createdname"] == DBNull.Value ? string.Empty : row["createdname"].ToString();
                partAct.Details = GetDetails(partAct.Id, toRoleId);

                return partAct;
            }
            else
            {
                return null;
            }
            
        }

        public PartnerActivity GetPartAct(int id)
        {
            var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "pactrowid", OracleDbType = OracleDbType.Int32,  Value = id }
            };

            var actDataTable = this.db.GetData("Select * from v_partner_activity where row_id=:pactrowid ", parameters);
            var partAct = new PartnerActivity();
            if (actDataTable != null)
            {
                DataRow row = actDataTable.Rows[0];
                partAct.Id = row["row_id"] == DBNull.Value ? 0 : int.Parse(row["row_id"].ToString());

                partAct.Activity.Id = row["act_id"] == DBNull.Value ? string.Empty : row["act_id"].ToString();
                partAct.Activity.Name = row["act_name"] == DBNull.Value ? string.Empty : row["act_name"].ToString();
                partAct.Activity.Type = row["act_type"] == DBNull.Value ? string.Empty : row["act_type"].ToString();
                partAct.Activity.Order = row["act_order"] == DBNull.Value ? 0 : int.Parse(row["act_order"].ToString());
                partAct.Activity.Internal = row["internal_use"] == DBNull.Value ? false : row["internal_use"].ToString() == "1" ? true : false;

                partAct.FromRole.Id = row["fromroleid"] == DBNull.Value ? 0 : int.Parse(row["fromroleid"].ToString());
                partAct.FromRole.Name = row["fromrolename"] == DBNull.Value ? string.Empty : row["fromrolename"].ToString();
                partAct.FromRole.IsActive = row["fromroleisactive"] == DBNull.Value ? false : row["fromroleisactive"].ToString() == "1" ? true : false;
                partAct.FromRole.Weight = row["fromroleweight"] == DBNull.Value ? 0 : int.Parse(row["fromroleweight"].ToString());
                partAct.FromRole.Order = row["fromroleorder"] == DBNull.Value ? byte.MinValue : byte.Parse(row["fromroleorder"].ToString());
                partAct.FromRole.Code = row["fromordercode"] == DBNull.Value ? string.Empty : row["fromordercode"].ToString();

                partAct.MaxQueryDuration.Id = row["queryduration"] == DBNull.Value ? string.Empty : row["queryduration"].ToString();
                partAct.MaxQueryDuration.Name = row["queryduration_name"] == DBNull.Value ? string.Empty : row["queryduration_name"].ToString();
                partAct.Scope.Id = row["act_scope"] == DBNull.Value ? string.Empty : row["act_scope"].ToString();
                partAct.Scope.Name = row["act_scope_name"] == DBNull.Value ? string.Empty : row["act_scope_name"].ToString();
                partAct.MaxQueryRows = row["maxrec"] == DBNull.Value ? 0 : int.Parse(row["maxrec"].ToString());
                partAct.OnlyPartnerChildren = row["onlypartchildren"] == DBNull.Value ? false : row["onlypartchildren"].ToString() == "1" ? true : false;
                partAct.CreatedOn = row["createdon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["createdon"].ToString());
                partAct.LastEditOn = row["lastediton"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["lastediton"].ToString());
                partAct.CreatedBy.Id = row["createdby"] == DBNull.Value ? string.Empty : row["createdby"].ToString();
                partAct.CreatedBy.Account = row["createdbyacc"] == DBNull.Value ? -1 : int.Parse(row["createdbyacc"].ToString());
                partAct.CreatedBy.Name = row["createdname"] == DBNull.Value ? string.Empty : row["createdname"].ToString();
                partAct.Details = GetDetails(partAct.Id);

            }
            return partAct;
        }

        private List<PartnerActivityDetail> GetDetails(int id, bool withMaster = false)
        {
            var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "MasetrRowId", OracleDbType = OracleDbType.Int32,  Value = id }
            };
            var masterDataTable = this.db.GetData("Select * from V_PARTNER_ACTIVITY_DETAIL  where master_row=:MasetrRowId order by row_id", parameters);

            var actDetails = new List<PartnerActivityDetail>();
            if (masterDataTable != null)
            {
                foreach (DataRow row in masterDataTable.Rows)
                {
                    var partAct = new PartnerActivityDetail();
                    partAct.Id = row["detail_row_id"] == DBNull.Value ? 0 : int.Parse(row["detail_row_id"].ToString());

                    partAct.ParentId = row["master_row"] == DBNull.Value ? 0 : int.Parse(row["master_row"].ToString());

                    partAct.ToRole.Id = row["toroleid"] == DBNull.Value ? 0 : int.Parse(row["toroleid"].ToString());
                    partAct.ToRole.Name = row["torolename"] == DBNull.Value ? string.Empty : row["torolename"].ToString();
                    partAct.ToRole.IsActive = row["toroleisactive"] == DBNull.Value ? false : row["toroleisactive"].ToString() == "1" ? true : false;
                    partAct.ToRole.Weight = row["toroleweight"] == DBNull.Value ? 0 : int.Parse(row["toroleweight"].ToString());
                    partAct.ToRole.Order = row["toroleorder"] == DBNull.Value ? byte.MinValue : byte.Parse(row["toroleorder"].ToString());
                    partAct.ToRole.Code = row["torolecode"] == DBNull.Value ? string.Empty : row["torolecode"].ToString();

                    partAct.CheckBalanceRequired = row["check_bal"] == DBNull.Value ? true : row["check_bal"].ToString() == "1" ? true  : false;
                    partAct.MaxValue = row["max_value"] == DBNull.Value ? 0 : int.Parse(row["max_value"].ToString());
                    partAct.MinValue = row["min_value"] == DBNull.Value ? 0 : int.Parse(row["min_value"].ToString());
                    partAct.BonusPercent = row["bonus_per"] == DBNull.Value ? 0 : double.Parse(row["bonus_per"].ToString());
                    partAct.BonusTaxPercent = row["bonus_tax"] == DBNull.Value ? 0 : double.Parse(row["bonus_tax"].ToString());
                    partAct.TaxPercent = row["taxper"] == DBNull.Value ? 0 : double.Parse(row["taxper"].ToString());
                    partAct.CreatedOn = row["createdon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["createdon"].ToString());
                    partAct.LastEditOn = row["lastediton"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["lastediton"].ToString());
                    partAct.CreatedBy.Id = row["createdby"] == DBNull.Value ? string.Empty : row["createdby"].ToString();
                    partAct.CreatedBy.Account = row["createdbyacc"] == DBNull.Value ? -1 : int.Parse(row["createdbyacc"].ToString());
                    partAct.CreatedBy.Name = row["createdname"] == DBNull.Value ? string.Empty : row["createdname"].ToString();

                    if (withMaster)  partAct.Parent = GetPartAct(partAct.ParentId);

                    actDetails.Add(partAct);
                }
            }
            return actDetails;
        }
        private List<PartnerActivityDetail> GetDetails(int id, int toRoleId, bool withMaster = false)
        {
            var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "MasetrRowId", OracleDbType = OracleDbType.Int32,  Value = id },
                 new OracleParameter{ ParameterName = "ToRoleId", OracleDbType = OracleDbType.Int32,  Value = toRoleId }
            };
            var masterDataTable = this.db.GetData("Select * from v_partner_activity_detail  where master_row=:MasetrRowId and toroleid=:ToRoleId order by row_id", parameters);

            var actDetails = new List<PartnerActivityDetail>();
            if (masterDataTable != null)
            {
                foreach (DataRow row in masterDataTable.Rows)
                {
                    var partAct = new PartnerActivityDetail();
                    partAct.Id = row["detail_row_id"] == DBNull.Value ? 0 : int.Parse(row["detail_row_id"].ToString());

                    partAct.ParentId = row["master_row"] == DBNull.Value ? 0 : int.Parse(row["master_row"].ToString());

                    partAct.ToRole.Id = row["toroleid"] == DBNull.Value ? 0 : int.Parse(row["toroleid"].ToString());
                    partAct.ToRole.Name = row["torolename"] == DBNull.Value ? string.Empty : row["torolename"].ToString();
                    partAct.ToRole.IsActive = row["toroleisactive"] == DBNull.Value ? false : row["toroleisactive"].ToString() == "1" ? true : false;
                    partAct.ToRole.Weight = row["toroleweight"] == DBNull.Value ? 0 : int.Parse(row["toroleweight"].ToString());
                    partAct.ToRole.Order = row["toroleorder"] == DBNull.Value ? byte.MinValue : byte.Parse(row["toroleorder"].ToString());
                    partAct.ToRole.Code = row["torolecode"] == DBNull.Value ? string.Empty : row["torolecode"].ToString();

                    partAct.CheckBalanceRequired = row["check_bal"] == DBNull.Value ? true : row["check_bal"].ToString() == "1" ? true : false;
                    partAct.MaxValue = row["max_value"] == DBNull.Value ? 0 : int.Parse(row["max_value"].ToString());
                    partAct.MinValue = row["min_value"] == DBNull.Value ? 0 : int.Parse(row["min_value"].ToString());
                    partAct.BonusPercent = row["bonus_per"] == DBNull.Value ? 0 : double.Parse(row["bonus_per"].ToString());
                    partAct.BonusTaxPercent = row["bonus_tax"] == DBNull.Value ? 0 : double.Parse(row["bonus_tax"].ToString());
                    partAct.TaxPercent = row["taxper"] == DBNull.Value ? 0 : double.Parse(row["taxper"].ToString());
                    partAct.CreatedOn = row["createdon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["createdon"].ToString());
                    partAct.LastEditOn = row["lastediton"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["lastediton"].ToString());
                    partAct.CreatedBy.Id = row["createdby"] == DBNull.Value ? string.Empty : row["createdby"].ToString();
                    partAct.CreatedBy.Account = row["createdbyacc"] == DBNull.Value ? -1 : int.Parse(row["createdbyacc"].ToString());
                    partAct.CreatedBy.Name = row["createdname"] == DBNull.Value ? string.Empty : row["createdname"].ToString();

                    if (withMaster) partAct.Parent = GetPartAct(partAct.ParentId);

                    actDetails.Add(partAct);
                }
            }
            return actDetails;
        }

        private PartnerActivityDetail GetDetail(int id, int parentId, bool withMaster = false)
        {
            var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "Row_Id", OracleDbType = OracleDbType.Int32,  Value = id },
                 new OracleParameter{ ParameterName = "parentId", OracleDbType = OracleDbType.Int32,  Value = parentId }
            };
            var masterDataTable = this.db.GetData("Select * from V_PARTNER_ACTIVITY_DETAIL  where detail_row_id=:Row_Id and row_id=:parentId ", parameters);

            var partAct = new PartnerActivityDetail();
            if (masterDataTable != null)
            {
                DataRow row = masterDataTable.Rows[0];
                    
                    partAct.Id = row["detail_row_id"] == DBNull.Value ? 0 : int.Parse(row["detail_row_id"].ToString());

                    partAct.ParentId = row["master_row"] == DBNull.Value ? 0 : int.Parse(row["master_row"].ToString());

                    partAct.ToRole.Id = row["toroleid"] == DBNull.Value ? 0 : int.Parse(row["toroleid"].ToString());
                    partAct.ToRole.Name = row["torolename"] == DBNull.Value ? string.Empty : row["torolename"].ToString();
                    partAct.ToRole.IsActive = row["toroleisactive"] == DBNull.Value ? false : row["toroleisactive"].ToString() == "1" ? true : false;
                    partAct.ToRole.Weight = row["toroleweight"] == DBNull.Value ? 0 : int.Parse(row["toroleweight"].ToString());
                    partAct.ToRole.Order = row["toroleorder"] == DBNull.Value ? byte.MinValue : byte.Parse(row["toroleorder"].ToString());
                    partAct.ToRole.Code = row["torolecode"] == DBNull.Value ? string.Empty : row["torolecode"].ToString();

                    partAct.CheckBalanceRequired = row["check_bal"] == DBNull.Value ? true : row["check_bal"].ToString() == "1" ? true : false;
                    partAct.MaxValue = row["max_value"] == DBNull.Value ? 0 : int.Parse(row["max_value"].ToString());
                    partAct.MinValue = row["min_value"] == DBNull.Value ? 0 : int.Parse(row["min_value"].ToString());
                    partAct.BonusPercent = row["bonus_per"] == DBNull.Value ? 0 : double.Parse(row["bonus_per"].ToString());
                    partAct.BonusTaxPercent = row["bonus_tax"] == DBNull.Value ? 0 : double.Parse(row["bonus_tax"].ToString());
                    partAct.TaxPercent = row["taxper"] == DBNull.Value ? 0 : double.Parse(row["taxper"].ToString());
                    partAct.CreatedOn = row["createdon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["createdon"].ToString());
                    partAct.LastEditOn = row["lastediton"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["lastediton"].ToString());
                    partAct.CreatedBy.Id = row["createdby"] == DBNull.Value ? string.Empty : row["createdby"].ToString();
                    partAct.CreatedBy.Account = row["createdbyacc"] == DBNull.Value ? -1 : int.Parse(row["createdbyacc"].ToString());
                    partAct.CreatedBy.Name = row["createdname"] == DBNull.Value ? string.Empty : row["createdname"].ToString();

                    if (withMaster) partAct.Parent = GetPartAct(partAct.ParentId);

            }
            return partAct;
        }
        private PartnerActivityDetail GetDetail(string actId, int fromRoleId, int toRoleId, bool withMaster = false)
        {
            var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "actId", OracleDbType = OracleDbType.Varchar2,  Value = actId },
                 new OracleParameter{ ParameterName = "fromId", OracleDbType = OracleDbType.Int32,  Value = fromRoleId },
                 new OracleParameter{ ParameterName = "toId", OracleDbType = OracleDbType.Int32,  Value = toRoleId }
            };
            var masterDataTable = this.db.GetData("Select * from V_PARTNER_ACTIVITY_DETAIL  where act_id=:actId and fromroleid=:fromId and toroleid=:toId ", parameters);

            var partAct = new PartnerActivityDetail();
            if (masterDataTable != null)
            {
                DataRow row = masterDataTable.Rows[0];

                partAct.Id = row["detail_row_id"] == DBNull.Value ? 0 : int.Parse(row["detail_row_id"].ToString());

                partAct.ParentId = row["master_row"] == DBNull.Value ? 0 : int.Parse(row["master_row"].ToString());

                partAct.ToRole.Id = row["toroleid"] == DBNull.Value ? 0 : int.Parse(row["toroleid"].ToString());
                partAct.ToRole.Name = row["torolename"] == DBNull.Value ? string.Empty : row["torolename"].ToString();
                partAct.ToRole.IsActive = row["toroleisactive"] == DBNull.Value ? false : row["toroleisactive"].ToString() == "1" ? true : false;
                partAct.ToRole.Weight = row["toroleweight"] == DBNull.Value ? 0 : int.Parse(row["toroleweight"].ToString());
                partAct.ToRole.Order = row["toroleorder"] == DBNull.Value ? byte.MinValue : byte.Parse(row["toroleorder"].ToString());
                partAct.ToRole.Code = row["torolecode"] == DBNull.Value ? string.Empty : row["torolecode"].ToString();

                partAct.CheckBalanceRequired = row["check_bal"] == DBNull.Value ? true : row["check_bal"].ToString() == "1" ? true : false;
                partAct.MaxValue = row["max_value"] == DBNull.Value ? 0 : int.Parse(row["max_value"].ToString());
                partAct.MinValue = row["min_value"] == DBNull.Value ? 0 : int.Parse(row["min_value"].ToString());
                partAct.BonusPercent = row["bonus_per"] == DBNull.Value ? 0 : double.Parse(row["bonus_per"].ToString());
                partAct.BonusTaxPercent = row["bonus_tax"] == DBNull.Value ? 0 : double.Parse(row["bonus_tax"].ToString());
                partAct.TaxPercent = row["taxper"] == DBNull.Value ? 0 : double.Parse(row["taxper"].ToString());
                partAct.CreatedOn = row["createdon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["createdon"].ToString());
                partAct.LastEditOn = row["lastediton"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["lastediton"].ToString());
                partAct.CreatedBy.Id = row["createdby"] == DBNull.Value ? string.Empty : row["createdby"].ToString();
                partAct.CreatedBy.Account = row["createdbyacc"] == DBNull.Value ? -1 : int.Parse(row["createdbyacc"].ToString());
                partAct.CreatedBy.Name = row["createdname"] == DBNull.Value ? string.Empty : row["createdname"].ToString();

                if (withMaster) partAct.Parent = GetPartAct(partAct.ParentId);

            }
            return partAct;
        }
      
    }

}
