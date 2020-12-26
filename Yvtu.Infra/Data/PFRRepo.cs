using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.Infra.Data
{
    public class PFRRepo
    {
        private readonly IAppDbContext db;

        public PFRRepo(IAppDbContext db)
        {
            this.db = db;
        }

        public List<PFR> GetList(int Account, string id, bool IncludeDates,DateTime StartDate , DateTime EndDate)
        {
            if (Account <= 0 && string.IsNullOrEmpty(id)) return null;
            #region Parameters
            var parameters = new List<OracleParameter>();
            var whereCluase = new StringBuilder();
            if (Account > 0)
            {
             whereCluase.Append(" WHERE partner_acc = :PartAcc");
             var p = new OracleParameter { ParameterName = "PartAcc", OracleDbType = OracleDbType.Int32, Value = Account };
             parameters.Add(p);
            }
            if (!string.IsNullOrEmpty(id))
            {
                whereCluase.Append(whereCluase.Length > 0 ? " AND partner_id = :pId" : " WHERE partner_id = :pId");
                var p = new OracleParameter { ParameterName = "pId", OracleDbType = OracleDbType.Varchar2, Value = id };
                parameters.Add(p);
            }
            if (IncludeDates)
            {
                if (StartDate > DateTime.MinValue && StartDate != null)
                {
                    whereCluase.Append(whereCluase.Length > 0 ? " AND createdon >= :StartDate" : " WHERE createdon >= :StartDate");
                    var p = new OracleParameter { ParameterName = "StartDate", OracleDbType = OracleDbType.Date, Value = StartDate };
                    parameters.Add(p);
                }
                if (EndDate > DateTime.MinValue && EndDate != null)
                {
                    whereCluase.Append(whereCluase.Length > 0 ? " AND createdon <= :EndDate" : " WHERE createdon <= :EndDate");
                    var p = new OracleParameter { ParameterName = "EndDate", OracleDbType = OracleDbType.Date, Value = EndDate.Add(TimeSpan.FromDays(1)) };
                    parameters.Add(p);
                }
            }
            #endregion

            string strSql = $"select * from V_PFR {whereCluase} order by createdon";

            DataTable masterDataTable;
            masterDataTable = db.GetData(strSql, parameters);

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;

            var results = new List<PFR>();
            foreach (DataRow row in masterDataTable.Rows)
            {
                var obj = ConvertDataRowToPFR(row);
                results.Add(obj);
            }
            return results;
        }

        private PFR ConvertDataRowToPFR(DataRow row)
        {
            var obj = new PFR();
            obj.Id = row["pfr_id"] == DBNull.Value ? -1 : int.Parse(row["pfr_id"].ToString());
            obj.CreatedOn = row["createdon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["createdon"].ToString());
            obj.ActivityTime = row["act_time"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["act_time"].ToString());
            obj.CreatedBy.Account = row["createdbyacc"] == DBNull.Value ? -1 : int.Parse(row["createdbyacc"].ToString());
            obj.CreatedBy.Id = row["creatorbyid"] == DBNull.Value ? string.Empty : row["creatorbyid"].ToString();
            obj.CreatedBy.Name = row["creatorbyname"] == DBNull.Value ? string.Empty : row["creatorbyname"].ToString();
            obj.PartnerId = row["partner_id"] == DBNull.Value ? string.Empty : row["partner_id"].ToString();
            obj.PartnerAccount = row["partner_acc"] == DBNull.Value ? -1 : int.Parse(row["partner_acc"].ToString());
            obj.Amount = row["amount"] == DBNull.Value ? 0 : double.Parse(row["amount"].ToString());
            obj.ActivityId = row["act_id"] == DBNull.Value ? string.Empty : row["act_id"].ToString();
            obj.ActivityName = row["act_name"] == DBNull.Value ? string.Empty : row["act_name"].ToString();
            obj.TransNo = row["act_no"] == DBNull.Value ? -1 : int.Parse(row["act_no"].ToString());
            return obj;
        }
    }
}
