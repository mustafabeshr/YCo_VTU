using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
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
                 new OracleParameter{ ParameterName = "v_message",OracleDbType = OracleDbType.Varchar2,  Value = sms.Message },
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
    }
}
