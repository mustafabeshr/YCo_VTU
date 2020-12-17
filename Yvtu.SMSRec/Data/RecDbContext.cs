﻿using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Yvtu.Infra.Data.Interfaces;
using Yvtu.SMSRec;
using static Yvtu.Infra.Data.Interfaces.IAppDbContext;

namespace Yvtu.SMSRec
{
    //public class OracleCommandParameters
    //{
    //    public string ParamName { get; set; }
    //    public string ParamType { get; set; }
    //    public string ParamValue { get; set; }
    //}

    public class RecDbContext : IRecDbContext
    {
        public IConfiguration Configuration { get; }
        public RecDbContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public int ExecuteSqlCommand(string sql, IEnumerable<OracleParameter> parameters)
        {
            using (var conn = GetConnection("DbConn"))
            {
                var cmd = GetCommand(sql, parameters);
                cmd.Connection = conn;
                if (conn.State != ConnectionState.Open) conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public int ExecuteStoredProc(string spName, IEnumerable<OracleParameter> parameters)
        {
            using (var conn = GetConnection("DbConn"))
            {
                var cmd = GetCommand(spName, parameters);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                if (conn.State != ConnectionState.Open) conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public async Task<int> ExecuteStoredProcAsync(string spName, IEnumerable<OracleParameter> parameters)
        {
            using (var conn = GetConnection("DbConn"))
            {
                var cmd = GetCommand(spName, parameters);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                if (conn.State != ConnectionState.Open) conn.Open();
                return await cmd.ExecuteNonQueryAsync();
            }
        }

        public int ExecuteFunction(string spName, IEnumerable<OracleParameter> parameters)
        {
            using (var conn = GetConnection("DbConn"))
            {
                var cmd = GetCommand(spName, parameters);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                if (conn.State != ConnectionState.Open) conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        private OracleCommand GetCommand(string sql, IEnumerable<OracleParameter> parameters)
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

        public DataTable GetData(string sql, IEnumerable<OracleParameter> parameters)
        {
            using (var conn = GetConnection("DbConn"))
            {
                var cmd = GetCommand(sql, parameters);
                cmd.Connection = conn;
                if (conn.State != ConnectionState.Open) conn.Open();
                var da = new OracleDataAdapter(cmd);
                var ds = new DataSet();
                da.Fill(ds);
                return ds.Tables[0];
            }
        }

        private OracleConnection GetConnection(string connectionStringName)
        {
            var connString = Configuration.GetConnectionString(connectionStringName);
            return new OracleConnection(connString);
        }

        public async Task<int> ExecuteSqlCommandAsync(string sql, IEnumerable<OracleParameter> parameters)
        {
            using (var conn = GetConnection("DbConn"))
            {
                var cmd = GetCommand(sql, parameters);
                cmd.Connection = conn;
                if (conn.State != ConnectionState.Open) conn.Open();
                return await cmd.ExecuteNonQueryAsync();
            }
        }

    }
}