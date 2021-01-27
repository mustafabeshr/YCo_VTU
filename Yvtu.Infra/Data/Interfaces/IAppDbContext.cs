using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Yvtu.Infra.Data.Interfaces
{
    public interface IAppDbContext
    {
        

        int ExecuteSqlCommand(string sql, IEnumerable<OracleParameter> parameters);
        DataTable GetData(string sql, IEnumerable<OracleParameter> parameters);
        Task<DataTable> GetDataAsync(string sql, IEnumerable<OracleParameter> parameters);

        Task<int> ExecuteSqlCommandAsync(string sql, IEnumerable<OracleParameter> parameters);
        int ExecuteStoredProc(string spName, IEnumerable<OracleParameter> parameters);
        Task<int> ExecuteStoredProcAsync(string spName, IEnumerable<OracleParameter> parameters);
        int ExecuteFunction(string spName, IEnumerable<OracleParameter> parameters);
        int GetIntScalarValue(string sql, IEnumerable<OracleParameter> parameters);
        double GetDoubleScalarValue(string sql, IEnumerable<OracleParameter> parameters);
    }
}
