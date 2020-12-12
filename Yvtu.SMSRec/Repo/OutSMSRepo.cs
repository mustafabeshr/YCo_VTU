using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.SMSRec.Repo
{
    public class OutSMSRepo
    {
        private readonly IRecDbContext db;

        public OutSMSRepo(IRecDbContext db)
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
                 new OracleParameter{ ParameterName = "v_message",OracleDbType = OracleDbType.Varchar2,  Value = outSMS.Message },
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
    }
}
