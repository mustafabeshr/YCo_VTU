using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.Infra.Data
{
    public class AppGlobalSettingsRepo
    {
        private readonly IAppDbContext db;

        public AppGlobalSettingsRepo(IAppDbContext db)
        {
            this.db = db;
        }
        public AppGlobalSettings GetSingle(string code)
        {
            var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "SettCode", OracleDbType = OracleDbType.Varchar2,  Value = code },
            };
            var globalDataTable = this.db.GetData("Select * from GLOGALSETTINGS where sett_code = :SettCode", parameters);
            var globalSetting = new AppGlobalSettings();
            if (globalDataTable != null)
            {
                DataRow row = globalDataTable.Rows[0];
                globalSetting.SettingCode = row["sett_code"] == DBNull.Value ? string.Empty : row["sett_code"].ToString();
                globalSetting.SettingName = row["sett_name"] == DBNull.Value ? string.Empty : row["sett_name"].ToString();
                globalSetting.SettingValue = row["sett_value"] == DBNull.Value ? string.Empty : row["sett_value"].ToString();
                globalSetting.SettingOrder = row["sett_order"] == DBNull.Value ? int.MinValue : int.Parse(row["sett_order"].ToString());
            }
            return globalSetting;
        }

        public List<AppGlobalSettings> GetAll()
        {
            var globalDataTable = this.db.GetData("Select * from GLOGALSETTINGS  order by sett_order", null);

            var settings = new List<AppGlobalSettings>();
            if (globalDataTable != null)
            {
                foreach (DataRow row in globalDataTable.Rows)
                {
                    var globalSetting = new AppGlobalSettings();
                    globalSetting.SettingCode = row["sett_code"] == DBNull.Value ? string.Empty : row["sett_code"].ToString();
                    globalSetting.SettingName = row["sett_name"] == DBNull.Value ? string.Empty : row["sett_name"].ToString();
                    globalSetting.SettingValue = row["sett_value"] == DBNull.Value ? string.Empty : row["sett_value"].ToString();
                    globalSetting.SettingOrder = row["sett_order"] == DBNull.Value ? int.MinValue : int.Parse(row["sett_order"].ToString());
                    settings.Add(globalSetting);
                }
            }
            return settings;
        }
    }
}
