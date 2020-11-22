using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.Infra.Data
{
    public class MoneyTransferRepo
    {
        private readonly IAppDbContext db;

        public MoneyTransferRepo(IAppDbContext db)
        {
            this.db = db;
        }

        public OpertionResult Create(MoneyTransfer transfer)
        {
            try
            {
                #region Parameters
                var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "retVal",OracleDbType = OracleDbType.Int32,  Direction = ParameterDirection.ReturnValue },
                 new OracleParameter{ ParameterName = "v_part_id", OracleDbType = OracleDbType.Varchar2,  Value = transfer.Partner.Id },
                 new OracleParameter{ ParameterName = "v_pay_type",OracleDbType = OracleDbType.Varchar2,  Value = transfer.PayType.Id },
                 new OracleParameter{ ParameterName = "v_pay_no",OracleDbType = OracleDbType.Varchar2,  Value = transfer.PayNo },
                 new OracleParameter{ ParameterName = "v_pay_date",OracleDbType = OracleDbType.Date,  Value = transfer.PayDate },
                 new OracleParameter{ ParameterName = "v_bank_name",OracleDbType = OracleDbType.Varchar2,  Value = transfer.PayBank },
                 new OracleParameter{ ParameterName = "v_createdby",OracleDbType = OracleDbType.Varchar2,  Value = transfer.CreatedBy.Id },
                 new OracleParameter{ ParameterName = "v_access_channel",OracleDbType = OracleDbType.Varchar2,  Value = "web" },
                 new OracleParameter{ ParameterName = "v_amount",OracleDbType = OracleDbType.Decimal,  Value = transfer.Amount },
                 new OracleParameter{ ParameterName = "v_bill_no",OracleDbType = OracleDbType.Varchar2,  Value = transfer.BillNo },
                 new OracleParameter{ ParameterName = "v_request_no",OracleDbType = OracleDbType.Varchar2,  Value = transfer.RequestNo },
                 new OracleParameter{ ParameterName = "v_request_amt",OracleDbType = OracleDbType.Decimal,  Value = transfer.RequestAmount },
                 new OracleParameter{ ParameterName = "v_note",OracleDbType = OracleDbType.Varchar2,  Value = transfer.Note }
                };

                #endregion
                db.ExecuteStoredProc("pk_financial.fn_MoneyTransfer", parameters);
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
