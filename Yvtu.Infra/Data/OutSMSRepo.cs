using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.Infra.Data
{
    public class OutSMSRepo
    {
        private readonly IAppDbContext db;

        public OutSMSRepo(IAppDbContext db)
        {
            this.db = db;
        }
        public OpertionResult Create(SMSOut outSMS)
        {

            try
            {
                #region Parameters
                var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "retVal",OracleDbType = OracleDbType.Int32,  Direction = ParameterDirection.ReturnValue },
                 new OracleParameter{ ParameterName = "v_message",OracleDbType = OracleDbType.NVarchar2,  Value = outSMS.Message },
                 new OracleParameter{ ParameterName = "v_receiver",OracleDbType = OracleDbType.Varchar2,  Value = outSMS.Receiver },
                };
                #endregion
                db.ExecuteStoredProc("pk_infra.fn_createoutsms", parameters);
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

        public Queue<SMSOut> GetPendingSMSQueue(int count = 100)
        {
            var masterDataTable = db.GetData("Select * from SMS_OUT t WHERE  ROWNUM < " + count + " order by row_id", null);

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;

            var results = new Queue<SMSOut>();
            foreach (DataRow row in masterDataTable.Rows)
            {
                var obj = new SMSOut();
                obj.Id = row["row_id"] == DBNull.Value ? -1 : int.Parse(row["row_id"].ToString());
                obj.Receiver = row["receiver"] == DBNull.Value ? string.Empty : row["receiver"].ToString();
                obj.Sender = row["sender"] == DBNull.Value ? string.Empty : row["sender"].ToString();
                obj.Message = row["message"] == DBNull.Value ? string.Empty : row["message"].ToString();
                results.Enqueue(obj);
            }
            return results;
        }

        public bool RemoveMessage(int id)
        {
            var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "MsgId", OracleDbType = OracleDbType.Int32,  Value = id }
            };
            return db.ExecuteSqlCommand("Delete from SMS_OUT Where Row_Id=:MsgId", parameters) > 0;
        }
    }
}
