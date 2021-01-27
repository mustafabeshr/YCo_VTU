using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.Infra.Data
{
    public class AdjustmentRepo
    {
        private readonly IAppDbContext db;
        private readonly IPartnerManager partnerManager;

        public AdjustmentRepo(IAppDbContext db, IPartnerManager partnerManager)
        {
            this.db = db;
            this.partnerManager = partnerManager;
        }

        public OpertionResult Create(Adjustment adjust)
        {
            try
            {
                #region Parameters
                var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "retVal",OracleDbType = OracleDbType.Int32,  Direction = ParameterDirection.ReturnValue },
                 new OracleParameter{ ParameterName = "v_createdby", OracleDbType = OracleDbType.Varchar2,  Value = adjust.CreatedBy.Id },
                 new OracleParameter{ ParameterName = "v_moneytranferid",OracleDbType = OracleDbType.Int32,  Value = adjust.MoneyTransferId },
                 new OracleParameter{ ParameterName = "v_amount",OracleDbType = OracleDbType.Decimal,  Value = adjust.Amount },
                 new OracleParameter{ ParameterName = "v_access_channel",OracleDbType = OracleDbType.Varchar2,  Value = adjust.AccessChannel.Id },
                 new OracleParameter{ ParameterName = "v_note",OracleDbType = OracleDbType.Varchar2,  Value = adjust.Note },
                 new OracleParameter{ ParameterName = "v_createdbyacc",OracleDbType = OracleDbType.Varchar2,  Value = adjust.CreatedBy.Account }
                };

                #endregion
                db.ExecuteStoredProc("pk_financial.fn_create_adjustment", parameters);
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

        public Adjustment GetSingleOrDefault(int id)
        {
            var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "AdjustId", OracleDbType = OracleDbType.Int32,  Value = id }
            };
            var masterDataTable = this.db.GetData("Select * from v_Adjustment  where adj_id=:AdjustId  ", parameters);

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;

            DataRow row = masterDataTable.Rows[0];
            var adjustment = ConvertDataRowToAdjustment(row);
            
            return adjustment;
        }

        public List<Adjustment> GetListWithPaging(int id,int moneytransferId, string createdById, string partnerId, DateTime startDate, DateTime endDate, Paging paging)
        {
            string criteria = string.Empty;
            var parameters = BuildCriteria(id, moneytransferId, createdById, partnerId, startDate, endDate, ref criteria);
            var strSqlStatment = new StringBuilder();
            strSqlStatment.Append("Select * from ( ");
            strSqlStatment.Append("select rownum as seq , main_data.* from ( ");
            strSqlStatment.Append($"Select * from V_ADJUSTMENT t { criteria } order by t.createon desc ");
            strSqlStatment.Append(") main_data ) ");
            strSqlStatment.Append($"WHERE seq > ({paging.PageNo - 1}) * {paging.PageSize} AND ROWNUM <= {paging.PageSize}");
            var masterDataTable = this.db.GetData(strSqlStatment.ToString(), parameters);

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;


            var adjustList = new List<Adjustment>();
            foreach (DataRow row in masterDataTable.Rows)
            {
                var obj = ConvertDataRowToAdjustment(row);
                adjustList.Add(obj);
            }

            return adjustList;
        }

        public int GetCount(int id, int moneytransferId, string createdById, string partnerId, DateTime startDate, DateTime endDate)
        {
            string WhereClause = string.Empty;
            var parameters = BuildCriteria(id, moneytransferId, createdById, partnerId, startDate, endDate, ref WhereClause);
            var strSqlStatment = new StringBuilder();
            strSqlStatment.Append($"Select count(*) val from V_ADJUSTMENT t { WhereClause }");
            var count = this.db.GetIntScalarValue(strSqlStatment.ToString(), parameters);
            return count;
        }
        private List<OracleParameter> BuildCriteria(int id, int moneytransferId, string createdById, string partnerId, DateTime startDate, DateTime endDate, ref string criteria)
        {
            var WhereClause = new StringBuilder();
            var parameters = new List<OracleParameter>();
            if (id > 0)
            {
                var parm = new OracleParameter { ParameterName = "TransId", OracleDbType = OracleDbType.Int32, Value = id };
                WhereClause.Append(" WHERE t.adj_id =:TransId ");
                parameters.Add(parm);
            }
            if (moneytransferId > 0)
            {
                var parm = new OracleParameter { ParameterName = "TransferId", OracleDbType = OracleDbType.Int32, Value = moneytransferId };
                WhereClause.Append(" WHERE t.moneytransferid =:TransferId ");
                parameters.Add(parm);
            }
            if (!string.IsNullOrEmpty(createdById))
            {
              WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE t.createdby=:Created " : " AND t.createdby=:Created ");
              var parm = new OracleParameter { ParameterName = "Created", OracleDbType = OracleDbType.Varchar2, Value = createdById };
              parameters.Add(parm);
            }
            if (!string.IsNullOrEmpty(partnerId))
            {
                WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) 
                    ? " WHERE exists(select 1 from money_transfer m where m.trans_id = t.moneytranferid and (m.part_id = :PartId OR m.createdby = :PartId) "
                    : " AND exists(select 1 from money_transfer m where m.trans_id = t.moneytranferid and (m.part_id = :PartId OR m.createdby = :PartId) ");
                var parm = new OracleParameter { ParameterName = "PartId", OracleDbType = OracleDbType.Varchar2, Value = partnerId };
                parameters.Add(parm);
            }
            if (startDate > DateTime.MinValue && startDate != null)
            {
                WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE t.createon>=:StartDate " : " AND t.createon>=:StartDate   ");
                var parm = new OracleParameter { ParameterName = "StartDate", OracleDbType = OracleDbType.Date, Value = startDate };
                parameters.Add(parm);
            }
            if (endDate > DateTime.MinValue && endDate != null)
            {
                WhereClause.Append(string.IsNullOrEmpty(WhereClause.ToString()) ? " WHERE t.createon<=:EndDate " : " AND t.createon<=:EndDate   ");
                var parm = new OracleParameter { ParameterName = "EndDate", OracleDbType = OracleDbType.Date, Value = endDate };
                parameters.Add(parm);
            }
            criteria = WhereClause.ToString();
            return parameters;
        }

        private Adjustment ConvertDataRowToAdjustment(DataRow row)
        {
            var adjustment = new Adjustment();
            adjustment.Id = row["adj_id"] == DBNull.Value ? -1 : int.Parse(row["adj_id"].ToString());
            adjustment.MoneyTransferId = row["moneytranferid"] == DBNull.Value ? -1 : int.Parse(row["moneytranferid"].ToString());
            adjustment.CreatedOn = row["createon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["createon"].ToString());
            var accessChannel = row["access_channel"] == DBNull.Value ? string.Empty : row["access_channel"].ToString();
            adjustment.AccessChannel = new CommonCodeRepo(db).GetCodesById(accessChannel, "access.channel");
            adjustment.Amount = row["amount"] == DBNull.Value ? 0 : double.Parse(row["amount"].ToString());
            adjustment.TaxPercent = row["tax_per"] == DBNull.Value ? 0 : double.Parse(row["tax_per"].ToString());
            adjustment.TaxAmount = row["tax_amt"] == DBNull.Value ? 0 : double.Parse(row["tax_amt"].ToString());
            adjustment.BonusPercent = row["bonus_per"] == DBNull.Value ? 0 : double.Parse(row["bonus_per"].ToString());
            adjustment.BounsAmount = row["bonus_amt"] == DBNull.Value ? 0 : double.Parse(row["bonus_amt"].ToString());
            adjustment.BounsTaxPercent = row["bonus_tax"] == DBNull.Value ? 0 : double.Parse(row["bonus_tax"].ToString());
            adjustment.BounsTaxAmount = row["bonus_tax_amt"] == DBNull.Value ? 0 : double.Parse(row["bonus_tax_amt"].ToString());
            adjustment.ReceivedAmount = row["received_amt"] == DBNull.Value ? 0 : double.Parse(row["received_amt"].ToString());
            adjustment.NetAmount = row["net_amount"] == DBNull.Value ? 0 : double.Parse(row["net_amount"].ToString());
            adjustment.Note = row["note"] == DBNull.Value ? string.Empty : row["note"].ToString();
            adjustment.CreatedBy.Id = row["createdby"] == DBNull.Value ? string.Empty : row["createdby"].ToString();
            adjustment.CreatedBy.Account = row["createdbyacc"] == DBNull.Value ? -1 : int.Parse(row["createdbyacc"].ToString());
            adjustment.CreatedBy.Name = row["creator_name"] == DBNull.Value ? string.Empty : row["creator_name"].ToString();
            adjustment.PeriodDays = row["period_days"] == DBNull.Value ? -1 : int.Parse(row["period_days"].ToString());
            adjustment.SettingsPeriodDays = row["sett_period"] == DBNull.Value ? -1 : int.Parse(row["sett_period"].ToString());

            var srcPartnerAccount = row["src_acc"] == DBNull.Value ? -1 : int.Parse(row["src_acc"].ToString());
            adjustment.SrcPartner = partnerManager.GetPartnerByAccount(srcPartnerAccount);
            adjustment.SrcPartner.Balance = row["src_bal"] == DBNull.Value ? -1 : long.Parse(row["src_bal"].ToString());
            adjustment.SrcPartner.Role.Id = row["src_role_id"] == DBNull.Value ? -1 : int.Parse(row["src_role_id"].ToString());
            adjustment.SrcPartner.Role.Name = row["src_role_name"] == DBNull.Value ? string.Empty : row["src_role_name"].ToString();
            adjustment.SrcPartner.Status.Name = row["src_status_name"] == DBNull.Value ? string.Empty : row["src_status_name"].ToString();
            adjustment.SrcPartner.Status.Id = row["src_status"] == DBNull.Value ? -1 : int.Parse(row["src_status"].ToString());

            var destPartnerAccount = row["dest_acc"] == DBNull.Value ? -1 : int.Parse(row["dest_acc"].ToString());
            adjustment.DestPartner = partnerManager.GetPartnerByAccount(destPartnerAccount);
            adjustment.DestPartner.Balance = row["dest_bal"] == DBNull.Value ? -1 : long.Parse(row["dest_bal"].ToString());
            adjustment.DestPartner.Role.Id = row["dest_role_id"] == DBNull.Value ? -1 : int.Parse(row["dest_role_id"].ToString());
            adjustment.DestPartner.Role.Name = row["dest_role_name"] == DBNull.Value ? string.Empty : row["dest_role_name"].ToString();
            adjustment.DestPartner.Status.Name = row["dest_status_name"] == DBNull.Value ? string.Empty : row["dest_status_name"].ToString();
            adjustment.DestPartner.Status.Id = row["dest_status"] == DBNull.Value ? -1 : int.Parse(row["dest_status"].ToString());
            return adjustment;
        }
    }
}
