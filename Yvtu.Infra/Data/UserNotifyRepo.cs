using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.Infra.Data
{
    public class UserNotifyRepo
    {
        private readonly IAppDbContext db;

        public UserNotifyRepo(IAppDbContext db)
        {
            this.db = db;
        }

        public OpertionResult Create(UserNotify obj, bool thenPostIt = false)
        {
            try
            {
                #region Parameters
                var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "retVal",OracleDbType = OracleDbType.Int32,  Direction = ParameterDirection.ReturnValue },
                 new OracleParameter{ ParameterName = "v_subject",OracleDbType = OracleDbType.NVarchar2,  Value = obj.Subject },
                 new OracleParameter{ ParameterName = "v_content",OracleDbType = OracleDbType.NVarchar2,  Value = obj.Content },
                 new OracleParameter{ ParameterName = "v_priority",OracleDbType = OracleDbType.Varchar2,  Value = obj.Priority.Id },
                 new OracleParameter{ ParameterName = "v_createdbyid",OracleDbType = OracleDbType.Varchar2,  Value = obj.CreatedBy.Id },
                 new OracleParameter{ ParameterName = "v_expire_time",OracleDbType = OracleDbType.Date,  Value =  obj.ExpireOn },
                 new OracleParameter{ ParameterName = "v_post_it",OracleDbType = OracleDbType.Int32,  Value = thenPostIt ? 1 : 0 }
                };
                #endregion
                db.ExecuteStoredProc("pk_infra.fn_create_usersinstruct", parameters);
                var result = int.Parse(parameters.Find(x => x.ParameterName == "retVal").Value.ToString());

                if (result > 0)
                {
                    foreach (var item in obj.NotifyToList)
                    {
                        item.UserNotifyId = result;
                        CreateUserNotifyTo(item);
                    }
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

        public int CreateUserNotifyTo(UserNotifyTo obj)
        {
            try
            {
                #region Parameters
                var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "retVal",OracleDbType = OracleDbType.Int32,  Direction = ParameterDirection.ReturnValue },
                 new OracleParameter{ ParameterName = "v_ins_id",OracleDbType = OracleDbType.Int32,  Value = obj.UserNotifyId },
                 new OracleParameter{ ParameterName = "v_role_id",OracleDbType = OracleDbType.Int32,  Value = obj.Role.Id }
                };
                #endregion
                db.ExecuteStoredProc("pk_infra.fn_create_userinstructto", parameters);
                var result = int.Parse(parameters.Find(x => x.ParameterName == "retVal").Value.ToString());

                return result;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public List<UserNotify> QueryWithPaging(int id, string content, string status, DateTime startDate, DateTime endDate, Paging paging)
        {
            var WhereClause = string.Empty;
            var parameters = BuildParameters(id, content, status, startDate, endDate, ref WhereClause);

            var strSqlStatment = new StringBuilder();
            strSqlStatment.Append("Select * from ( ");
            strSqlStatment.Append("select rownum as seq , main_data.* from ( ");
            strSqlStatment.Append("Select * from v_users_instruct t  " + WhereClause + " order by createdon desc ");
            strSqlStatment.Append(") main_data ) ");
            strSqlStatment.Append($"WHERE seq > ({paging.PageNo - 1}) * {paging.PageSize} AND ROWNUM <= {paging.PageSize}");

            var masterDataTable = this.db.GetData(strSqlStatment.ToString(), parameters);

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;

            var results = new List<UserNotify>();
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
            if (!string.IsNullOrEmpty(status) && status != "-1")
            {
                var parm = new OracleParameter { ParameterName = "StatusId", OracleDbType = OracleDbType.Varchar2, Value = status };
                WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE status=:StatusId " : " AND status=:StatusId ");
                parameters.Add(parm);
            }
            if (!string.IsNullOrEmpty(content))
            {
                var parm = new OracleParameter { ParameterName = "Cntnt", OracleDbType = OracleDbType.Varchar2, Value = content };
                WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE subject LIKE  '%' ||  :Cntnt || '%'  " : " AND subject LIKE  '%' ||  :Cntnt || '%' ");
                parameters.Add(parm);
            }


            if (startDate > DateTime.MinValue && startDate != null)
            {
                WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE trunc(createdon)>=:StartDate " : " AND trunc(createdon)>=:StartDate   ");
                var parm = new OracleParameter { ParameterName = "StartDate", OracleDbType = OracleDbType.Date, Value = startDate };
                parameters.Add(parm);
            }
            if (endDate > DateTime.MinValue && endDate != null)
            {
                WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE trunc(createdon)<=:EndDate " : " AND trunc(createdon)<=:EndDate   ");
                var parm = new OracleParameter { ParameterName = "EndDate", OracleDbType = OracleDbType.Date, Value = endDate };
                parameters.Add(parm);
            }

            WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE ROWNUM <= 500 " : " AND ROWNUM <= 500 ");

            criteria = WhereClause.ToString();
            return parameters;
        }
        private UserNotify ConvertDataRowToUserNotify(DataRow row)
        {
            var obj = new UserNotify();
            obj.Id = row["ins_id"] == DBNull.Value ? -1 : int.Parse(row["ins_id"].ToString());
            obj.CreatedOn = row["createdon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["createdon"].ToString());
            obj.Content = row["content"] == DBNull.Value ? string.Empty : row["content"].ToString();
            obj.Subject = row["subject"] == DBNull.Value ? string.Empty : row["subject"].ToString();
            obj.Priority.Id = row["priority"] == DBNull.Value ? string.Empty : row["priority"].ToString();
            obj.Priority.Name = row["priority_name"] == DBNull.Value ? string.Empty : row["priority_name"].ToString();
            obj.Status.Id = row["status"] == DBNull.Value ? string.Empty : row["status"].ToString();
            obj.Status.Name = row["status_name"] == DBNull.Value ? string.Empty : row["status_name"].ToString();
            obj.StatusOn = row["status_time"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["status_time"].ToString());
            obj.CreatedBy.Id = row["createdbyid"] == DBNull.Value ? string.Empty : row["createdbyid"].ToString();
            obj.CreatedBy.Account = row["createdbyacc"] == DBNull.Value ? 0 : int.Parse(row["createdbyacc"].ToString());
            obj.CreatedBy.Name = row["createdby_name"] == DBNull.Value ? string.Empty : row["createdby_name"].ToString();
            obj.ExpireOn = row["expire_time"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["expire_time"].ToString());
            obj.NotifyToList = GetUserNotifyToList(obj.Id);
            if(obj.Status.Id == "post")
            {
                obj.UserNotifyHistoryCount = new UserNotifyHistoryRepo(db).GetUserNotifyHisCount(obj.Id);
            }
            else
            {
                obj.UserNotifyHistoryCount = 0;
            }
            return obj;
        }
        public int GetCount(int id, string content, string status, DateTime startDate, DateTime endDate)
        {
            string WhereClause = string.Empty;
            var parameters = BuildParameters(id, content, status, startDate, endDate, ref WhereClause);
            var strSqlStatment = new StringBuilder();
            strSqlStatment.Append($"Select count(*) val from v_users_instruct  { WhereClause }");
            var count = this.db.GetIntScalarValue(strSqlStatment.ToString(), parameters);
            return count;
        }
        private List<UserNotifyTo> GetUserNotifyToList(int id)
        {
            if (id <= 0) return null;
            var WhereClause = new StringBuilder();
            var parameters = new List<OracleParameter>();
            var parm = new OracleParameter { ParameterName = "InsId", OracleDbType = OracleDbType.Int32, Value = id };
            parameters.Add(parm);
            var strSqlStatment = new StringBuilder();
            strSqlStatment.Append($"select t.to_id, t.ins_id, t.role_id,r.rolename from users_instruct_to t, approle r where t.role_id = r.roleid and t.ins_id = :InsId ");
            var masterDataTable = this.db.GetData(strSqlStatment.ToString(), parameters);
            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;

            var results = new List<UserNotifyTo>();
            foreach (DataRow row in masterDataTable.Rows)
            {
                var obj = new UserNotifyTo();
                obj.Id = row["to_id"] == DBNull.Value ? -1 : int.Parse(row["to_id"].ToString());
                obj.UserNotifyId = row["ins_id"] == DBNull.Value ? -1 : int.Parse(row["ins_id"].ToString());
                obj.Role.Id = row["role_id"] == DBNull.Value ? -1 : int.Parse(row["role_id"].ToString());
                obj.Role.Name = row["rolename"] == DBNull.Value ? string.Empty : row["rolename"].ToString();
                results.Add(obj);
            }
            return results;
        }

        public OpertionResult Post(int id)
        {
            try
            {
                #region Parameters
                var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "retVal",OracleDbType = OracleDbType.Int32,  Direction = ParameterDirection.ReturnValue },
                 new OracleParameter{ ParameterName = "v_ins_id",OracleDbType = OracleDbType.Int32,  Value = id }
                };
                #endregion
                db.ExecuteStoredProc("pk_infra.fn_userinstruct_post", parameters);
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

        public int Delete(int id)
        {
            try
            {
                #region Parameters
                var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "retVal",OracleDbType = OracleDbType.Int32,  Direction = ParameterDirection.ReturnValue },
                 new OracleParameter{ ParameterName = "v_ins_id",OracleDbType = OracleDbType.Int32,  Value = id }
                };
                #endregion
                db.ExecuteStoredProc("pk_infra.fn_userinstruct_delete", parameters);
                var result = int.Parse(parameters.Find(x => x.ParameterName == "retVal").Value.ToString());

                return result;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
        public int DeleteNotifyTo(int id)
        {
            try
            {
                #region Parameters
                var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "retVal",OracleDbType = OracleDbType.Int32,  Direction = ParameterDirection.ReturnValue },
                 new OracleParameter{ ParameterName = "v_to_id",OracleDbType = OracleDbType.Int32,  Value = id }
                };
                #endregion
                db.ExecuteStoredProc("pk_infra.fn_userinstructto_delete", parameters);
                var result = int.Parse(parameters.Find(x => x.ParameterName == "retVal").Value.ToString());

                return result;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        
    }
}
