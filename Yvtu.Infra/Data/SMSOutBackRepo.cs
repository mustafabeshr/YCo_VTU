using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.Infra.Data
{
    public class SMSOutBackRepo
    {
        public class GetListParam
        {
            public string Receiver { get; set; }
            public string Message { get; set; }
            public bool IncludeDates { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }
        private readonly IAppDbContext db;
        private readonly IPartnerManager partnerManager;

        public SMSOutBackRepo(IAppDbContext db, IPartnerManager partnerManager)
        {
            this.db = db;
            this.partnerManager = partnerManager;
        }

        public List<SMSOutBack> GetList(GetListParam param)
        {
            #region Parameters

            var parameters = new List<OracleParameter>();
            var whereCluase = new StringBuilder();
            if (param != null)
            {

                if (!string.IsNullOrEmpty(param.Receiver))
                {
                    whereCluase.Append(" WHERE receiver = :Receiver");
                    var p = new OracleParameter { ParameterName = "Receiver", OracleDbType = OracleDbType.Varchar2, Value = param.Receiver };
                    parameters.Add(p);
                }
                
                if (param.IncludeDates)
                {
                    if (param.StartDate > DateTime.MinValue && param.StartDate != null)
                    {
                        whereCluase.Append(whereCluase.Length > 0 ? " AND createdon >= :StartDate" : " WHERE createdon >= :StartDate");
                        var p = new OracleParameter { ParameterName = "StartDate", OracleDbType = OracleDbType.Date, Value = param.StartDate };
                        parameters.Add(p);
                    }
                    if (param.EndDate > DateTime.MinValue && param.EndDate != null)
                    {
                        whereCluase.Append(whereCluase.Length > 0 ? " AND createdon <= :EndDate" : " WHERE createdon <= :EndDate");
                        var p = new OracleParameter { ParameterName = "EndDate", OracleDbType = OracleDbType.Date, Value = param.EndDate.AddDays(1) };
                        parameters.Add(p);
                    }
                }
                if (!string.IsNullOrEmpty(param.Message))
                {
                    whereCluase.Append(whereCluase.Length > 0 ? " AND (message LIKE '%' ||  :Message || '%') " : " WHERE (message LIKE '%' ||  :Message || '%') ");
                    var p = new OracleParameter { ParameterName = "Message", OracleDbType = OracleDbType.Varchar2, Value = param.Message };
                    parameters.Add(p);
                }
            }

            #endregion

            string strSql = $"select * from v_smsout {whereCluase} order by row_id";

            DataTable masterDataTable;
            masterDataTable = db.GetData(strSql, parameters);

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;

            var results = new List<SMSOutBack>();
            foreach (DataRow row in masterDataTable.Rows)
            {
                var obj = new SMSOutBack();
                obj.Id = row["row_id"] == DBNull.Value ? -1 : int.Parse(row["row_id"].ToString());
                obj.Receiver = row["receiver"] == DBNull.Value ? string.Empty : row["receiver"].ToString();
                obj.Sender = row["sender"] == DBNull.Value ? string.Empty : row["sender"].ToString();
                obj.Message = row["message"] == DBNull.Value ? string.Empty : row["message"].ToString();
                obj.CreatedOn = row["createdon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["createdon"].ToString());
                obj.BackedOn = row["backedon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["backedon"].ToString());
                results.Add(obj);
            }
            return results;
        }
    }
}
