using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Yvtu.Core.Entities;

namespace Yvtu.Infra.Data.Interfaces
{
    public class DataAuditRepo : IDataAuditRepo
    {
        private readonly IAppDbContext db;

        public DataAuditRepo(IAppDbContext db)
        {
            this.db = db;
        }
        public async Task<OpertionResult> CreateAsync(DataAudit data)
        {
            var insertSql = "insert into data_audit " +
                "  (partner_id, createdon, act_id, action_id, note, old_value, new_value, system_note, error, success) values " +
                "   (:v_partner_id, sysdate, :v_act_id, :v_action_id, :v_note, :v_old_value, :v_new_value, :v_system_note, :v_error, :v_success)";
            try
            {
                #region Parameters
                    var parameters = new List<OracleParameter> {
                    new OracleParameter{ ParameterName = "v_partner_id", OracleDbType = OracleDbType.Varchar2,  Value = data.PartnerId },
                    new OracleParameter{ ParameterName = "v_act_id",OracleDbType = OracleDbType.Varchar2,  Value = data.Activity.Id },
                    new OracleParameter{ ParameterName = "v_action_id",OracleDbType = OracleDbType.Varchar2,  Value = data.Action.Id },
                    new OracleParameter{ ParameterName = "v_note",OracleDbType = OracleDbType.Varchar2,  Value = data.Note},
                    new OracleParameter{ ParameterName = "v_old_value",OracleDbType = OracleDbType.Varchar2,  Value = data.OldValue },
                    new OracleParameter{ ParameterName = "v_new_value",OracleDbType = OracleDbType.Varchar2,  Value = data.NewValue },
                    new OracleParameter{ ParameterName = "v_system_note",OracleDbType = OracleDbType.Varchar2,  Value = data.SystemNote },
                    new OracleParameter{ ParameterName = "v_error",OracleDbType = OracleDbType.Varchar2,  Value = data.Error },
                    new OracleParameter{ ParameterName = "v_success",OracleDbType = OracleDbType.Int32,  Value = data.Success ? 1 : 0 },
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

        public List<DataAudit> GetAuditig(string partnerId, string activityId, string actionId, DateTime startDate, DateTime endDate)
        {
            string sql = "Select * from v_data_audit ";
            string whereClause = string.Empty;

            #region Build Parameters
            var parameters = new List<OracleParameter>();
            if (!string.IsNullOrEmpty(partnerId))
            {
                var param1 = new OracleParameter() { ParameterName = "partnerId", OracleDbType = OracleDbType.Varchar2, Value = partnerId };
                parameters.Add(param1);
                whereClause = " WHERE partner_id=:partnerId ";
            }
            if (!string.IsNullOrEmpty(activityId))
            {
                var param1 = new OracleParameter() { ParameterName = "activityId", OracleDbType = OracleDbType.Varchar2, Value = activityId };
                parameters.Add(param1);
                whereClause += string.IsNullOrEmpty(whereClause) ? " WHERE act_id=:activityId " : " AND act_id=:activityId ";
            }
            if (!string.IsNullOrEmpty(actionId))
            {
                var param1 = new OracleParameter() { ParameterName = "actionId", OracleDbType = OracleDbType.Varchar2, Value = actionId };
                parameters.Add(param1);
                whereClause += string.IsNullOrEmpty(whereClause) ? " WHERE action_id=:actionId " : " AND action_id=:actionId ";
            }
            if (startDate != null && startDate != DateTime.MinValue)
            {
                var param1 = new OracleParameter() { ParameterName = "startDate", OracleDbType = OracleDbType.Date, Value = startDate };
                parameters.Add(param1);
                whereClause += string.IsNullOrEmpty(whereClause) ? " WHERE createdon>=:startDate " : " AND createdon>=:startDate ";
            }
            if (endDate != null && endDate != DateTime.MinValue)
            {
                var param1 = new OracleParameter() { ParameterName = "endDate", OracleDbType = OracleDbType.Date, Value = endDate };
                parameters.Add(param1);
                whereClause += string.IsNullOrEmpty(whereClause) ? " WHERE createdon<=:endDate " : " AND createdon<=:endDate ";
            }

            #endregion

            DataTable dt = null;
            if (parameters != null && parameters.Count > 0)
            {
                dt = this.db.GetData(sql + whereClause, parameters);
            }
            else
            {
                dt = this.db.GetData(sql + whereClause, null);
            }


            var dataList = new List<DataAudit>();
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    var currObj = new DataAudit();
                    currObj.Id = row["row_id"] == DBNull.Value ? 0 : int.Parse(row["row_id"].ToString());
                    currObj.PartnerId = row["partner_id"] == DBNull.Value ? string.Empty : row["partner_id"].ToString();
                    currObj.PartnerName = row["partner_name"] == DBNull.Value ? string.Empty : row["partner_name"].ToString();
                    currObj.CreatedOn = row["createdon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["createdon"].ToString());
                    currObj.Activity.Id = row["act_id"] == DBNull.Value ? string.Empty : row["act_id"].ToString();
                    currObj.Activity.Name = row["act_name"] == DBNull.Value ? string.Empty : row["act_name"].ToString();
                    currObj.Activity.Type = row["act_type"] == DBNull.Value ? string.Empty : row["act_type"].ToString();
                    currObj.Activity.Order = row["act_order"] == DBNull.Value ? int.MinValue :int.Parse(row["act_order"].ToString());
                    currObj.Activity.Internal = row["internal_use"] == DBNull.Value ? false : row["internal_use"].ToString() == "1" ? true : false;
                    currObj.Action.Id = row["action_id"] == DBNull.Value ? string.Empty : row["action_id"].ToString();
                    currObj.Action.Name = row["action_name"] == DBNull.Value ? string.Empty : row["action_name"].ToString();
                    currObj.Action.Type = row["code_type"] == DBNull.Value ? string.Empty : row["code_type"].ToString();
                    currObj.Action.Order = row["code_order"] == DBNull.Value ? int.MinValue : int.Parse(row["code_order"].ToString());
                    currObj.Note = row["note"] == DBNull.Value ? string.Empty : row["note"].ToString();
                    currObj.OldValue = row["old_value"] == DBNull.Value ? string.Empty : row["old_value"].ToString();
                    currObj.NewValue = row["new_value"] == DBNull.Value ? string.Empty : row["new_value"].ToString();
                    currObj.SystemNote = row["system_note"] == DBNull.Value ? string.Empty : row["system_note"].ToString();
                    currObj.Error = row["error"] == DBNull.Value ? string.Empty : row["error"].ToString();
                    currObj.Success = row["success"] == DBNull.Value ? false : row["success"].ToString() == "1" ? true : false;
                    dataList.Add(currObj);
                }
            }
            return  dataList;
        }
    }
}