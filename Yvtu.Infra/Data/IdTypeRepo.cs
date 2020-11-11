using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.Infra.Data
{
    public class IdTypeRepo
    {
        private readonly IAppDbContext db;

        public IdTypeRepo(IAppDbContext db)
        {
            this.db = db;
        }

        public List<IdType> GetTypes()
        {
            var idTypeDataTable = this.db.GetData("Select * from idtypes where isactive = 1 order by 1", null);

            var idTypes = new List<IdType>();
            if (idTypeDataTable != null)
            {
                foreach (DataRow row in idTypeDataTable.Rows)
                {
                    var idType = new IdType();
                    idType.Id = row["idtypeid"] == null ? 0 : int.Parse(row["idtypeid"].ToString());
                    idType.Name = row["idtypename"] == null ? string.Empty : row["idtypename"].ToString();
                    idTypes.Add(idType);
                }
            }
            return idTypes;
        }
    }
}
