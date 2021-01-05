﻿using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.Infra.Data
{
    public class BackgroundServiceRepo
    {
        public class BackgroundServiceListParam
        {
            public int Id { get; set; }
            public bool IncludeDates { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string Source { get; set; }
            public string CreatedById { get; set; }
            public string PartnerId { get; set; }
            public string Status { get; set; }
        }

        private readonly IAppDbContext db;

        public BackgroundServiceRepo(IAppDbContext db)
        {
            this.db = db;
        }

        public OpertionResult Create(BackgroundService obj)
        {

            try
            {
                #region Parameters
                var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "retVal",OracleDbType = OracleDbType.Int32,  Direction = ParameterDirection.ReturnValue },
                 new OracleParameter{ ParameterName = "v_createdby",OracleDbType = OracleDbType.Varchar2,  Value = obj.CreatedBy.Id },
                 new OracleParameter{ ParameterName = "v_createdbyacc",OracleDbType = OracleDbType.Int32,  Value = obj.CreatedBy.Account },
                 new OracleParameter{ ParameterName = "v_bg_source",OracleDbType = OracleDbType.Varchar2,  Value = obj.Source.Id },
                 new OracleParameter{ ParameterName = "v_partner_id",OracleDbType = OracleDbType.Varchar2,  Value = obj.Partner.Id },
                 new OracleParameter{ ParameterName = "v_partner_acc",OracleDbType = OracleDbType.Int32,  Value = obj.Partner.Account },
                 new OracleParameter{ ParameterName = "v_start_date",OracleDbType = OracleDbType.Date,  Value = obj.StartDate },
                 new OracleParameter{ ParameterName = "v_end_date",OracleDbType = OracleDbType.Date,  Value = obj.EndDate },
                 new OracleParameter{ ParameterName = "v_note",OracleDbType = OracleDbType.Varchar2,  Value = obj.Note },
                 new OracleParameter{ ParameterName = "v_active_time",OracleDbType = OracleDbType.Date,  Value = obj.ActiveTime }
                };
                #endregion
                db.ExecuteStoredProc("pk_infra.fn_create_bgservice", parameters);
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

        public async Task<List<BackgroundService>> GetBackgroundServices(BackgroundServiceListParam param)
        {
            #region Parameters
            var parameters = new List<OracleParameter>();
            var whereCluase = new StringBuilder();
            if (param != null)
            {
                if (!string.IsNullOrEmpty(param.Source))
                {
                    whereCluase.Append(" WHERE bg_source = :Source");
                    var p = new OracleParameter { ParameterName = "Source", OracleDbType = OracleDbType.Varchar2, Value = param.Source };
                    parameters.Add(p);
                }

                if (param.Id > 0)
                {
                    whereCluase.Append(whereCluase.Length > 0 ? " AND bg_id = :bgId" : " WHERE bg_id = :bgId");
                    var p = new OracleParameter { ParameterName = "bgId", OracleDbType = OracleDbType.Int32, Value = param.Id };
                    parameters.Add(p);
                }

                if (!string.IsNullOrEmpty(param.CreatedById))
                {
                    whereCluase.Append(whereCluase.Length > 0 ? " AND createdby = :CreatedById" : " WHERE createdby = :CreatedById");
                    var p = new OracleParameter { ParameterName = "CreatedById", OracleDbType = OracleDbType.Varchar2, Value = param.CreatedById };
                    parameters.Add(p);
                }

                if (!string.IsNullOrEmpty(param.PartnerId))
                {
                    whereCluase.Append(whereCluase.Length > 0 ? " AND partner_id = :PartnerId" : " WHERE partner_id = :PartnerId");
                    var p = new OracleParameter { ParameterName = "PartnerId", OracleDbType = OracleDbType.Varchar2, Value = param.PartnerId };
                    parameters.Add(p);
                }

                if (!string.IsNullOrEmpty(param.Status))
                {
                    whereCluase.Append(whereCluase.Length > 0 ? " AND status = :StatusId" : " WHERE status = :StatusId");
                    var p = new OracleParameter { ParameterName = "StatusId", OracleDbType = OracleDbType.Varchar2, Value = param.Status };
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
            }
            #endregion

            string strSql = $"select * from v_bg_service {whereCluase} order by bg_id";
            DataTable masterDataTable;
            masterDataTable = await db.GetDataAsync(strSql, parameters);

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;

            var results = new List<BackgroundService>();
            foreach (DataRow row in masterDataTable.Rows)
            {
                var obj = new BackgroundService();
                obj.Id = row["bg_id"] == DBNull.Value ? 0 : int.Parse(row["bg_id"].ToString());
                obj.CreatedOn = row["createdon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["createdon"].ToString());
                obj.CreatedBy.Account = row["createdbyacc"] == DBNull.Value ? 0 : int.Parse(row["createdbyacc"].ToString());
                obj.CreatedBy.Id = row["createdby"] == DBNull.Value ? string.Empty : row["createdby"].ToString();
                obj.CreatedBy.Name = row["createdby_name"] == DBNull.Value ? string.Empty : row["createdby_name"].ToString();
                obj.Source.Id = row["bg_source"] == DBNull.Value ? string.Empty : row["bg_source"].ToString();
                obj.Source.Name = row["bg_source_name"] == DBNull.Value ? string.Empty : row["bg_source_name"].ToString();
                obj.Partner.Account = row["partner_acc"] == DBNull.Value ? 0 : int.Parse(row["partner_acc"].ToString());
                obj.Partner.Id = row["partner_id"] == DBNull.Value ? string.Empty : row["partner_id"].ToString();
                obj.Partner.Name = row["partner_name"] == DBNull.Value ? string.Empty : row["partner_name"].ToString();
                obj.StartDate = row["start_date"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["start_date"].ToString());
                obj.EndDate = row["end_date"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["end_date"].ToString());
                obj.RecordCount = row["record_count"] == DBNull.Value ? 0 : int.Parse(row["record_count"].ToString());
                obj.DurationInSec = row["duration_sec"] == DBNull.Value ? 0 : int.Parse(row["duration_sec"].ToString());
                obj.Status.Id = row["status"] == DBNull.Value ? string.Empty : row["status"].ToString();
                obj.Status.Name = row["status_name"] == DBNull.Value ? string.Empty : row["status_name"].ToString();
                obj.StatusTime = row["status_time"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["status_time"].ToString());
                obj.FileName= row["file_name"] == DBNull.Value ? string.Empty : row["file_name"].ToString();
                obj.FileLocation= row["file_location"] == DBNull.Value ? string.Empty : row["file_location"].ToString();
                obj.Note= row["note"] == DBNull.Value ? string.Empty : row["note"].ToString();
                obj.ActiveTime = row["active_time"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["active_time"].ToString());
                results.Add(obj);
            }
            return results;
        }
    }
}
