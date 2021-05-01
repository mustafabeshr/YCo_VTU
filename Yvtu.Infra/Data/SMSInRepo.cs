using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.Infra.Data
{
    public class SMSInRepo

    {
        private readonly IAppDbContext db;

        public SMSInRepo(IAppDbContext db)
        {
            this.db = db;
        }
        public OpertionResult Create(SMSIn sms)
        {

            try
            {
                #region Parameters
                var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "retVal",OracleDbType = OracleDbType.Int32,  Direction = ParameterDirection.ReturnValue },
                 new OracleParameter{ ParameterName = "v_reciever",OracleDbType = OracleDbType.Varchar2,  Value = sms.Receiver },
                 new OracleParameter{ ParameterName = "v_sender",OracleDbType = OracleDbType.Varchar2,  Value = sms.Sender },
                 new OracleParameter{ ParameterName = "v_message",OracleDbType = OracleDbType.NVarchar2,  Value = sms.Message },
                 new OracleParameter{ ParameterName = "v_ref_no",OracleDbType = OracleDbType.Int32,  Value = sms.RefNo },
                 new OracleParameter{ ParameterName = "v_lang",OracleDbType = OracleDbType.Varchar2,  Value = sms.Lang }
                };
                #endregion
                db.ExecuteStoredProc("pk_infra.fn_create_in_sms", parameters);
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

        public async Task<List<SMSIn>> GetSMSMessagesAsync(string mobileNo, string Msg, bool IncludeDates, DateTime? startDate, DateTime? endDate)
        {
            #region Parameters
            var parameters = new List<OracleParameter>();
            var whereCluase = new StringBuilder();

            if (!string.IsNullOrEmpty(mobileNo))
            {
                whereCluase.Append(" WHERE sender = :mobileNo");
                var p = new OracleParameter { ParameterName = "mobileNo", OracleDbType = OracleDbType.Varchar2, Value = mobileNo };
                parameters.Add(p);
            }
            
            if (!string.IsNullOrEmpty(Msg))
            {
                whereCluase.Append(whereCluase.Length > 0 ? " AND message LIKE '%' || :Msg || '%' " : " WHERE message LIKE '%' || :Msg || '%' ");
                var p = new OracleParameter { ParameterName = "Msg", OracleDbType = OracleDbType.Varchar2, Value = Msg };
                parameters.Add(p);
            }

            if (IncludeDates)
            {
                if (startDate > DateTime.MinValue && startDate != null)
                {
                    whereCluase.Append(whereCluase.Length > 0 ? " AND createdon >= :StartDate" : " WHERE createdon >= :StartDate");
                    var p = new OracleParameter { ParameterName = "StartDate", OracleDbType = OracleDbType.Date, Value = startDate };
                    parameters.Add(p);
                }
                if (endDate > DateTime.MinValue && endDate != null)
                {
                    whereCluase.Append(whereCluase.Length > 0 ? " AND createdon <= :EndDate" : " WHERE createdon <= :EndDate");
                    var p = new OracleParameter { ParameterName = "EndDate", OracleDbType = OracleDbType.Date, Value = endDate};
                    parameters.Add(p);
                }
            }
            #endregion

            string strSql = $"select * from sms_in {whereCluase} order by createdon";
            DataTable masterDataTable;
            masterDataTable = await db.GetDataAsync(strSql, parameters);

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;

            var results = new List<SMSIn>();
            foreach (DataRow row in masterDataTable.Rows)
            {
                var obj = new SMSIn();
                obj.Id = row["in_no"] == DBNull.Value ? 0 : int.Parse(row["in_no"].ToString());
                obj.CreatedOn = row["createdon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["createdon"].ToString());
                obj.Message = row["message"] == DBNull.Value ? string.Empty : row["message"].ToString();
                obj.Receiver = row["reciever"] == DBNull.Value ? string.Empty : row["reciever"].ToString();
                obj.Sender = row["sender"] == DBNull.Value ? string.Empty : row["sender"].ToString();
                obj.Lang = row["lang"] == DBNull.Value ? string.Empty : row["lang"].ToString();
                obj.RefNo = row["ref_no"] == DBNull.Value ? 0 : int.Parse(row["ref_no"].ToString());
                results.Add(obj);
            }
            return results;
        }
    }
}
