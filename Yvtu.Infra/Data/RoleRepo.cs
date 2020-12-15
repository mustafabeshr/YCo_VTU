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
    public class RoleRepo
    {
        private readonly IAppDbContext db;
        private readonly IPartnerActivityRepo partnerActivityRepo;

        public RoleRepo(IAppDbContext db,  IPartnerActivityRepo partnerActivityRepo)
        {
            this.db = db;
            this.partnerActivityRepo = partnerActivityRepo;
        }

        public List<Role> GetRoles()
        {
            var rolesDataTable = this.db.GetData("Select * from AppRole where isactive = 1 order by roleorder", null);

            var roles = new List<Role>();
            if (rolesDataTable != null)
            {
                foreach (DataRow row in rolesDataTable.Rows)
                {
                    var role = new Role();
                    role.Id = row["roleid"] == DBNull.Value ? 0 : int.Parse(row["roleid"].ToString());
                    role.Name = row["rolename"] == DBNull.Value ? string.Empty : row["rolename"].ToString();
                    role.Code = row["rolecode"] == DBNull.Value ? string.Empty : row["rolecode"].ToString();
                    role.Weight = row["weight"] == DBNull.Value ? 0 : int.Parse(row["weight"].ToString());
                    role.Order = row["roleorder"] == DBNull.Value ? byte.MinValue : byte.Parse(row["roleorder"].ToString());
                    role.IsActive = row["isactive"] == DBNull.Value ? false : row["isactive"].ToString() == "1" ? true : false;
                    roles.Add(role);
                }
            }
            return roles;
        }

        public int GetPartnerCount(int roleId)
        {
            var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "Role_Id", OracleDbType = OracleDbType.Int32,  Value = roleId },
            };
            var roleDataTable = this.db.GetData("Select nvl(count(*), 0) cnt from partner where roleid = :Role_Id", parameters);
            int count = 0;
            if (roleDataTable != null)
            {
                DataRow row = roleDataTable.Rows[0];
                count = row["cnt"] == DBNull.Value ? 0 : int.Parse(row["cnt"].ToString());
            }
            return count;
        }
        public List<Role> GetAuthorizedRoles(string actId,int roleId)
        {
            var permission = partnerActivityRepo.GetPartAct(actId, roleId);
            if (permission == null) return null;
            if (permission.Details == null) return null;
            var roles = new List<Role>();
            foreach (var item in permission.Details)
            {
                var role = GetRole(item.ToRole.Id);
                roles.Add(role);
            }
            return roles;
        }
        public Role GetRole(int id)
        {
            var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "roleId", OracleDbType = OracleDbType.Int32,  Value = id },
            };
            var roleDataTable = this.db.GetData("Select * from AppRole where roleid = :roleId", parameters);
            var role = new Role();
            if (roleDataTable != null)
            {
               DataRow row = roleDataTable.Rows[0];
               role.Id = row["roleid"] == DBNull.Value ? 0 : int.Parse(row["roleid"].ToString());
               role.Name = row["rolename"] == DBNull.Value ? string.Empty : row["rolename"].ToString();
                role.Code = row["rolecode"] == DBNull.Value ? string.Empty : row["rolecode"].ToString();
                role.Weight = row["weight"] == DBNull.Value ? 0 : int.Parse(row["weight"].ToString());
               role.Order = row["roleorder"] == DBNull.Value ? byte.MinValue : byte.Parse(row["roleorder"].ToString());
            }
            return role;
        }

        
    }
}
