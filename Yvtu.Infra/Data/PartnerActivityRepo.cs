using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.Infra.Data
{
    public class PartnerActivityRepo : IPartnerActivityRepo
    {
        private readonly IAppDbContext db;

        public PartnerActivityRepo(IAppDbContext db)
        {
            this.db = db;
        }

        public bool Delete(int id)
        {
            var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "Row_Id", OracleDbType = OracleDbType.Int32,  Value = id }
            };
            return db.ExecuteSqlCommand("Delete from Partner_Activity Where Row_Id=:Row_Id", parameters) > 0;
        }

        public async Task<OpertionResult> CreateAsync(PartnerActivity partnerActivity)
        {
            var insertSql = "insert into partner_activity " +
                "   ( act_id, fromroleid, toroleid, check_bal, max_value, min_value, bonus_per " +
                "   , taxper, bonus_tax, queryduration, act_scope, maxrec, onlypartchildren, createdon, createdby, lastediton) values " +
                "(:v_act_id, :v_fromroleid, :v_toroleid, :v_check_bal, :v_max_value, :v_min_value, :v_bonus_per " +
                "  , :v_taxper, :v_bonus_tax, :v_queryduration, :v_act_scope, :v_maxrec, :v_onlypartchildren, sysdate, :v_createdby, sysdate) ";
            try
            {
                #region Parameters
                var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "v_act_id", OracleDbType = OracleDbType.Varchar2,  Value = partnerActivity.Activity.Id },
                 new OracleParameter{ ParameterName = "v_fromroleid",OracleDbType = OracleDbType.Int32,  Value = partnerActivity.FromRole.Id },
                 new OracleParameter{ ParameterName = "v_toroleid",OracleDbType = OracleDbType.Int32,  Value = partnerActivity.ToRole.Id },
                 new OracleParameter{ ParameterName = "v_check_bal",OracleDbType = OracleDbType.Int32,  Value = partnerActivity.CheckBalanceRequired ? 1 : 0 },
                 new OracleParameter{ ParameterName = "v_max_value",OracleDbType = OracleDbType.Int32,  Value = partnerActivity.MaxValue },
                 new OracleParameter{ ParameterName = "v_min_value",OracleDbType = OracleDbType.Int32,  Value = partnerActivity.MinValue },
                 new OracleParameter{ ParameterName = "v_bonus_per",OracleDbType = OracleDbType.Decimal,  Value = partnerActivity.BonusPercent },
                 new OracleParameter{ ParameterName = "v_taxper",OracleDbType = OracleDbType.Decimal,  Value = partnerActivity.TaxPercent },
                 new OracleParameter{ ParameterName = "v_bonus_tax",OracleDbType = OracleDbType.Decimal,  Value = partnerActivity.BonusTaxPercent },
                 new OracleParameter{ ParameterName = "v_queryduration",OracleDbType = OracleDbType.Varchar2,  Value = partnerActivity.MaxQueryDuration.Id },
                 new OracleParameter{ ParameterName = "v_act_scope",OracleDbType = OracleDbType.Varchar2,  Value = partnerActivity.Scope.Id },
                 new OracleParameter{ ParameterName = "v_maxrec",OracleDbType = OracleDbType.Int32,  Value = partnerActivity.MaxQueryRows },
                 new OracleParameter{ ParameterName = "v_onlypartchildren",OracleDbType = OracleDbType.Int32,  Value = partnerActivity.OnlyPartnerChildren ? 1 : 0 },
                 new OracleParameter{ ParameterName = "v_createdby",OracleDbType = OracleDbType.Varchar2,  Value = partnerActivity.CreatedBy.Id }
                };
                #endregion
                var result = await db.ExecuteSqlCommandAsync(insertSql, parameters);

                if (result > 0)
                {
                    return new OpertionResult { AffectedCount = result, Success = true, Error = string.Empty };
                }
                else
                {
                    return new OpertionResult { AffectedCount = result, Success = false, Error = string.Empty };
                }
            }
            catch (Exception ex)
            {
                return new OpertionResult { AffectedCount = -1, Success = false, Error = ex.Message };
            }
        }
        public async Task<OpertionResult> EditAsync(PartnerActivity partnerActivity)
        {
            var updateSql = "update partner_activity " +
                "   set   act_id = :v_act_id, fromroleid = :v_fromroleid, toroleid = :v_toroleid, check_bal = :v_check_bal, max_value = :v_max_value, " +
                "   min_value = :v_min_value, bonus_per = :v_bonus_per, taxper = :v_taxper, bonus_tax = :v_bonus_tax, queryduration = :v_queryduration, " +
                "act_scope = :v_act_scope, maxrec = :v_maxrec, onlypartchildren = :v_onlypartchildren, lastediton = sysdate " +
                "  where row_id = :v_row_id ";
            try
            {
                #region Parameters
                var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "v_act_id", OracleDbType = OracleDbType.Varchar2,  Value = partnerActivity.Activity.Id },
                 new OracleParameter{ ParameterName = "v_fromroleid",OracleDbType = OracleDbType.Int32,  Value = partnerActivity.FromRole.Id },
                 new OracleParameter{ ParameterName = "v_toroleid",OracleDbType = OracleDbType.Int32,  Value = partnerActivity.ToRole.Id },
                 new OracleParameter{ ParameterName = "v_check_bal",OracleDbType = OracleDbType.Int32,  Value = partnerActivity.CheckBalanceRequired ? 1 : 0 },
                 new OracleParameter{ ParameterName = "v_max_value",OracleDbType = OracleDbType.Int32,  Value = partnerActivity.MaxValue },
                 new OracleParameter{ ParameterName = "v_min_value",OracleDbType = OracleDbType.Int32,  Value = partnerActivity.MinValue },
                 new OracleParameter{ ParameterName = "v_bonus_per",OracleDbType = OracleDbType.Decimal,  Value = partnerActivity.BonusPercent },
                 new OracleParameter{ ParameterName = "v_taxper",OracleDbType = OracleDbType.Decimal,  Value = partnerActivity.TaxPercent },
                 new OracleParameter{ ParameterName = "v_bonus_tax",OracleDbType = OracleDbType.Decimal,  Value = partnerActivity.BonusTaxPercent },
                 new OracleParameter{ ParameterName = "v_queryduration",OracleDbType = OracleDbType.Varchar2,  Value = partnerActivity.MaxQueryDuration.Id },
                 new OracleParameter{ ParameterName = "v_act_scope",OracleDbType = OracleDbType.Varchar2,  Value = partnerActivity.Scope.Id },
                 new OracleParameter{ ParameterName = "v_maxrec",OracleDbType = OracleDbType.Int32,  Value = partnerActivity.MaxQueryRows },
                 new OracleParameter{ ParameterName = "v_onlypartchildren",OracleDbType = OracleDbType.Int32,  Value = partnerActivity.OnlyPartnerChildren ? 1 : 0 },
                 new OracleParameter{ ParameterName = "v_row_id",OracleDbType = OracleDbType.Int32,  Value = partnerActivity.Id }
                };
                #endregion
                var result = await db.ExecuteSqlCommandAsync(updateSql, parameters);

                if (result > 0)
                {
                    return new OpertionResult { AffectedCount = result, Success = true, Error = string.Empty };
                }
                else
                {
                    return new OpertionResult { AffectedCount = result, Success = false, Error = string.Empty };
                }
            }
            catch (Exception ex)
            {
                return new OpertionResult { AffectedCount = -1, Success = false, Error = ex.Message };
            }
        }

        public List<PartnerActivity> GetAllList()
        {
            var actDataTable = this.db.GetData("Select * from v_partner_activity  order by act_type,fromroleorder", null);

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

                    partAct.ToRole.Id = row["toroleid"] == DBNull.Value ? int.MinValue : int.Parse(row["toroleid"].ToString());
                    partAct.ToRole.Name = row["torolename"] == DBNull.Value ? string.Empty : row["torolename"].ToString();
                    partAct.ToRole.IsActive = row["toroleisactive"] == DBNull.Value ? false : row["toroleisactive"].ToString() == "1" ? true : false;
                    partAct.ToRole.Weight = row["toroleweight"] == DBNull.Value ? 0 : int.Parse(row["toroleweight"].ToString());
                    partAct.ToRole.Order = row["toroleorder"] == DBNull.Value ? byte.MinValue : byte.Parse(row["toroleorder"].ToString());
                    partAct.ToRole.Code = row["torolecode"] == DBNull.Value ? string.Empty : row["torolecode"].ToString();

                    partAct.CheckBalanceRequired = row["check_bal"] == DBNull.Value ? false : row["check_bal"].ToString() == "1" ? true : false;
                    partAct.MaxValue = row["max_value"] == DBNull.Value ? 0 : int.Parse(row["max_value"].ToString());
                    partAct.MinValue = row["min_value"] == DBNull.Value ? 0 : int.Parse(row["min_value"].ToString());
                    partAct.BonusPercent = row["bonus_per"] == DBNull.Value ? 0 : double.Parse(row["bonus_per"].ToString());
                    partAct.TaxPercent = row["taxper"] == DBNull.Value ? 0 : double.Parse(row["taxper"].ToString());
                    partAct.BonusTaxPercent = row["bonus_tax"] == DBNull.Value ? 0 : double.Parse(row["bonus_tax"].ToString());
                    partAct.MaxQueryDuration.Id = row["queryduration"] == DBNull.Value ? string.Empty : row["queryduration"].ToString();
                    partAct.MaxQueryDuration.Name = row["queryduration_name"] == DBNull.Value ? string.Empty : row["queryduration_name"].ToString();
                    partAct.Scope.Id = row["act_scope"] == DBNull.Value ? string.Empty : row["act_scope"].ToString();
                    partAct.Scope.Name = row["act_scope_name"] == DBNull.Value ? string.Empty : row["act_scope_name"].ToString();
                    partAct.MaxQueryRows = row["maxrec"] == DBNull.Value ? 0 : int.Parse(row["maxrec"].ToString());
                    partAct.OnlyPartnerChildren = row["onlypartchildren"] == DBNull.Value ? false : row["onlypartchildren"].ToString() == "1" ? true : false;
                    partAct.CreatedOn = row["createdon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["createdon"].ToString());
                    partAct.LastEditOn = row["lastediton"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["lastediton"].ToString());
                    partAct.CreatedBy.Id = row["createdby"] == DBNull.Value ? string.Empty : row["createdby"].ToString();
                    partAct.CreatedBy.Name = row["createdname"] == DBNull.Value ? string.Empty : row["createdname"].ToString();

                    activities.Add(partAct);
                }
            }
            return activities;
        }

        public List<PartnerActivity> GetListByActivity(string activityId)
        {
            var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "activityId", OracleDbType = OracleDbType.Varchar2,  Value = activityId }
            };
            var actDataTable = this.db.GetData("Select * from v_partner_activity  where act_id=:activityId order by fromroleid,fromroleorder ", parameters);

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

                    partAct.ToRole.Id = row["toroleid"] == DBNull.Value ? int.MinValue : int.Parse(row["toroleid"].ToString());
                    partAct.ToRole.Name = row["torolename"] == DBNull.Value ? string.Empty : row["torolename"].ToString();
                    partAct.ToRole.IsActive = row["toroleisactive"] == DBNull.Value ? false : row["toroleisactive"].ToString() == "1" ? true : false;
                    partAct.ToRole.Weight = row["toroleweight"] == DBNull.Value ? 0 : int.Parse(row["toroleweight"].ToString());
                    partAct.ToRole.Order = row["toroleorder"] == DBNull.Value ? byte.MinValue : byte.Parse(row["toroleorder"].ToString());
                    partAct.ToRole.Code = row["torolecode"] == DBNull.Value ? string.Empty : row["torolecode"].ToString();

                    partAct.CheckBalanceRequired = row["check_bal"] == DBNull.Value ? false : row["check_bal"].ToString() == "1" ? true : false;
                    partAct.MaxValue = row["max_value"] == DBNull.Value ? 0 : int.Parse(row["max_value"].ToString());
                    partAct.MinValue = row["min_value"] == DBNull.Value ? 0 : int.Parse(row["min_value"].ToString());
                    partAct.BonusPercent = row["bonus_per"] == DBNull.Value ? 0 : double.Parse(row["bonus_per"].ToString());
                    partAct.TaxPercent = row["taxper"] == DBNull.Value ? 0 : double.Parse(row["taxper"].ToString());
                    partAct.BonusTaxPercent = row["bonus_tax"] == DBNull.Value ? 0 : double.Parse(row["bonus_tax"].ToString());
                    partAct.MaxQueryDuration.Id = row["queryduration"] == DBNull.Value ? string.Empty : row["queryduration"].ToString();
                    partAct.MaxQueryDuration.Name = row["queryduration_name"] == DBNull.Value ? string.Empty : row["queryduration_name"].ToString();
                    partAct.Scope.Id = row["act_scope"] == DBNull.Value ? string.Empty : row["act_scope"].ToString();
                    partAct.Scope.Name = row["act_scope_name"] == DBNull.Value ? string.Empty : row["act_scope_name"].ToString();
                    partAct.MaxQueryRows = row["maxrec"] == DBNull.Value ? 0 : int.Parse(row["maxrec"].ToString());
                    partAct.OnlyPartnerChildren = row["onlypartchildren"] == DBNull.Value ? false : row["onlypartchildren"].ToString() == "1" ? true : false;
                    partAct.CreatedOn = row["createdon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["createdon"].ToString());
                    partAct.LastEditOn = row["lastediton"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["lastediton"].ToString());
                    partAct.CreatedBy.Id = row["createdby"] == DBNull.Value ? string.Empty : row["createdby"].ToString();
                    partAct.CreatedBy.Name = row["createdname"] == DBNull.Value ? string.Empty : row["createdname"].ToString();

                    activities.Add(partAct);
                }
            }
            return activities;
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

                    partAct.ToRole.Id = row["toroleid"] == DBNull.Value ? int.MinValue : int.Parse(row["toroleid"].ToString());
                    partAct.ToRole.Name = row["torolename"] == DBNull.Value ? string.Empty : row["torolename"].ToString();
                    partAct.ToRole.IsActive = row["toroleisactive"] == DBNull.Value ? false : row["toroleisactive"].ToString() == "1" ? true : false;
                    partAct.ToRole.Weight = row["toroleweight"] == DBNull.Value ? 0 : int.Parse(row["toroleweight"].ToString());
                    partAct.ToRole.Order = row["toroleorder"] == DBNull.Value ? byte.MinValue : byte.Parse(row["toroleorder"].ToString());
                    partAct.ToRole.Code = row["torolecode"] == DBNull.Value ? string.Empty : row["torolecode"].ToString();

                    partAct.CheckBalanceRequired = row["check_bal"] == DBNull.Value ? false : row["check_bal"].ToString() == "1" ? true : false;
                    partAct.MaxValue = row["max_value"] == DBNull.Value ? 0 : int.Parse(row["max_value"].ToString());
                    partAct.MinValue = row["min_value"] == DBNull.Value ? 0 : int.Parse(row["min_value"].ToString());
                    partAct.BonusPercent = row["bonus_per"] == DBNull.Value ? 0 : double.Parse(row["bonus_per"].ToString());
                    partAct.TaxPercent = row["taxper"] == DBNull.Value ? 0 : double.Parse(row["taxper"].ToString());
                    partAct.BonusTaxPercent = row["bonus_tax"] == DBNull.Value ? 0 : double.Parse(row["bonus_tax"].ToString());
                    partAct.MaxQueryDuration.Id = row["queryduration"] == DBNull.Value ? string.Empty : row["queryduration"].ToString();
                    partAct.MaxQueryDuration.Name = row["queryduration_name"] == DBNull.Value ? string.Empty : row["queryduration_name"].ToString();
                    partAct.Scope.Id = row["act_scope"] == DBNull.Value ? string.Empty : row["act_scope"].ToString();
                    partAct.Scope.Name = row["act_scope_name"] == DBNull.Value ? string.Empty : row["act_scope_name"].ToString();
                    partAct.MaxQueryRows = row["maxrec"] == DBNull.Value ? 0 : int.Parse(row["maxrec"].ToString());
                    partAct.OnlyPartnerChildren = row["onlypartchildren"] == DBNull.Value ? false : row["onlypartchildren"].ToString() == "1" ? true : false;
                    partAct.CreatedOn = row["createdon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["createdon"].ToString());
                    partAct.LastEditOn = row["lastediton"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["lastediton"].ToString());
                    partAct.CreatedBy.Id = row["createdby"] == DBNull.Value ? string.Empty : row["createdby"].ToString();
                    partAct.CreatedBy.Name = row["createdname"] == DBNull.Value ? string.Empty : row["createdname"].ToString();

                    activities.Add(partAct);
                }
            }
            return activities;
        }

        public PartnerActivity GetPartAct(string actId, string fromRoleId, string toRoleId)
        {
            var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "actId", OracleDbType = OracleDbType.Varchar2,  Value = actId },
                 new OracleParameter{ ParameterName = "fromRoleId", OracleDbType = OracleDbType.Int32,  Value = fromRoleId },
                 new OracleParameter{ ParameterName = "toRoleId", OracleDbType = OracleDbType.Int32,  Value = toRoleId },
            };
            var actDataTable = this.db.GetData("Select * from v_partner_activity  where act_id=:actId and fromroleid=:fromroleid and toroleid=:toroleid ", parameters);
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

                partAct.ToRole.Id = row["toroleid"] == DBNull.Value ? int.MinValue : int.Parse(row["toroleid"].ToString());
                partAct.ToRole.Name = row["torolename"] == DBNull.Value ? string.Empty : row["torolename"].ToString();
                partAct.ToRole.IsActive = row["toroleisactive"] == DBNull.Value ? false : row["toroleisactive"].ToString() == "1" ? true : false;
                partAct.ToRole.Weight = row["toroleweight"] == DBNull.Value ? 0 : int.Parse(row["toroleweight"].ToString());
                partAct.ToRole.Order = row["toroleorder"] == DBNull.Value ? byte.MinValue : byte.Parse(row["toroleorder"].ToString());
                partAct.ToRole.Code = row["torolecode"] == DBNull.Value ? string.Empty : row["torolecode"].ToString();

                partAct.CheckBalanceRequired = row["check_bal"] == DBNull.Value ? false : row["check_bal"].ToString() == "1" ? true : false;
                partAct.MaxValue = row["max_value"] == DBNull.Value ? 0 : int.Parse(row["max_value"].ToString());
                partAct.MinValue = row["min_value"] == DBNull.Value ? 0 : int.Parse(row["min_value"].ToString());
                partAct.BonusPercent = row["bonus_per"] == DBNull.Value ? 0 : double.Parse(row["bonus_per"].ToString());
                partAct.TaxPercent = row["taxper"] == DBNull.Value ? 0 : double.Parse(row["taxper"].ToString());
                partAct.BonusTaxPercent = row["bonus_tax"] == DBNull.Value ? 0 : double.Parse(row["bonus_tax"].ToString());
                partAct.MaxQueryDuration.Id = row["queryduration"] == DBNull.Value ? string.Empty : row["queryduration"].ToString();
                partAct.MaxQueryDuration.Name = row["queryduration_name"] == DBNull.Value ? string.Empty : row["queryduration_name"].ToString();
                partAct.Scope.Id = row["act_scope"] == DBNull.Value ? string.Empty : row["act_scope"].ToString();
                partAct.Scope.Name = row["act_scope_name"] == DBNull.Value ? string.Empty : row["act_scope_name"].ToString();
                partAct.MaxQueryRows = row["maxrec"] == DBNull.Value ? 0 : int.Parse(row["maxrec"].ToString());
                partAct.OnlyPartnerChildren = row["onlypartchildren"] == DBNull.Value ? false : row["onlypartchildren"].ToString() == "1" ? true : false;
                partAct.CreatedOn = row["createdon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["createdon"].ToString());
                partAct.LastEditOn = row["lastediton"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["lastediton"].ToString());
                partAct.CreatedBy.Id = row["createdby"] == DBNull.Value ? string.Empty : row["createdby"].ToString();
                partAct.CreatedBy.Name = row["createdname"] == DBNull.Value ? string.Empty : row["createdname"].ToString();
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

                partAct.ToRole.Id = row["toroleid"] == DBNull.Value ? int.MinValue : int.Parse(row["toroleid"].ToString());
                partAct.ToRole.Name = row["torolename"] == DBNull.Value ? string.Empty : row["torolename"].ToString();
                partAct.ToRole.IsActive = row["toroleisactive"] == DBNull.Value ? false : row["toroleisactive"].ToString() == "1" ? true : false;
                partAct.ToRole.Weight = row["toroleweight"] == DBNull.Value ? 0 : int.Parse(row["toroleweight"].ToString());
                partAct.ToRole.Order = row["toroleorder"] == DBNull.Value ? byte.MinValue : byte.Parse(row["toroleorder"].ToString());
                partAct.ToRole.Code = row["torolecode"] == DBNull.Value ? string.Empty : row["torolecode"].ToString();

                partAct.CheckBalanceRequired = row["check_bal"] == DBNull.Value ? false : row["check_bal"].ToString() == "1" ? true : false;
                partAct.MaxValue = row["max_value"] == DBNull.Value ? 0 : int.Parse(row["max_value"].ToString());
                partAct.MinValue = row["min_value"] == DBNull.Value ? 0 : int.Parse(row["min_value"].ToString());
                partAct.BonusPercent = row["bonus_per"] == DBNull.Value ? 0 : double.Parse(row["bonus_per"].ToString());
                partAct.TaxPercent = row["taxper"] == DBNull.Value ? 0 : double.Parse(row["taxper"].ToString());
                partAct.BonusTaxPercent = row["bonus_tax"] == DBNull.Value ? 0 : double.Parse(row["bonus_tax"].ToString());
                partAct.MaxQueryDuration.Id = row["queryduration"] == DBNull.Value ? string.Empty : row["queryduration"].ToString();
                partAct.MaxQueryDuration.Name = row["queryduration_name"] == DBNull.Value ? string.Empty : row["queryduration_name"].ToString();
                partAct.Scope.Id = row["act_scope"] == DBNull.Value ? string.Empty : row["act_scope"].ToString();
                partAct.Scope.Name = row["act_scope_name"] == DBNull.Value ? string.Empty : row["act_scope_name"].ToString();
                partAct.MaxQueryRows = row["maxrec"] == DBNull.Value ? 0 : int.Parse(row["maxrec"].ToString());
                partAct.OnlyPartnerChildren = row["onlypartchildren"] == DBNull.Value ? false : row["onlypartchildren"].ToString() == "1" ? true : false;
                partAct.CreatedOn = row["createdon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["createdon"].ToString());
                partAct.LastEditOn = row["lastediton"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["lastediton"].ToString());
                partAct.CreatedBy.Id = row["createdby"] == DBNull.Value ? string.Empty : row["createdby"].ToString();
                partAct.CreatedBy.Name = row["createdname"] == DBNull.Value ? string.Empty : row["createdname"].ToString();

            }
            return partAct;
        }
    }
}
