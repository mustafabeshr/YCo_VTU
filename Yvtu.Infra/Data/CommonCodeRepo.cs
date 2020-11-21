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
    public class CommonCodeRepo
    {
        private readonly IAppDbContext db;

        public CommonCodeRepo(IAppDbContext db)
        {
            this.db = db;
        }

        public List<CommonCode> GetCodesByType(string type)
        {
            var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "typeId", OracleDbType = OracleDbType.Varchar2,  Value = type },
            };
            var codeDataTable = this.db.GetData("Select * from Common_Code where code_type = :typeId order by code_order", parameters);

            var commonCodes = new List<CommonCode>();
            if (codeDataTable != null)
            {
                foreach (DataRow row in codeDataTable.Rows)
                {
                    var commonCode = new CommonCode();
                    commonCode.Id = row["code_id"] == DBNull.Value ? string.Empty : row["code_id"].ToString();
                    commonCode.Name = row["code_name"] == DBNull.Value ? string.Empty : row["code_name"].ToString();
                    commonCode.Type = row["code_type"] == DBNull.Value ? string.Empty : row["code_type"].ToString();
                    commonCode.Order = row["code_order"] == DBNull.Value ? 0 : int.Parse(row["code_order"].ToString());
                    commonCodes.Add(commonCode);
                }
            }
            return commonCodes;
        }
        public CommonCode GetCodesById(string id)
        {
            var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "CodeId", OracleDbType = OracleDbType.Varchar2,  Value = id },
            };
            var codeDataTable = this.db.GetData("Select * from Common_Code where code_id = :CodeId", parameters);
            var commonCode = new CommonCode();
            if (codeDataTable != null && codeDataTable.Rows.Count > 0)
            {
                DataRow row = codeDataTable.Rows[0];
                commonCode.Id = row["code_id"] == DBNull.Value ? string.Empty : row["code_id"].ToString();
                commonCode.Name = row["code_name"] == DBNull.Value ? string.Empty : row["code_name"].ToString();
                commonCode.Type = row["code_type"] == DBNull.Value ? string.Empty : row["code_type"].ToString();
                commonCode.Order = row["code_order"] == DBNull.Value ? 0 : int.Parse(row["code_order"].ToString());
            }
            return commonCode;
        }
    }
}
