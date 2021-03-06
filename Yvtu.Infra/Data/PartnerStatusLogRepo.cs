﻿using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.Infra.Data
{
    public class PartnerStatusLogRepo
    {
        private readonly IAppDbContext db;
        private readonly IPartnerManager partnerManager;

        public class GetListParam
        {
            public string CreatedById { get; set; }
            public int CreatedByAccount { get; set; }
            public string PartnerId { get; set; }
            public int PartnerAccount { get; set; }
            public bool IncludeDates { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }

        public PartnerStatusLogRepo(IAppDbContext db, IPartnerManager partnerManager)
        {
            this.db = db;
            this.partnerManager = partnerManager;
        }
        public OpertionResult Create(PartnerStatusLog log)
        {
            try
            {
                #region Parameters
                var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "retVal",OracleDbType = OracleDbType.Int32,  Direction = ParameterDirection.ReturnValue },
                 new OracleParameter{ ParameterName = "v_createdby",OracleDbType = OracleDbType.Varchar2,  Value = log.CreatedBy.Id },
                 new OracleParameter{ ParameterName = "v_createdbyacc",OracleDbType = OracleDbType.Int32,  Value = log.CreatedBy.Account },
                 new OracleParameter{ ParameterName = "v_partner_id",OracleDbType = OracleDbType.Varchar2,  Value = log.Partner.Id },
                 new OracleParameter{ ParameterName = "v_partner_acc",OracleDbType = OracleDbType.Int32,  Value = log.Partner.Account },
                 new OracleParameter{ ParameterName = "v_old_status",OracleDbType = OracleDbType.Int32,  Value = log.OldStatus.Id },
                 new OracleParameter{ ParameterName = "v_new_status",OracleDbType = OracleDbType.Int32,  Value = log.NewStatus.Id },
                 new OracleParameter{ ParameterName = "v_note",OracleDbType = OracleDbType.NVarchar2,  Value = log.Note },
                 new OracleParameter{ ParameterName = "v_newstatus_expireon",OracleDbType = OracleDbType.Date,  Value = log.NewStatusExpireOn }
                };
                #endregion
                db.ExecuteStoredProc("pk_infra.fn_create_partnerstatus_log", parameters);
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

        public List<PartnerStatusLog> GetLogByPartnerId(string id)
        {
            #region Parameters
            var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "PartnerId",OracleDbType = OracleDbType.Varchar2,  Value = id }
                };
            #endregion
            DataTable masterDataTable;
            masterDataTable = db.GetData("select * from partner_status_log where partner_id=:PartnerId order by log_id", parameters);

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;

            var results = new List<PartnerStatusLog>();
            foreach (DataRow row in masterDataTable.Rows)
            {
                var obj = ConvertDataRowToPartnerStatusLog(row);
                results.Add(obj);
            }
            return results;
        }
        public List<PartnerStatusLog> GetLogByPartnerAccount(int id)
        {
            #region Parameters
            var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "PartnerAccount",OracleDbType = OracleDbType.Int32,  Value = id }
                };
            #endregion
            DataTable masterDataTable;
            masterDataTable = db.GetData("select * from partner_status_log where partner_acc=:PartnerAccount order by log_id", parameters);

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;

            var results = new List<PartnerStatusLog>();
            foreach (DataRow row in masterDataTable.Rows)
            {
                var obj = ConvertDataRowToPartnerStatusLog(row);
                results.Add(obj);
            }
            return results;
        }

        public List<PartnerStatusLog> GetList(GetListParam param)
        {
            #region Parameters

            var parameters = new List<OracleParameter>();
            var whereCluase = new StringBuilder();
            if (param != null)
            {

                if (!string.IsNullOrEmpty(param.PartnerId))
                {
                    whereCluase.Append(" WHERE partner_id = :PartnerId");
                    var p = new OracleParameter { ParameterName = "PartnerId", OracleDbType = OracleDbType.Varchar2, Value = param.PartnerId };
                    parameters.Add(p);
                }
                if (!string.IsNullOrEmpty(param.CreatedById))
                {
                    whereCluase.Append(whereCluase.Length > 0 ? " AND createdby = :CreatedById" : " WHERE createdby = :CreatedById");
                    var p = new OracleParameter { ParameterName = "CreatedById", OracleDbType = OracleDbType.Varchar2, Value = param.CreatedById };
                    parameters.Add(p);
                }

                if (param.CreatedByAccount > 0)
                {
                    whereCluase.Append(whereCluase.Length > 0 ? " AND createdbyacc = :CreatedByAccount" : " WHERE createdbyacc = :CreatedByAccount");
                    var p = new OracleParameter { ParameterName = "CreatedByAccount", OracleDbType = OracleDbType.Int32, Value = param.CreatedByAccount };
                    parameters.Add(p);
                }
                if (param.PartnerAccount > 0)
                {
                    whereCluase.Append(whereCluase.Length > 0 ? " AND partner_acc = :PartnerAccount" : " WHERE partner_acc = :PartnerAccount");
                    var p = new OracleParameter { ParameterName = "PartnerAccount", OracleDbType = OracleDbType.Int32, Value = param.PartnerAccount };
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

            string strSql = $"select * from PARTNER_STATUS_LOG {whereCluase} order by log_id DESC";

            DataTable masterDataTable;
            masterDataTable = db.GetData(strSql, parameters);

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;

            var results = new List<PartnerStatusLog>();
            foreach (DataRow row in masterDataTable.Rows)
            {
                var obj = ConvertDataRowToPartnerStatusLog(row);
                results.Add(obj);
            }
            return results;
        }

        private PartnerStatusLog ConvertDataRowToPartnerStatusLog(DataRow row)
        {
            var obj = new PartnerStatusLog();
            obj.Id = row["log_id"] == DBNull.Value ? -1 : int.Parse(row["log_id"].ToString());
            obj.Note = row["note"] == DBNull.Value ? string.Empty : row["note"].ToString();
            var partnerAccount = row["partner_acc"] == DBNull.Value ? -1 : int.Parse(row["partner_acc"].ToString());
            obj.Partner = partnerManager.GetPartnerByAccount(partnerAccount);
            var oldStatus = row["old_status"] == DBNull.Value ? -1 : int.Parse(row["old_status"].ToString());
            obj.OldStatus = new PartnerStatusRepo(db).GetStatus(oldStatus);
            var newStatus = row["new_status"] == DBNull.Value ? -1 : int.Parse(row["new_status"].ToString());
            obj.NewStatus = new PartnerStatusRepo(db).GetStatus(newStatus);
            obj.CreatedOn = row["createdon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["createdon"].ToString());
            obj.NewStatusExpireOn = row["newstatus_expireon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["newstatus_expireon"].ToString());
            var createdAccount = row["createdbyacc"] == DBNull.Value ? -1 : int.Parse(row["createdbyacc"].ToString());
            var creator = partnerManager.GetPartnerByAccount(createdAccount);
            obj.CreatedBy.Id = creator.Id;
            obj.CreatedBy.Account = creator.Account;
            obj.CreatedBy.Name = creator.Name;
            return obj;
        }
    }
}
