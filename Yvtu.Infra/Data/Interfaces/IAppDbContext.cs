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

        Task<int> ExecuteSqlCommandAsync(string sql, IEnumerable<OracleParameter> parameters);
    }
}
