using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.Infra.Data
{
    public class ConfiscationRepo
    {
        public class GetListParam
        {
            public int PartnerAccount { get; set; }
            public string PartnerId { get; set; }
            public int CreatorAccount { get; set; }
            public string CreatorId { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }
        private readonly IAppDbContext db;
        private readonly IPartnerManager partnerManager;

        public ConfiscationRepo(IAppDbContext db, IPartnerManager partnerManager)
        {
            this.db = db;
            this.partnerManager = partnerManager;
        }
        public OpertionResult Create(Confiscation created)
        {
            try
            {
                #region Parameters
                var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "retVal",OracleDbType = OracleDbType.Int32,  Direction = ParameterDirection.ReturnValue },
                 new OracleParameter{ ParameterName = "v_createdby",OracleDbType = OracleDbType.Varchar2,  Value = created.CreatedBy.Id },
                 new OracleParameter{ ParameterName = "v_createdbyacc",OracleDbType = OracleDbType.Int32,  Value = created.CreatedBy.Account },
                 new OracleParameter{ ParameterName = "v_partner_id",OracleDbType = OracleDbType.Varchar2,  Value = created.Partner.Id },
                 new OracleParameter{ ParameterName = "v_partner_acc",OracleDbType = OracleDbType.Int32,  Value = created.Partner.Account },
                 new OracleParameter{ ParameterName = "v_note",OracleDbType = OracleDbType.Varchar2,  Value = created.Note }
                };
                #endregion
                db.ExecuteStoredProc("pk_financial.fn_create_confiscation", parameters);
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


        public List<Confiscation> GetList(GetListParam param)
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
                    if (!string.IsNullOrEmpty(param.CreatorId))
                    {
                        whereCluase.Append(whereCluase.Length > 0 ? " WHERE createdby = :CreatorId" : " AND createdby = :CreatorId");
                        var p = new OracleParameter { ParameterName = "CreatorId", OracleDbType = OracleDbType.Varchar2, Value = param.CreatorId };
                        parameters.Add(p);
                    }
                    if (param.PartnerAccount > 0)
                    {
                        whereCluase.Append(whereCluase.Length > 0 ? " WHERE partner_acc = :PartnerAccount" : " AND partner_acc = :PartnerAccount");
                        var p = new OracleParameter { ParameterName = "PartnerAccount", OracleDbType = OracleDbType.Int32, Value = param.PartnerAccount };
                        parameters.Add(p);
                    }
                    if (param.CreatorAccount > 0)
                    {
                        whereCluase.Append(whereCluase.Length > 0 ? " WHERE partner_acc = :createdbyacc" : " AND createdbyacc = :CreatorAccount");
                        var p = new OracleParameter { ParameterName = "CreatorAccount", OracleDbType = OracleDbType.Int32, Value = param.CreatorAccount };
                        parameters.Add(p);
                    }
                    if (param.StartDate > DateTime.MinValue && param.StartDate != null)
                    {
                        whereCluase.Append(whereCluase.Length > 0 ? " WHERE createdon >= :StartDate" : " AND createdon >= :StartDate");
                        var p = new OracleParameter { ParameterName = "StartDate", OracleDbType = OracleDbType.Date, Value = param.StartDate };
                        parameters.Add(p);
                    }
                    if (param.EndDate > DateTime.MinValue && param.EndDate != null)
                    {
                        whereCluase.Append(whereCluase.Length > 0 ? " WHERE createdon <= :EndDate" : " AND createdon <= :EndDate");
                        var p = new OracleParameter { ParameterName = "EndDate", OracleDbType = OracleDbType.Date, Value = param.EndDate };
                        parameters.Add(p);
                    }
            }

            #endregion

            string strSql = $"select * from confiscation {whereCluase} order by con_id";

            DataTable masterDataTable;
            masterDataTable = db.GetData(strSql, parameters);

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;

            var results = new List<Confiscation>();
            foreach (DataRow row in masterDataTable.Rows)
            {
                var obj = new Confiscation();
                obj.Id = row["con_id"] == DBNull.Value ? -1 : int.Parse(row["con_id"].ToString());
                obj.Amount = row["balance"] == DBNull.Value ? -1 : double.Parse(row["balance"].ToString());
                obj.Note = row["note"] == DBNull.Value ? string.Empty : row["note"].ToString();
                obj.CreatedOn = row["createdon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["createdon"].ToString());
                var createdAccount = row["createdbyacc"] == DBNull.Value ? -1 : int.Parse(row["createdbyacc"].ToString());
                var createdBy = partnerManager.GetPartnerByAccount(createdAccount);
                obj.CreatedBy.Id = createdBy.Id;
                obj.CreatedBy.Name = createdBy.Name;
                obj.CreatedBy.Account = createdBy.Account;
                var partnerAccount = row["partner_acc"] == DBNull.Value ? -1 : int.Parse(row["partner_acc"].ToString());
                var partner = partnerManager.GetPartnerByAccount(partnerAccount);
                obj.Partner = partner;
                results.Add(obj);
            }
            return results;
        }
    }
}
