using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Yvtu.Core.Entities;
using Yvtu.Core.Queries;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.SMSRec.Repo
{
    public class RechargeCollectionRepo
    {
        private readonly IRecDbContext db;

        public RechargeCollectionRepo(IRecDbContext db)
        {
            this.db = db;
        }

        public OpertionResult Create(RechargeCollection rechargeCollection)
        {

            try
            {
                #region Parameters
                var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "retVal",OracleDbType = OracleDbType.Int32,  Direction = ParameterDirection.ReturnValue },
                 new OracleParameter{ ParameterName = "v_subs_no", OracleDbType = OracleDbType.Varchar2,  Value = rechargeCollection.SubscriberNo},
                 new OracleParameter{ ParameterName = "v_amount",OracleDbType = OracleDbType.Decimal,  Value = rechargeCollection.Amount },
                 new OracleParameter{ ParameterName = "v_pos_id",OracleDbType = OracleDbType.Varchar2,  Value = rechargeCollection.PointOfSale.Id },
                 new OracleParameter{ ParameterName = "v_access_channel",OracleDbType = OracleDbType.Varchar2,  Value = rechargeCollection.AccessChannel.Id },
                 new OracleParameter{ ParameterName = "v_queue_no",OracleDbType = OracleDbType.Int32,  Value = rechargeCollection.QueueNo },
                 new OracleParameter{ ParameterName = "v_ref_no",OracleDbType = OracleDbType.Int32,  Value = rechargeCollection.RefNo },
                 new OracleParameter{ ParameterName = "v_ref_message",OracleDbType = OracleDbType.Varchar2,  Value = rechargeCollection.RefMessage },
                 new OracleParameter{ ParameterName = "v_ref_time",OracleDbType = OracleDbType.Date,  Value = rechargeCollection.RefTime },
                 new OracleParameter{ ParameterName = "v_ref_trans_no",OracleDbType = OracleDbType.Varchar2,  Value = rechargeCollection.RefTransNo },
                 new OracleParameter{ ParameterName = "v_debug_info",OracleDbType = OracleDbType.Varchar2,  Value = rechargeCollection.DebugInfo },
                 new OracleParameter{ ParameterName = "v_status",OracleDbType = OracleDbType.Int32,  Value = rechargeCollection.Status.Id }
                };
                #endregion
                db.ExecuteStoredProc("pk_financial.fn_create_collection", parameters);
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
