using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace Yvtu.RechargePrcFW.lib
{
    public class SMSOutRepo
    {
        public OpertionResult Create(Entities.SMSOut sms)
        {
            try
            {
                #region Parameters
                var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "retVal",OracleDbType = OracleDbType.Int32,  Direction = ParameterDirection.ReturnValue },
                 new OracleParameter{ ParameterName = "v_message", OracleDbType = OracleDbType.NVarchar2,  Value = sms.Message},
                 new OracleParameter{ ParameterName = "v_receiver",OracleDbType = OracleDbType.Varchar2,  Value = sms.Receiver }
                };
                #endregion
                DB.ExecuteStoredProc("pk_infra.fn_createOutSMS", parameters);
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
