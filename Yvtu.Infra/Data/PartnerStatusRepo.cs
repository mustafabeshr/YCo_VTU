using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.Infra.Data
{
    public class PartnerStatusRepo
    {
        private readonly IAppDbContext db;

        public PartnerStatusRepo(IAppDbContext db)
        {
            this.db = db;
        }

        public List<PartnerStatus> GetStatusList()
        {
            var statusDataTable = this.db.GetData("Select * from PARTNER_STATUS", null);

            var statusList = new List<PartnerStatus>();
            if (statusDataTable != null)
            {
                foreach (DataRow row in statusDataTable.Rows)
                {
                    var status = new PartnerStatus();
                    status.Id = row["statusid"] == DBNull.Value ? 0 : int.Parse(row["statusid"].ToString());
                    status.Name = row["statusname"] == DBNull.Value ? string.Empty : row["statusname"].ToString();
                    statusList.Add(status);
                }
            }
            return statusList;
        }
        public PartnerStatus GetStatus(int id)
        {
            var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "statusId", OracleDbType = OracleDbType.Int32,  Value = id },
            };
            var statusDataTable = this.db.GetData("Select * from PARTNER_STATUS where statusid = :statusId", parameters);
            var status = new PartnerStatus();
            if (statusDataTable != null)
            {
               DataRow row = statusDataTable.Rows[0];
                status.Id = row["statusid"] == DBNull.Value ? 0 : int.Parse(row["statusid"].ToString());
                status.Name = row["statusname"] == DBNull.Value ? string.Empty : row["statusname"].ToString();
            }
            return status;
        }
    }
}
