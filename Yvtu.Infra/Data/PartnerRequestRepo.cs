using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.Infra.Data
{
    public class PartnerRequestRepo

    {
        private readonly IAppDbContext db;

        public PartnerRequestRepo(IAppDbContext db)
        {
            this.db = db;
        }
        public OpertionResult Create(PartnerRequest req)
        {

            try
            {
                #region Parameters
                var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "retVal",OracleDbType = OracleDbType.Int32,  Direction = ParameterDirection.ReturnValue },
                 new OracleParameter{ ParameterName = "v_req_id",OracleDbType = OracleDbType.Int32,  Value = req.RequestId },
                 new OracleParameter{ ParameterName = "v_req_content",OracleDbType = OracleDbType.Varchar2,  Value = req.Content },
                 new OracleParameter{ ParameterName = "v_replay_time",OracleDbType = OracleDbType.Date,  Value = req.ReplayTime },
                 new OracleParameter{ ParameterName = "v_replay_desc",OracleDbType = OracleDbType.Varchar2,  Value = req.ReplayDesc },
                 new OracleParameter{ ParameterName = "v_replay_shortcode",OracleDbType = OracleDbType.Varchar2,  Value = req.Shortcode },
                 new OracleParameter{ ParameterName = "v_status",OracleDbType = OracleDbType.Int32,  Value = req.Status },
                 new OracleParameter{ ParameterName = "v_queue_no",OracleDbType = OracleDbType.Int32,  Value = req.QueueNo },
                 new OracleParameter{ ParameterName = "v_error",OracleDbType = OracleDbType.Varchar2,  Value = req.Error },
                 new OracleParameter{ ParameterName = "v_accesschannel",OracleDbType = OracleDbType.Varchar2,  Value = req.AccessChannel },
                };
                #endregion
                db.ExecuteStoredProc("pk_infra.fn_create_partner_request", parameters);
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
