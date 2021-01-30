using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.Infra.Data
{
    public class UserNotifyHistoryRepo
    {
        private readonly IAppDbContext db;

        public UserNotifyHistoryRepo(IAppDbContext db)
        {
            this.db = db;
        }


        public OpertionResult Read(int id)
        {
            try
            {
                #region Parameters
                var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "retVal",OracleDbType = OracleDbType.Int32,  Direction = ParameterDirection.ReturnValue },
                 new OracleParameter{ ParameterName = "v_his_id",OracleDbType = OracleDbType.Int32,  Value = id }
                };
                #endregion
                db.ExecuteStoredProc("pk_infra.fn_read_userinstructhis", parameters);
                var result = int.Parse(parameters.Find(x => x.ParameterName == "retVal").Value.ToString());

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

        public List<UserNotifyHistory> QueryWithPaging(int id, string content, string status, DateTime startDate, DateTime endDate, Paging paging)
        {
            var WhereClause = string.Empty;
            var parameters = BuildParameters(id, content, status, startDate, endDate, ref WhereClause);

            var strSqlStatment = new StringBuilder();
            strSqlStatment.Append("Select * from ( ");
            strSqlStatment.Append("select rownum as seq , main_data.* from ( ");
            strSqlStatment.Append("Select * from v_users_instruct_his t  " + WhereClause + " order by createdon desc ");
            strSqlStatment.Append(") main_data ) ");
            strSqlStatment.Append($"WHERE seq > ({paging.PageNo - 1}) * {paging.PageSize} AND ROWNUM <= {paging.PageSize}");

            var masterDataTable = this.db.GetData(strSqlStatment.ToString(), parameters);

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;

            var results = new List<UserNotifyHistory>();
            foreach (DataRow row in masterDataTable.Rows)
            {
                var obj = ConvertDataRowToUserNotify(row);
                results.Add(obj);
            }
            return results;
        }
        private List<OracleParameter> BuildParameters(int id, string content, string status, DateTime startDate, DateTime endDate, ref string criteria)
        {
            var WhereClause = new StringBuilder();
            var parameters = new List<OracleParameter>();
            if (id > 0)
            {
                var parm = new OracleParameter { ParameterName = "NId", OracleDbType = OracleDbType.Int32, Value = id };
                WhereClause.Append(" WHERE ins_id=:NId ");
                parameters.Add(parm);
            }
            if (!string.IsNullOrEmpty(status))
            {
                var parm = new OracleParameter { ParameterName = "StatusId", OracleDbType = OracleDbType.Varchar2, Value = status };
                WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE status=:StatusId " : " AND status=:StatusId ");
                parameters.Add(parm);
            }
            if (!string.IsNullOrEmpty(content))
            {
                var parm = new OracleParameter { ParameterName = "Cntnt", OracleDbType = OracleDbType.Varchar2, Value = content };
                WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE content LIKE  '%' ||  :Cntnt || '%'  " : " AND content LIKE  '%' ||  :Cntnt || '%' ");
                parameters.Add(parm);
            }

            if (startDate > DateTime.MinValue && startDate != null)
            {
                WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE createdon>=:StartDate " : " AND createdon>=:StartDate   ");
                var parm = new OracleParameter { ParameterName = "StartDate", OracleDbType = OracleDbType.Date, Value = startDate };
                parameters.Add(parm);
            }
            if (endDate > DateTime.MinValue && endDate != null)
            {
                WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE createdon<=:EndDate " : " AND createdon<=:EndDate   ");
                var parm = new OracleParameter { ParameterName = "EndDate", OracleDbType = OracleDbType.Date, Value = endDate };
                parameters.Add(parm);
            }

            WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE ROWNUM <= 500 " : " AND ROWNUM <= 500 ");

            criteria = WhereClause.ToString();
            return parameters;
        }
        private UserNotifyHistory ConvertDataRowToUserNotify(DataRow row)
        {
            var obj = new UserNotifyHistory();
            obj.Id = row["his_id"] == DBNull.Value ? -1 : int.Parse(row["his_id"].ToString());
            obj.UserNotifyId = row["ins_id"] == DBNull.Value ? -1 : int.Parse(row["ins_id"].ToString());
            obj.CreatedOn = row["createdon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["createdon"].ToString());
            obj.Content = row["content"] == DBNull.Value ? string.Empty : row["content"].ToString();
            obj.Partner.Id = row["partner_id"] == DBNull.Value ? string.Empty : row["partner_id"].ToString();
            obj.Partner.Account = row["partner_acc"] == DBNull.Value ? 0 :int.Parse(row["partner_acc"].ToString());
            obj.Partner.Name = row["partner_name"] == DBNull.Value ? string.Empty : row["partner_name"].ToString();
            obj.Priority.Id = row["priority"] == DBNull.Value ? string.Empty : row["priority"].ToString();
            obj.Priority.Name = row["priority_name"] == DBNull.Value ? string.Empty : row["priority_name"].ToString();
            obj.Status.Id = row["status"] == DBNull.Value ? string.Empty : row["status"].ToString();
            obj.Status.Name = row["status_name"] == DBNull.Value ? string.Empty : row["status_name"].ToString();
            obj.StatusOn = row["status_time"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["status_time"].ToString());
            obj.CreatedBy.Id = row["createdbyid"] == DBNull.Value ? string.Empty : row["createdbyid"].ToString();
            obj.CreatedBy.Account = row["createdbyacc"] == DBNull.Value ? 0 : int.Parse(row["createdbyacc"].ToString());
            obj.CreatedBy.Name = row["createdby_name"] == DBNull.Value ? string.Empty : row["createdby_name"].ToString();
            obj.ExpireOn = row["expire_time"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["expire_time"].ToString());
            obj.HistoryOn = row["historyon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["historyon"].ToString());
            return obj;
        }
    }
}
