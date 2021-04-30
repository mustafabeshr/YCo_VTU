using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Oracle.ManagedDataAccess.Client;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.Infra.Data
{
    public class PaymentValuesRepo
    {
        private readonly IAppDbContext _db;
        private readonly IPartnerManager _partnerManager;

        public PaymentValuesRepo(IAppDbContext db, IPartnerManager partnerManager)
        {
            _db = db;
            _partnerManager = partnerManager;
        }
        public OpertionResult Create(PaymentValues pValue)
        {
            try
            {
                #region Parameters
                var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "retVal",OracleDbType = OracleDbType.Int32,  Direction = ParameterDirection.ReturnValue },
                 new OracleParameter{ ParameterName = "v_pvalue", OracleDbType = OracleDbType.Double,  Value = pValue.PayValue },
                 new OracleParameter{ ParameterName = "v_createdby",OracleDbType = OracleDbType.Varchar2,  Value = pValue.CreatedBy.Id },
                 new OracleParameter{ ParameterName = "v_created_acc",OracleDbType = OracleDbType.Int32,  Value = pValue.CreatedBy.Account }
                };
                #endregion
                _db.ExecuteStoredProc("pk_infra.fn_create_pay_values", parameters);
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

        public PaymentValues GetSingleOrDefault(double pValue)
        {
            var parameters = new List<OracleParameter> {
                new OracleParameter{ ParameterName = "pValue", OracleDbType = OracleDbType.Double,  Value = pValue }
            };
            var masterDataTable = this._db.GetData("Select * from pay_values  where pvalue=:pValue ", parameters);

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;

            DataRow row = masterDataTable.Rows[0];
            var paymentVlue = ConvertDataRowToPaymentValues(row);
            return paymentVlue;
        }

        public List<PaymentValues> GetAll()
        {
            //var parameters = new List<OracleParameter> {
            //    new OracleParameter{ ParameterName = "pValue", OracleDbType = OracleDbType.Decimal,  Value = pValue }
            //};
            var masterDataTable = this._db.GetData("Select * from pay_values  order by pvalue ", null);

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;
            var paymentVlaues = new List<PaymentValues>();
            foreach (DataRow row in masterDataTable.Rows)
            {
                var obj = ConvertDataRowToPaymentValues(row);
                paymentVlaues.Add(obj);
            }

            return paymentVlaues;

        }

        public OpertionResult Remove(double pValue)
        {
            try
            {
                #region Parameters
                var parameters = new List<OracleParameter> {
                    new OracleParameter{ ParameterName = "retVal",OracleDbType = OracleDbType.Int32,  Direction = ParameterDirection.ReturnValue },
                    new OracleParameter{ ParameterName = "v_pvalue", OracleDbType = OracleDbType.Decimal,  Value = pValue }
                };
                #endregion
                _db.ExecuteStoredProc("pk_infra.fn_delete_pay_values", parameters);
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
        private PaymentValues ConvertDataRowToPaymentValues(DataRow row)
        {
            var appObj = new PaymentValues();
            appObj.Seq = row["seq"] == DBNull.Value ? -1 : int.Parse(row["seq"].ToString());
            appObj.PayValue = row["pvalue"] == DBNull.Value ? -1 : double.Parse(row["pvalue"].ToString());
            appObj.CreatedOn = row["createdon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["createdon"].ToString());
            var creatorAccount = row["created_acc"] == DBNull.Value ? -1 : int.Parse(row["created_acc"].ToString());

            var creator = _partnerManager.GetPartnerByAccount(creatorAccount);
            appObj.CreatedBy.Account = creatorAccount;
            appObj.CreatedBy.Id = creator.Id;
            appObj.CreatedBy.Name = creator.Name;
            
            return appObj;
        }
    }
}
