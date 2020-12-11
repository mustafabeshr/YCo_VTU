using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
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

    }
}
