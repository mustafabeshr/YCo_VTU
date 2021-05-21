using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.Infra.Data
{
    public class ApiIPBlacklistRepo
    {
        private readonly IAppDbContext _db;
        public ApiIPBlacklistRepo(IAppDbContext db)
        {
            this._db = db;
        }

        public OpertionResult Create(ApiIPBlacklist obj)
        {

            try
            {
                #region Parameters
                var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "retVal",OracleDbType = OracleDbType.Int32,  Direction = ParameterDirection.ReturnValue },
                 new OracleParameter{ ParameterName = "v_ip_address",OracleDbType = OracleDbType.Varchar2,  Value = obj.IPAddress }
                };
                #endregion
                _db.ExecuteStoredProc("pk_infra.fn_create_api_ip_blacklist", parameters);
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

        public ApiIPBlacklist GetSingleOrDefault(string ip)
        {
            #region Parameters
            var parameters = new List<OracleParameter>();
            var whereCluase = new StringBuilder();

            if (string.IsNullOrEmpty(ip))
            {
                return null;
            }
                whereCluase.Append(" WHERE ip_address = :ipAddress");
                var p = new OracleParameter { ParameterName = "ipAddress", OracleDbType = OracleDbType.Varchar2, Value = ip };
                parameters.Add(p);

            
            #endregion

            string strSql = $"select * from api_ip_blacklist {whereCluase} order by createdon desc";
            DataTable masterDataTable;
            masterDataTable = _db.GetData(strSql, parameters);

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;

            DataRow row = masterDataTable.Rows[0];
            var obj = new ApiIPBlacklist();
            obj.IPAddress = row["ip_address"] == DBNull.Value ? string.Empty : row["ip_address"].ToString();
            obj.CreatedOn = row["createdon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["createdon"].ToString());

            return obj;
        }

        public List<ApiIPBlacklist> GetList(string ip)
        {
            #region Parameters
            var parameters = new List<OracleParameter>();
            var whereCluase = new StringBuilder();

            if (!string.IsNullOrEmpty(ip))
            {
                whereCluase.Append(" WHERE sender = :ipAddress");
                var p = new OracleParameter { ParameterName = "ipAddress", OracleDbType = OracleDbType.Varchar2, Value = ip };
                parameters.Add(p);
            }


            #endregion

            string strSql = $"select * from api_ip_blacklist {whereCluase} order by createdon desc";
            DataTable masterDataTable;
            masterDataTable = _db.GetData(strSql, parameters);

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;

            var results = new List<ApiIPBlacklist>();
            foreach (DataRow row in masterDataTable.Rows)
            {
                var obj = new ApiIPBlacklist();
                obj.IPAddress = row["ip_address"] == DBNull.Value ? string.Empty : row["ip_address"].ToString();
                obj.CreatedOn = row["createdon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["createdon"].ToString());
                results.Add(obj);
            }
            return results;
        }

        public bool isBlacklisted(string ip)
        {
            string WhereClause = string.Empty;
            var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "v_ip_address",OracleDbType = OracleDbType.Varchar2,  Value = ip }
                };
            var strSqlStatment = new StringBuilder();
            strSqlStatment.Append($"Select count(*) val from api_ip_blacklist  where ip_address = :v_ip_address");
            var count = this._db.GetIntScalarValue(strSqlStatment.ToString(), parameters);
            return count > 0;
        }

        public OpertionResult Remove(string ipAddres)
        {
            try
            {
                #region Parameters
                var parameters = new List<OracleParameter> {
                    new OracleParameter{ ParameterName = "retVal",OracleDbType = OracleDbType.Int32,  Direction = ParameterDirection.ReturnValue },
                    new OracleParameter{ ParameterName = "v_ip_address", OracleDbType = OracleDbType.Varchar2,  Value = ipAddres }
                };  
                #endregion
                _db.ExecuteStoredProc("pk_infra.fn_ApiBlacklist_remove", parameters);
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
    }
}
