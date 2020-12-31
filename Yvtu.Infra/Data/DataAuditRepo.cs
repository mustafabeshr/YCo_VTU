using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Yvtu.Core.Entities;
using static Yvtu.Infra.Data.Interfaces.IDataAuditRepo;

namespace Yvtu.Infra.Data.Interfaces
{
    public class DataAuditRepo : IDataAuditRepo
    {
        private readonly IAppDbContext db;

        public DataAuditRepo(IAppDbContext db)
        {
            this.db = db;
        }
        public OpertionResult Create(DataAudit data)
        {
            try
            {
                #region Parameters
                    var parameters = new List<OracleParameter> {
                    new OracleParameter{ ParameterName = "v_partner_id", OracleDbType = OracleDbType.Varchar2,  Value = data.PartnerId },
                    new OracleParameter{ ParameterName = "v_partner_acc", OracleDbType = OracleDbType.Int32,  Value = data.PartnerAccount },
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
                var result =  db.ExecuteStoredProc("pk_utility.sp_create_audit", parameters);
                return new OpertionResult { AffectedCount = result, Success = true, Error = string.Empty };
                //if (result > 0)
                //{
                //    return new OpertionResult { AffectedCount = result, Success = true, Error = string.Empty };
                //}
                //else
                //{
                //    return new OpertionResult { AffectedCount = result, Success = false, Error = string.Empty };
                //}
            }
            catch (Exception ex)
            {
                return new OpertionResult { AffectedCount = -1, Success = false, Error = ex.Message };
            }
        }

        public List<DataAudit> GetAuditig(GetListParam param)
        {
            string sql = "Select * from v_data_audit ";
            string whereClause = string.Empty;

            #region Build Parameters
            var parameters = new List<OracleParameter>();
            if (!string.IsNullOrEmpty(param.CreatorId))
            {
                var param1 = new OracleParameter() { ParameterName = "CreatorId", OracleDbType = OracleDbType.Varchar2, Value = param.CreatorId };
                parameters.Add(param1);
                whereClause = " WHERE partner_id=:CreatorId ";
            }

            if (param.CreatorAccount > 0)
            {
                var param1 = new OracleParameter() { ParameterName = "CreatorAccount", OracleDbType = OracleDbType.Int32, Value = param.CreatorAccount };
                parameters.Add(param1);
                whereClause += string.IsNullOrEmpty(whereClause) ? " WHERE partner_acc=:CreatorAccount " : " AND partner_acc=:CreatorAccount ";
            }

            if (!string.IsNullOrEmpty(param.ActivityId))
            {
                var param1 = new OracleParameter() { ParameterName = "ActivityId", OracleDbType = OracleDbType.Varchar2, Value = param.ActivityId };
                parameters.Add(param1);
                whereClause += string.IsNullOrEmpty(whereClause) ? " WHERE act_id=:ActivityId " : " AND act_id=:ActivityId ";
            }
            //if (!string.IsNullOrEmpty(actionId))
            //{
            //    var param1 = new OracleParameter() { ParameterName = "actionId", OracleDbType = OracleDbType.Varchar2, Value = actionId };
            //    parameters.Add(param1);
            //    whereClause += string.IsNullOrEmpty(whereClause) ? " WHERE action_id=:actionId " : " AND action_id=:actionId ";
            //}
            if (param.IncludeDates)
            {
                if (param.StartDate != null && param.StartDate != DateTime.MinValue)
                {
                    var param1 = new OracleParameter() { ParameterName = "StartDate", OracleDbType = OracleDbType.Date, Value = param.StartDate };
                    parameters.Add(param1);
                    whereClause += string.IsNullOrEmpty(whereClause) ? " WHERE createdon>=:StartDate " : " AND createdon>=:StartDate ";
                }
                if (param.EndDate != null && param.EndDate != DateTime.MinValue)
                {
                    var param1 = new OracleParameter() { ParameterName = "EndDate", OracleDbType = OracleDbType.Date, Value = param.EndDate };
                    parameters.Add(param1);
                    whereClause += string.IsNullOrEmpty(whereClause) ? " WHERE createdon<=:EndDate " : " AND createdon<=:EndDate ";
                }
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
                    currObj.PartnerAccount = row["partner_acc"] == DBNull.Value ? -1 : int.Parse(row["partner_acc"].ToString());
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
                    if (!string.IsNullOrEmpty(currObj.OldValue))
                    {
                        var old = currObj.OldValue.Split('\n');
                        currObj.OldValueList = old.OfType<string>().ToList();
                    }
                    if (!string.IsNullOrEmpty(currObj.NewValue))
                    {
                        var n = currObj.NewValue.Split('\n');
                        currObj.NewValueList = n.OfType<string>().ToList();
                    }
                    dataList.Add(currObj);
                }
            }
            return  dataList;
        }
    }
}