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
                    role.Id = row["roleid"] == null ? 0 : int.Parse(row["roleid"].ToString());
                    role.Name = row["rolename"] == null ? string.Empty : row["rolename"].ToString();
                    role.Weight = row["weight"] == null ? 0 : int.Parse(row["weight"].ToString());
                    role.Order = row["roleorder"] == null ? byte.MinValue : byte.Parse(row["roleorder"].ToString());
                    roles.Add(role);
                }
            }
            return roles;
        }
    }
}
