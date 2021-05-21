using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace Yvtu.RechargePrcFW.lib
{
    public class PaymentValuesRepo
    {
        public Entities.PaymentValues GetSingleOrDefault(double pValue)
        {
            var parameters = new List<OracleParameter> {
                new OracleParameter{ ParameterName = "pValue", OracleDbType = OracleDbType.Double,  Value = pValue }
            };
            var masterDataTable = DB.GetDataTable("Select * from pay_values  where pvalue=:pValue ", parameters);

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;

            DataRow row = masterDataTable.Rows[0];
            var paymentVlue = ConvertDataRowToPaymentValues(row);
            return paymentVlue;
        }
        private Entities.PaymentValues ConvertDataRowToPaymentValues(DataRow row)
        {
            var appObj = new Entities.PaymentValues();
            appObj.Seq = row["seq"] == DBNull.Value ? -1 : int.Parse(row["seq"].ToString());
            appObj.PayValue = row["pvalue"] == DBNull.Value ? -1 : double.Parse(row["pvalue"].ToString());
            appObj.ProfileId = row["profile_id"] == DBNull.Value ? -1 : double.Parse(row["profile_id"].ToString());

            return appObj;
        }
    }
}
