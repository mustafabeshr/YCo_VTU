using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yvtu.RechargePrcFW.lib
{
    class DB
    {
        private static OracleConnection GetConnection()
        {
            return new OracleConnection(SharedParams.ConString);
        }
        private static OracleCommand GetCommand(string sql, IEnumerable<OracleParameter> parameters)
        {
            var cmd = new OracleCommand(sql);
            if (parameters != null && parameters.Count() > 0)
            {
                foreach (var p in parameters)
                {
                    cmd.Parameters.Add(p);
                }
            }
            return cmd;
        }
        public static int ExecuteStoredProc(string spName, IEnumerable<OracleParameter> parameters)
        {
            using (var conn = GetConnection())
            {
                var cmd = GetCommand(spName, parameters);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                if (conn.State != ConnectionState.Open) conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public static int ExecuteQuery(string strsql, IEnumerable<OracleParameter> parameters, int timeout = 30)
        {
            using (var conn = GetConnection())
            {
                var cmd = GetCommand(strsql, parameters);
                cmd.Connection = conn;
                if (conn.State != ConnectionState.Open) conn.Open();
                return  cmd.ExecuteNonQuery();
            }
        }
        public static DataTable GetDataTable(string strsql, IEnumerable<OracleParameter> parameters)
        {
            
            using (var conn = GetConnection())
            {
                var cmd = GetCommand(strsql, parameters);
                cmd.Connection = conn;
                if (conn.State != ConnectionState.Open) conn.Open();
                var da = new OracleDataAdapter(cmd);
                var ds = new DataSet();
                da.Fill(ds);
                return ds.Tables[0];
            }
        }

      
    }
}
