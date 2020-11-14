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

        public RoleRepo(IAppDbContext db)
        {
            this.db = db;
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
                    roles.Add(role);
                }
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
