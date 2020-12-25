using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.Infra.Data
{
    public class SMSOneRepo
    {
        public class GetListParam
        {
            public string CreatorId { get; set; }
            public int CreatorAccount { get; set; }
            public string Receiver { get; set; }
            public string Message { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }
        private readonly IAppDbContext db;
        private readonly IPartnerManager partnerManager;

        public SMSOneRepo(IAppDbContext db, IPartnerManager partnerManager)
        {
            this.db = db;
            this.partnerManager = partnerManager;
        }
        public OpertionResult Create(SMSOne created)
        {
            try
            {
                #region Parameters
                var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "retVal",OracleDbType = OracleDbType.Int32,  Direction = ParameterDirection.ReturnValue },
                 new OracleParameter{ ParameterName = "v_createdby",OracleDbType = OracleDbType.Varchar2,  Value = created.CreatedBy.Id },
                 new OracleParameter{ ParameterName = "v_createdbyacc",OracleDbType = OracleDbType.Int32,  Value = created.CreatedBy.Account },
                 new OracleParameter{ ParameterName = "v_receiver",OracleDbType = OracleDbType.Varchar2,  Value = created.Receiver },
                 new OracleParameter{ ParameterName = "v_msg",OracleDbType = OracleDbType.Varchar2,  Value = created.Message },
                 new OracleParameter{ ParameterName = "v_note",OracleDbType = OracleDbType.Varchar2,  Value = created.Note }
                };
                #endregion
                db.ExecuteStoredProc("pk_infra.fn_create_smsone", parameters);
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


        public List<SMSOne> GetList(GetListParam param)
        {
            #region Parameters

            var parameters = new List<OracleParameter>();
            var whereCluase = new StringBuilder();
            if (param != null)
            {

                if (!string.IsNullOrEmpty(param.Receiver))
                {
                    whereCluase.Append(" WHERE receiver = :Receiver");
                    var p = new OracleParameter { ParameterName = "Receiver", OracleDbType = OracleDbType.Varchar2, Value = param.Receiver };
                    parameters.Add(p);
                }
                if (!string.IsNullOrEmpty(param.CreatorId))
                {
                    whereCluase.Append(whereCluase.Length > 0 ? " WHERE createdby = :CreatorId" : " AND createdby = :CreatorId");
                    var p = new OracleParameter { ParameterName = "CreatorId", OracleDbType = OracleDbType.Varchar2, Value = param.CreatorId };
                    parameters.Add(p);
                }
                
                if (param.CreatorAccount > 0)
                {
                    whereCluase.Append(whereCluase.Length > 0 ? " WHERE createdbyacc = :createdbyacc" : " AND createdbyacc = :CreatorAccount");
                    var p = new OracleParameter { ParameterName = "CreatorAccount", OracleDbType = OracleDbType.Int32, Value = param.CreatorAccount };
                    parameters.Add(p);
                }
                if (param.StartDate > DateTime.MinValue && param.StartDate != null)
                {
                    whereCluase.Append(whereCluase.Length > 0 ? " WHERE createdon >= :StartDate" : " AND createdon >= :StartDate");
                    var p = new OracleParameter { ParameterName = "StartDate", OracleDbType = OracleDbType.Date, Value = param.StartDate };
                    parameters.Add(p);
                }
                if (param.EndDate > DateTime.MinValue && param.EndDate != null)
                {
                    whereCluase.Append(whereCluase.Length > 0 ? " WHERE createdon <= :EndDate" : " AND createdon <= :EndDate");
                    var p = new OracleParameter { ParameterName = "EndDate", OracleDbType = OracleDbType.Date, Value = param.EndDate };
                    parameters.Add(p);
                }
                if (!string.IsNullOrEmpty(param.Message))
                {
                    whereCluase.Append(whereCluase.Length > 0 ? " WHERE (msg LIKE '%' ||  :Message || '%') " : " AND (msg LIKE '%' ||  :Message || '%') ");
                    var p = new OracleParameter { ParameterName = "CreatorId", OracleDbType = OracleDbType.Varchar2, Value = param.CreatorId };
                    parameters.Add(p);
                }
            }

            #endregion

            string strSql = $"select * from SEND_SMSONE_HIS {whereCluase} order by sms_id";

            DataTable masterDataTable;
            masterDataTable = db.GetData(strSql, parameters);

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;

            var results = new List<SMSOne>();
            foreach (DataRow row in masterDataTable.Rows)
            {
                var obj = new SMSOne();
                obj.Id = row["con_id"] == DBNull.Value ? -1 : int.Parse(row["con_id"].ToString());
                obj.Note = row["note"] == DBNull.Value ? string.Empty : row["note"].ToString();
                obj.Shortcode = row["shortcode"] == DBNull.Value ? string.Empty : row["shortcode"].ToString();
                obj.Receiver = row["receiver"] == DBNull.Value ? string.Empty : row["receiver"].ToString();
                obj.CreatedOn = row["createdon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["createdon"].ToString());
                var createdAccount = row["createdbyacc"] == DBNull.Value ? -1 : int.Parse(row["createdbyacc"].ToString());
                var createdBy = partnerManager.GetPartnerByAccount(createdAccount);
                obj.CreatedBy.Id = createdBy.Id;
                obj.CreatedBy.Name = createdBy.Name;
                obj.CreatedBy.Account = createdBy.Account;
                results.Add(obj);
            }
            return results;
        }
    }
}
