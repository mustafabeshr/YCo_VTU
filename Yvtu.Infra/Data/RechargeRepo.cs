using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Yvtu.Core.Entities;
using Yvtu.Core.Queries;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.Infra.Data
{
    public class RechargeRepo
    {
        private readonly IAppDbContext db;
        private readonly IPartnerManager partnerManager;

        public RechargeRepo(IAppDbContext db, IPartnerManager partnerManager)
        {
            this.db = db;
            this.partnerManager = partnerManager;
        }

        public RechargeQuery Query(RechargeQuery param)
        {
           
            var WhereClause = new StringBuilder();
            var parameters = new List<OracleParameter>();
            if (param.QPosAccount > 0)
            {
                var parm = new OracleParameter { ParameterName = "QPosAccount", OracleDbType = OracleDbType.Int32, Value = param.QPosAccount };
                WhereClause.Append(" WHERE pos_acc=:QPosAccount ");
                parameters.Add(parm);
            }
            if (!string.IsNullOrEmpty(param.QPosId))
            {
                var parm = new OracleParameter { ParameterName = "QPosId", OracleDbType = OracleDbType.Varchar2, Value = param.QPosId };
                WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE pos_id=:QPosId " : " AND pos_id=:QPosId ");
                parameters.Add(parm);
            }
            if (!string.IsNullOrEmpty(param.QSubsId))
            {
                var parm = new OracleParameter { ParameterName = "QSubsId", OracleDbType = OracleDbType.Varchar2, Value = param.QSubsId };
                WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE subs_no=:QSubsId " : " AND subs_no=:QSubsId ");
                parameters.Add(parm);
            }
            if (param.StatusId > 0)
            {
                var parm = new OracleParameter { ParameterName = "StatusId", OracleDbType = OracleDbType.Int32, Value = param.StatusId };
                WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE status=:StatusId " : " AND status=:StatusId ");
                parameters.Add(parm);
            }
            if (!string.IsNullOrEmpty(param.AccessChannelId))
            {
                var parm = new OracleParameter { ParameterName = "AccessChannelId", OracleDbType = OracleDbType.Varchar2, Value = param.AccessChannelId };
                WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE access_channel=:AccessChannelId " : " AND access_channel=:AccessChannelId ");
                parameters.Add(parm);
            }
            if (param.QFromDate > DateTime.MinValue && param.QFromDate != null)
            {
                WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE createdon>=:StartDate " : " AND createdon>=:StartDate   ");
                var parm = new OracleParameter { ParameterName = "StartDate", OracleDbType = OracleDbType.Date, Value = param.QFromDate };
                parameters.Add(parm);
            }
            if (param.QToDate > DateTime.MinValue && param.QToDate != null)
            {
                WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE createdon<=:EndDate " : " AND createdon<=:EndDate   ");
                var parm = new OracleParameter { ParameterName = "EndDate", OracleDbType = OracleDbType.Date, Value = param.QToDate };
                parameters.Add(parm);
            }


            if (param.QueryScope == "CurOpOnly")
            {
                WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE pos_id=:PartId " : " AND pos_id=:PartId ");
                var parm = new OracleParameter { ParameterName = "PartId", OracleDbType = OracleDbType.Varchar2, Value = param.CurrentUserId };
                parameters.Add(parm);
            }
            else if(param.QueryScope == "Exclusive")
            {
                WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ?
                    " WHERE (exists (select 1 from partner where (partner.partner_id = t.pos_id) and partner.ref_partner = '" + param.CurrentUserId + "'))"
                    : " AND  (exists (select 1 from partner where (partner.partner_id = t.pos_id) and partner.ref_partner = '" + param.CurrentUserId + "'))");
                var parm = new OracleParameter { ParameterName = "PartId", OracleDbType = OracleDbType.Varchar2, Value = param.CurrentUserId };
                parameters.Add(parm);
            }
            WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE ROWNUM <= 200 " : " AND ROWNUM <= 200 ");

            var masterDataTable = this.db.GetData("Select * from v_collection t " + WhereClause + " order by createdon desc", parameters);

            if (masterDataTable == null) return param;
            if (masterDataTable.Rows.Count == 0) return param;

            var results = new List<RechargeQueryResult>();
            foreach (DataRow row in masterDataTable.Rows)
            {
                var obj = new RechargeQueryResult();
                obj.Id = row["cl_id"] == DBNull.Value ? -1 : int.Parse(row["cl_id"].ToString());
                obj.SubsNo = row["subs_no"] == DBNull.Value ? string.Empty : row["subs_no"].ToString();
                obj.CreatedOn = row["createdon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["createdon"].ToString());
                obj.SubsNo = row["subs_no"] == DBNull.Value ? string.Empty : row["subs_no"].ToString();
                obj.Amount = row["amount"] == DBNull.Value ? 0 : double.Parse(row["amount"].ToString());
                obj.POSId = row["pos_id"] == DBNull.Value ? string.Empty : row["pos_id"].ToString();
                obj.POSAccount = row["pos_acc"] == DBNull.Value ? 0 :int.Parse(row["pos_acc"].ToString());
                obj.POSBalance = row["pos_bal"] == DBNull.Value ? 0 :double.Parse(row["pos_bal"].ToString());
                obj.POSName = row["pos_name"] == DBNull.Value ? string.Empty : row["pos_name"].ToString();
                obj.POSRoleId = row["pos_role_id"] == DBNull.Value ? 0 :int.Parse(row["pos_role_id"].ToString());
                obj.POSRoleName = row["pos_role_name"] == DBNull.Value ? string.Empty : row["pos_role_name"].ToString();
                obj.AccessChannelId = row["access_channel"] == DBNull.Value ? string.Empty : row["access_channel"].ToString();
                obj.AccessChannelName = row["access_channel_name"] == DBNull.Value ? string.Empty : row["access_channel_name"].ToString();
                obj.Status = row["status"] == DBNull.Value ? 0 :int.Parse(row["status"].ToString());
                obj.StatusName = row["status_name"] == DBNull.Value ? string.Empty : row["status_name"].ToString();
                obj.StatusOn = row["status_time"] == DBNull.Value ? DateTime.MinValue :DateTime.Parse(row["status_time"].ToString());
                obj.QueueNo = row["queue_no"] == DBNull.Value ? 0 :int.Parse(row["queue_no"].ToString());
                obj.RefNo = row["ref_no"] == DBNull.Value ? string.Empty : row["ref_no"].ToString();
                obj.RefMessage = row["ref_message"] == DBNull.Value ? string.Empty : row["ref_message"].ToString();
                obj.RefTime = row["ref_time"] == DBNull.Value ? DateTime.MinValue :DateTime.Parse(row["ref_time"].ToString());
                obj.RefTransNo = row["ref_trans_no"] == DBNull.Value ? string.Empty : row["ref_trans_no"].ToString();
                obj.DebugInfo = row["debug_info"] == DBNull.Value ? string.Empty : row["debug_info"].ToString();
                results.Add(obj);
            }
            param.Results = results;
            return param;
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
        public OpertionResult UpdateWithBalance(RechargeCollection rechargeCollection)
        {

            try
            {
                #region Parameters
                var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "retVal",OracleDbType = OracleDbType.Int32,  Direction = ParameterDirection.ReturnValue },
                 new OracleParameter{ ParameterName = "v_cl_id", OracleDbType = OracleDbType.Int32,  Value = rechargeCollection.Id},
                 new OracleParameter{ ParameterName = "v_status",OracleDbType = OracleDbType.Int32,  Value = rechargeCollection.Status.Id },
                 new OracleParameter{ ParameterName = "v_ref_no",OracleDbType = OracleDbType.Varchar2,  Value = rechargeCollection.RefNo },
                 new OracleParameter{ ParameterName = "v_ref_message",OracleDbType = OracleDbType.Varchar2,  Value = rechargeCollection.RefMessage },
                 new OracleParameter{ ParameterName = "v_ref_time",OracleDbType = OracleDbType.Date,  Value = rechargeCollection.RefTime },
                 new OracleParameter{ ParameterName = "v_ref_trans_no",OracleDbType = OracleDbType.Varchar2,  Value = rechargeCollection.RefTransNo },
                 new OracleParameter{ ParameterName = "v_debug_info",OracleDbType = OracleDbType.Varchar2,  Value = rechargeCollection.DebugInfo },
                 new OracleParameter{ ParameterName = "v_update_bal",OracleDbType = OracleDbType.Int32,  Value = 1 }
                };
                #endregion
                db.ExecuteStoredProc("pk_financial.fn_update_collection", parameters);
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
        public Queue<GrappedRecharge> GetPendingRechargeQueue(int queueNo, int count = 100)
        {
            var masterDataTable = db.GetData("Select * from v_pending_recharge_req t WHERE queue_no="+ queueNo + "  AND ROWNUM < "+ count + " order by dcreatedon", null);

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
                obj.Status = row["status"] == DBNull.Value ? 0 :int.Parse(row["status"].ToString());
                obj.StatusTime = row["status_time"] == DBNull.Value ? DateTime.MinValue :DateTime.Parse(row["status_time"].ToString());
                obj.QueueNo = row["queue_no"] == DBNull.Value ? 0 :int.Parse(row["queue_no"].ToString());
                obj.RefNo = row["ref_no"] == DBNull.Value ? string.Empty : row["ref_no"].ToString();
                obj.RefMessage = row["ref_message"] == DBNull.Value ? string.Empty : row["ref_message"].ToString();
                obj.RefTransNo = row["ref_trans_no"] == DBNull.Value ? string.Empty : row["ref_trans_no"].ToString();
                obj.RefTime = row["ref_time"] == DBNull.Value ? DateTime.MinValue :DateTime.Parse(row["ref_time"].ToString());
                obj.DebugInfo = row["debug_info"] == DBNull.Value ? string.Empty : row["debug_info"].ToString();
                results.Enqueue(obj);
            }
            return results;
        }
        public async Task<List<RechargeCollection>> GetCollectionsAsync(string whereClause, List<OracleParameter> parameters)
        {
            string strSql = $"select * from V_COLLECTION {whereClause} order by cl_id";
            DataTable masterDataTable;
            masterDataTable = await db.GetDataAsync(strSql, parameters);

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;

            var results = new List<RechargeCollection>();
            foreach (DataRow row in masterDataTable.Rows)
            {
                var obj = new RechargeCollection();
                obj.Id = row["cl_id"] == DBNull.Value ? 0 : int.Parse(row["cl_id"].ToString());
                obj.CreatedOn = row["createdon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["createdon"].ToString());
                obj.SubscriberNo = row["subs_no"] == DBNull.Value ? string.Empty : row["subs_no"].ToString();
                obj.Amount = row["amount"] == DBNull.Value ? 0 : double.Parse(row["amount"].ToString());
                obj.PointOfSale.Id = row["pos_id"] == DBNull.Value ? string.Empty : row["pos_id"].ToString();
                obj.PointOfSale.Account = row["pos_acc"] == DBNull.Value ? 0 :int.Parse(row["pos_acc"].ToString());
                obj.PointOfSale.Balance = row["pos_bal"] == DBNull.Value ? 0 : double.Parse(row["pos_bal"].ToString());
                obj.PointOfSale.Name = row["pos_name"] == DBNull.Value ? string.Empty : row["pos_name"].ToString();
                obj.PointOfSale.Role.Id = row["pos_role_id"] == DBNull.Value ? 0 : int.Parse(row["pos_role_id"].ToString());
                obj.PointOfSale.Role.Name = row["pos_role_name"] == DBNull.Value ? string.Empty : row["pos_role_name"].ToString();
                obj.AccessChannel.Id = row["access_channel"] == DBNull.Value ? string.Empty : row["access_channel"].ToString();
                obj.AccessChannel.Name = row["access_channel_name"] == DBNull.Value ? string.Empty : row["access_channel_name"].ToString();
                obj.Status.Id = row["status"] == DBNull.Value ? 0 : int.Parse(row["status"].ToString());
                obj.Status.Name = row["status_name"] == DBNull.Value ? string.Empty : row["status_name"].ToString();
                obj.StatusTime = row["status_time"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["status_time"].ToString());
                obj.QueueNo = row["queue_no"] == DBNull.Value ? 0 : int.Parse(row["queue_no"].ToString());
                obj.RefNo = row["ref_no"] == DBNull.Value ? string.Empty : row["ref_no"].ToString();
                obj.RefMessage = row["ref_message"] == DBNull.Value ? string.Empty : row["ref_message"].ToString();
                obj.RefTime = row["ref_time"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["ref_time"].ToString());
                obj.RefTransNo = row["ref_trans_no"] == DBNull.Value ? string.Empty : row["ref_trans_no"].ToString();
                obj.DebugInfo = row["debug_info"] == DBNull.Value ? string.Empty : row["debug_info"].ToString();
                results.Add(obj);
            }
            return results;
        }

        public async Task<List<ToExcelSchema.RechargeCollection>> GetCollectionsForExcelAsync(string whereClause, List<OracleParameter> parameters)
        {
            string strSql = $"select * from V_COLLECTION {whereClause} order by cl_id";
            DataTable masterDataTable;
            masterDataTable = await db.GetDataAsync(strSql, parameters);

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;

            var results = new List<ToExcelSchema.RechargeCollection>();
            foreach (DataRow row in masterDataTable.Rows)
            {
                var obj = new ToExcelSchema.RechargeCollection();
                obj.Id = row["cl_id"] == DBNull.Value ? 0 : int.Parse(row["cl_id"].ToString());
                obj.CreatedOn = row["createdon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["createdon"].ToString());
                obj.SubscriberNo = row["subs_no"] == DBNull.Value ? string.Empty : row["subs_no"].ToString();
                obj.Amount = row["amount"] == DBNull.Value ? 0 : double.Parse(row["amount"].ToString());
                obj.POSId = row["pos_id"] == DBNull.Value ? string.Empty : row["pos_id"].ToString();
                obj.POSAccount = row["pos_acc"] == DBNull.Value ? 0 : int.Parse(row["pos_acc"].ToString());
                obj.POSBalance = row["pos_bal"] == DBNull.Value ? 0 : double.Parse(row["pos_bal"].ToString());
                obj.POSName = row["pos_name"] == DBNull.Value ? string.Empty : row["pos_name"].ToString();
                obj.POSRoleName = row["pos_role_name"] == DBNull.Value ? string.Empty : row["pos_role_name"].ToString();
                obj.AccessChannel = row["access_channel_name"] == DBNull.Value ? string.Empty : row["access_channel_name"].ToString();
                obj.Status = row["status_name"] == DBNull.Value ? string.Empty : row["status_name"].ToString();
                obj.StatusTime = row["status_time"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["status_time"].ToString());
                obj.QueueNo = row["queue_no"] == DBNull.Value ? 0 : int.Parse(row["queue_no"].ToString());
                obj.RefNo = row["ref_no"] == DBNull.Value ? string.Empty : row["ref_no"].ToString();
                obj.RefMessage = row["ref_message"] == DBNull.Value ? string.Empty : row["ref_message"].ToString();
                obj.RefTime = row["ref_time"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["ref_time"].ToString());
                obj.RefTransNo = row["ref_trans_no"] == DBNull.Value ? string.Empty : row["ref_trans_no"].ToString();
                obj.DebugInfo = row["debug_info"] == DBNull.Value ? string.Empty : row["debug_info"].ToString();
                results.Add(obj);
            }
            return results;
        }
    }
}
