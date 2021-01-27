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

        private OpertionResult GeneratePFR(int account, DateTime startDate, DateTime endDate)
        {
            try
            {
                #region Parameters
                var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "retVal",OracleDbType = OracleDbType.Int32,  Direction = ParameterDirection.ReturnValue },
                 new OracleParameter{ ParameterName = "account", OracleDbType = OracleDbType.Int32,  Value = account },
                 new OracleParameter{ ParameterName = "startdate",OracleDbType = OracleDbType.Date,  Value = startDate },
                 new OracleParameter{ ParameterName = "enddate",OracleDbType = OracleDbType.Date,  Value = endDate }
                };

                #endregion
                db.ExecuteStoredProc("pk_financial.fn_generate_pfr", parameters);
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


        public List<PFR> GetList(int Account, bool IncludeDates,DateTime StartDate , DateTime EndDate)
        {
            if (Account <= 0) return null;
            #region Parameters
            //var openingPFR = new PFR();
            var whereCluase = string.Empty;
            //if (StartDate > DateTime.MinValue && StartDate != null)
            //{
            //    openingPFR = GetPeriodOpeningBalance(Account, StartDate);
            //}
            var parameters = BuildParametersList(Account, IncludeDates, StartDate, EndDate, ref whereCluase);
            #endregion
            var strSqlStatment = new StringBuilder();
            strSqlStatment.Append($"select * from pfr_online {whereCluase} order by createdon");


            DataTable masterDataTable;
            masterDataTable = db.GetData(strSqlStatment.ToString(), parameters);
            var results = new List<PFR>();
            //if (openingPFR != null)
            //{
            //    results.Add(openingPFR);
            //}
            if (masterDataTable == null) return results;
            if (masterDataTable.Rows.Count == 0) return results;

            
            foreach (DataRow row in masterDataTable.Rows)
            {
                var obj = ConvertDataRowToPFR(row);
                results.Add(obj);
            }
            return results;
        }
        public List<PFR> GetListWithPaging(int Account, bool IncludeDates, DateTime StartDate, DateTime EndDate, Paging paging)
        {
            if (Account <= 0) return null;

            GeneratePFR(Account, StartDate, EndDate);
            #region Parameters
            var openingPFR = new PFR();
            var whereCluase = string.Empty;
            //if (StartDate > DateTime.MinValue && StartDate != null && paging.PageNo == 1)
            //{
            //    openingPFR = GetPeriodOpeningBalance(Account, StartDate);
            //}
                var parameters = BuildParametersList(Account, IncludeDates, StartDate, EndDate, ref whereCluase);
            #endregion
            var strSqlStatment = new StringBuilder();
            strSqlStatment.Append("Select * from ( ");
            strSqlStatment.Append("select rownum as seq , main_data.* from ( ");
            strSqlStatment.Append($"select * from pfr_online {whereCluase} order by createdon");
            strSqlStatment.Append(") main_data ) ");
            strSqlStatment.Append($"WHERE seq > ({paging.PageNo - 1}) * {paging.PageSize} AND ROWNUM <= {paging.PageSize}");


            DataTable masterDataTable;
            masterDataTable = db.GetData(strSqlStatment.ToString(), parameters);
            var results = new List<PFR>();
            //if (openingPFR != null)
            //{
            //    results.Add(openingPFR);
            //}
            if (masterDataTable == null) return results;
            if (masterDataTable.Rows.Count == 0) return results;


            foreach (DataRow row in masterDataTable.Rows)
            {
                var obj = ConvertDataRowToPFR(row);
                results.Add(obj);
            }
            return results;
        }
        public int GetCount(int Account, string id, bool IncludeDates, DateTime StartDate, DateTime EndDate)
        {
            string whereCluase = string.Empty;
            var parameters = BuildParametersList(Account, IncludeDates, StartDate, EndDate, ref whereCluase);
            var strSqlStatment = new StringBuilder();
            strSqlStatment.Append($"Select count(*) val from pfr_online  { whereCluase }");
            var count = this.db.GetIntScalarValue(strSqlStatment.ToString(), parameters);
            return count;
        }


        private List<OracleParameter>BuildParametersList(int Account, bool IncludeDates, DateTime StartDate, DateTime EndDate, ref string criteria)
        {
            var parameters = new List<OracleParameter>();
            var whereCluase = new StringBuilder();
            StartDate = StartDate.AddDays(-1);
            whereCluase.Append(" WHERE partner_acc = :PartAcc");
            var p = new OracleParameter { ParameterName = "PartAcc", OracleDbType = OracleDbType.Int32, Value = Account };
            parameters.Add(p);

             if (StartDate > DateTime.MinValue && StartDate != null)
                {
                    whereCluase.Append(whereCluase.Length > 0 ? " AND createdon >= :StartDate" : " WHERE createdon >= :StartDate");
                    p = new OracleParameter { ParameterName = "StartDate", OracleDbType = OracleDbType.Date, Value = StartDate };
                    parameters.Add(p);
                }
                if (EndDate > DateTime.MinValue && EndDate != null)
                {
                    whereCluase.Append(whereCluase.Length > 0 ? " AND createdon <= :EndDate" : " WHERE createdon <= :EndDate");
                    p = new OracleParameter { ParameterName = "EndDate", OracleDbType = OracleDbType.Date, Value = EndDate.Add(TimeSpan.FromDays(1)) };
                    parameters.Add(p);
                }
            
            criteria = whereCluase.ToString();
            return parameters;
        }

        private  PFR GetPeriodOpeningBalance(int Account, DateTime EndDate)
        {
            if (Account <= 0) return null;
            #region Parameters
            var parameters = new List<OracleParameter>();
            var whereCluase = new StringBuilder();
            whereCluase.Append(" WHERE partner_acc = :PartAcc");
            var p = new OracleParameter { ParameterName = "PartAcc", OracleDbType = OracleDbType.Int32, Value = Account };
            parameters.Add(p);
            if (EndDate > DateTime.MinValue && EndDate != null)
            {
                whereCluase.Append(whereCluase.Length > 0 ? " AND createdon <= :EndDate" : " WHERE createdon <= :EndDate");
                p = new OracleParameter { ParameterName = "EndDate", OracleDbType = OracleDbType.Date, Value = EndDate };
                parameters.Add(p);
            }
            #endregion

            string strSql = $"select sum(amount) amt from V_PFR {whereCluase}";

            DataTable masterDataTable;
            masterDataTable = db.GetData(strSql, parameters);

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;

            DataRow row = masterDataTable.Rows[0];
            var obj = new PFR();
            obj.Id = -1;
            obj.CreatedOn = EndDate;
            obj.ActivityTime = DateTime.MinValue;
            obj.CreatedBy.Account = 0;
            obj.CreatedBy.Id = string.Empty;
            obj.CreatedBy.Name = string.Empty;
            obj.PartnerId = string.Empty;
            obj.PartnerAccount = Account;
            obj.Amount = row["amt"] == DBNull.Value ? 0 : double.Parse(row["amt"].ToString());
            obj.ActivityId = string.Empty;
            obj.ActivityName = "رصيد افتتاحي";
            obj.TransNo = 0;
            
            return obj;
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
            obj.Balance = row["bal"] == DBNull.Value ? 0 : double.Parse(row["bal"].ToString());
            obj.ActivityId = row["act_id"] == DBNull.Value ? string.Empty : row["act_id"].ToString();
            obj.ActivityName = row["act_name"] == DBNull.Value ? string.Empty : row["act_name"].ToString();
            obj.TransNo = row["act_no"] == DBNull.Value ? -1 : int.Parse(row["act_no"].ToString());
            return obj;
        }
    }
}
