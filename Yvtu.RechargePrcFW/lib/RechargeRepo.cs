using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yvtu.RechargePrcFW.lib
{
    public class RechargeResponseDto
    {
        public string ResultCode { get; set; }
        public string ResultDesc { get; set; }
        public int Duration { get; set; }
        public string TransNo { get; set; }
    }
    public class OpertionResult
    {
        public bool Success { get; set; }
        public int AffectedCount { get; set; }
        public string Error { get; set; }
    }
    public class RechargeRepo
    {

        public Queue<GrappedRecharge> GetPendingRechargeQueue(int queueNo, int count = 100)
        {
            var masterDataTable = DB.GetDataTable("Select * from v_pending_recharge_req t WHERE queue_no=" + queueNo + "  AND ROWNUM < " + count + " order by dcreatedon",null);

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;

            var results = new Queue<GrappedRecharge>();
            foreach (DataRow row in masterDataTable.Rows)
            {
                var obj = new GrappedRecharge();
                obj.Id = row["row_id"] == DBNull.Value ? -1 : int.Parse(row["row_id"].ToString());
                obj.SubscriberNo = row["subs_no"] == DBNull.Value ? string.Empty : row["subs_no"].ToString();
                obj.MasterId = row["cl_id"] == DBNull.Value ? int.MinValue : int.Parse(row["cl_id"].ToString());
                obj.Amount = row["amount"] == DBNull.Value ? 0 : double.Parse(row["amount"].ToString());
                obj.PointOfSaleId = row["pos_id"] == DBNull.Value ? string.Empty : row["pos_id"].ToString();
                obj.PointOfSaleAccount = row["pos_acc"] == DBNull.Value ? 0 : int.Parse(row["pos_acc"].ToString());
                obj.AccessChannelId = row["access_channel"] == DBNull.Value ? string.Empty : row["access_channel"].ToString();
                obj.Status = row["status"] == DBNull.Value ? 0 : int.Parse(row["status"].ToString());
                obj.StatusTime = row["status_time"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["status_time"].ToString());
                obj.QueueNo = row["queue_no"] == DBNull.Value ? 0 : int.Parse(row["queue_no"].ToString());
                obj.RefNo = row["ref_no"] == DBNull.Value ? string.Empty : row["ref_no"].ToString();
                obj.RefMessage = row["ref_message"] == DBNull.Value ? string.Empty : row["ref_message"].ToString();
                obj.RefTransNo = row["ref_trans_no"] == DBNull.Value ? string.Empty : row["ref_trans_no"].ToString();
                obj.RefTime = row["ref_time"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["ref_time"].ToString());
                obj.DebugInfo = row["debug_info"] == DBNull.Value ? string.Empty : row["debug_info"].ToString();
                results.Enqueue(obj);
            }
            return results;
        }
        public OpertionResult UpdateWithBalance(RechargeCollection rechargeCollection)
        {

            try
            {
                #region Parameters
                var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "retVal",OracleDbType = OracleDbType.Int32,  Direction = ParameterDirection.ReturnValue },
                 new OracleParameter{ ParameterName = "v_cl_id", OracleDbType = OracleDbType.Int32,  Value = rechargeCollection.Id},
                 new OracleParameter{ ParameterName = "v_status",OracleDbType = OracleDbType.Int32,  Value = rechargeCollection.Status },
                 new OracleParameter{ ParameterName = "v_ref_no",OracleDbType = OracleDbType.Varchar2,  Value = rechargeCollection.RefNo },
                 new OracleParameter{ ParameterName = "v_ref_message",OracleDbType = OracleDbType.Varchar2,  Value = rechargeCollection.RefMessage },
                 new OracleParameter{ ParameterName = "v_ref_time",OracleDbType = OracleDbType.Date,  Value = rechargeCollection.RefTime },
                 new OracleParameter{ ParameterName = "v_ref_trans_no",OracleDbType = OracleDbType.Varchar2,  Value = rechargeCollection.RefTransNo },
                 new OracleParameter{ ParameterName = "v_debug_info",OracleDbType = OracleDbType.Varchar2,  Value = rechargeCollection.DebugInfo },
                 new OracleParameter{ ParameterName = "v_update_bal",OracleDbType = OracleDbType.Int32,  Value = 1 }
                };
                #endregion
                DB.ExecuteStoredProc("pk_financial.fn_update_collection", parameters);
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

        public bool RemoveRechargeDraft(int id)
        {
            var parameters = new List<OracleParameter>
            {
                new OracleParameter {ParameterName = "id", OracleDbType = OracleDbType.Int32, Value = id},
            };
            var result =DB.ExecuteQuery("delete from collection_draft where ref_no=:id", parameters, 0);
            return result > 0;
        }

    }
}
