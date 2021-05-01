using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Yvtu.Core.Entities;
using Yvtu.Core.Queries;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.Infra.Data
{
    public interface IApiDbLog
    {
        OpertionResult Create(ApiLogFile obj);

        ApiLogFileQuery Query(ApiLogFileQuery param);

    }
    public class ApiLogFileRepo : IApiDbLog
    {
        private readonly IAppDbContext _db;

        public ApiLogFileRepo(IAppDbContext db)
        {
            this._db = db;
        }

        public OpertionResult Create(ApiLogFile obj)
        {

            try
            {
                #region Parameters
                var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "retVal",OracleDbType = OracleDbType.Int32,  Direction = ParameterDirection.ReturnValue },
                 new OracleParameter{ ParameterName = "v_log_data",OracleDbType = OracleDbType.NVarchar2,  Value = obj.Data },
                 new OracleParameter{ ParameterName = "v_log_user",OracleDbType = OracleDbType.Varchar2,  Value = obj.User },
                 new OracleParameter{ ParameterName = "v_log_ip",OracleDbType = OracleDbType.Varchar2,  Value = obj.Ip },
                 new OracleParameter{ ParameterName = "v_log_level",OracleDbType = OracleDbType.Int32,  Value = obj.Level },
                 new OracleParameter{ ParameterName = "v_log_action",OracleDbType = OracleDbType.Varchar2,  Value = obj.Action }
                };
                #endregion
                _db.ExecuteStoredProc("pk_infra.fn_create_api_log", parameters);
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


        public ApiLogFileQuery Query(ApiLogFileQuery param)
        {
            string WhereClause = string.Empty;
            var parameters = BuildCriteria(param, ref WhereClause);

            var strSqlStatment = new StringBuilder();
            strSqlStatment.Append("Select * from ( ");
            strSqlStatment.Append("select rownum as seq , main_data.* from ( ");
            strSqlStatment.Append("Select * from API_LOG  " + WhereClause );
            strSqlStatment.Append(") main_data ) ");
            strSqlStatment.Append($"WHERE seq > ({param.Paging.PageNo - 1}) * {param.Paging.PageSize} AND ROWNUM <= {param.Paging.PageSize}");
            var masterDataTable = this._db.GetData(strSqlStatment.ToString(), parameters);

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;

            var apiLogFiles = new List<ApiLogFile>();
            foreach (DataRow row in masterDataTable.Rows)
            {
                var obj = ConvertDataRowToDataModel(row);
                apiLogFiles.Add(obj);
            }
            param.Results = apiLogFiles;

            return param;
        }

        private ApiLogFile ConvertDataRowToDataModel(DataRow row)
        {
            var apiLog = new ApiLogFile();
            apiLog.Id = row["log_id"] == DBNull.Value ? 0 : int.Parse(row["log_id"].ToString());
            apiLog.Level = row["log_level"] == DBNull.Value ? 0 : int.Parse(row["log_level"].ToString());
            apiLog.CreatedOn = row["createdon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["createdon"].ToString());
            apiLog.Data = row["log_data"] == DBNull.Value ? string.Empty : row["log_data"].ToString();
            apiLog.User = row["log_user"] == DBNull.Value ? string.Empty : row["log_user"].ToString();
            apiLog.Ip = row["log_ip"] == DBNull.Value ? string.Empty : row["log_ip"].ToString();
            apiLog.Action = row["log_action"] == DBNull.Value ? string.Empty : row["log_action"].ToString();
            return apiLog;
        }

        private List<OracleParameter> BuildCriteria(ApiLogFileQuery param, ref string criteria)
        {
            var WhereClause = new StringBuilder();
            var parameters = new List<OracleParameter>();
            if (!string.IsNullOrEmpty(param.Data))
            {
                var parm = new OracleParameter { ParameterName = ":logData", OracleDbType = OracleDbType.Varchar2, Value = param.Data };
                WhereClause.Append(" WHERE log_data LIKE  '%' ||  :logData || '%' ");
                parameters.Add(parm);
            }
            if (!string.IsNullOrEmpty(param.User))
            {
                var parm = new OracleParameter { ParameterName = ":logUser", OracleDbType = OracleDbType.Varchar2, Value = param.User };
                WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE log_user = :logUser " : " AND log_user = :logUser ");
                parameters.Add(parm);
            }
            if (!string.IsNullOrEmpty(param.Ip))
            {
                var parm = new OracleParameter { ParameterName = ":logIp", OracleDbType = OracleDbType.Varchar2, Value = param.Ip };
                WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE log_ip = :logIp " : " AND log_ip = :logIp ");
                parameters.Add(parm);
            }
            if (!string.IsNullOrEmpty(param.Action))
            {
                var parm = new OracleParameter { ParameterName = ":logAction", OracleDbType = OracleDbType.Varchar2, Value = param.Action };
                WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE log_action = :logAction " : " AND log_action = :logAction ");
                parameters.Add(parm);
            }
            if (param.Level != -1)
            {
                var parm = new OracleParameter { ParameterName = ":logLevel", OracleDbType = OracleDbType.Int32, Value = param.Level };
                WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE log_level = :logLevel " : " AND log_level = :logLevel ");
                parameters.Add(parm);
            }

            if (param.From > DateTime.MinValue && param.From != null)
            {
                WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE trunc(createdon)>=:StartDate " : " AND trunc(createdon)>=:StartDate   ");
                var parm = new OracleParameter { ParameterName = "StartDate", OracleDbType = OracleDbType.Date, Value = param.From };
                parameters.Add(parm);
            }
            if (param.To > DateTime.MinValue && param.To != null)
            {
                WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE trunc(createdon)<=:EndDate " : " AND trunc(createdon)<=:EndDate   ");
                var parm = new OracleParameter { ParameterName = "EndDate", OracleDbType = OracleDbType.Date, Value = param.To };
                parameters.Add(parm);
            }

            WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE ROWNUM <= 500 " : " AND ROWNUM <= 500   ");

            if (!string.IsNullOrEmpty(param.SortBy))
            {
                WhereClause.Append($" ORDER BY {param.SortBy} ");
                WhereClause.Append(!string.IsNullOrEmpty(param.SortType) ? " DESC " : param.SortType);
            }else
            {
                WhereClause.Append($" ORDER BY log_id desc ");
            }

            criteria = WhereClause.ToString();
            return parameters;
        }

    }
}
