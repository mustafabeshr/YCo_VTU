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
    public class ActivityRepo
    {
        private readonly IAppDbContext db;

        public ActivityRepo(IAppDbContext db)
        {
            this.db = db;
        }

        public List<Activity> GetActivities()
        {
            var actDataTable = this.db.GetData("Select * from activities  order by act_type, act_order", null);

            var activities = new List<Activity>();
            if (actDataTable != null)
            {
                foreach (DataRow row in actDataTable.Rows)
                {
                    var activity = new Activity();
                    activity.Id = row["act_id"] == DBNull.Value ? string.Empty : row["act_id"].ToString();
                    activity.Name = row["act_name"] == DBNull.Value ? string.Empty : row["act_name"].ToString();
                    activity.Type = row["act_type"] == DBNull.Value ? string.Empty : row["act_type"].ToString();
                    activity.Order = row["act_order"] == DBNull.Value ? int.MinValue : int.Parse(row["act_order"].ToString());
                    activity.Internal = row["internal_use"] == DBNull.Value ? false : row["internal_use"].ToString() == "1" ? true : false;
                    activities.Add(activity);
                }
            }
            return activities;
        }

        public Activity GetActivity(string id)
        {
            var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "actId", OracleDbType = OracleDbType.Varchar2,  Value = id },
            };
            var actDataTable = this.db.GetData("Select * from activities where act_id = :actId", parameters);
            var activity = new Activity();
            if (actDataTable != null)
            {
                DataRow row = actDataTable.Rows[0];
                activity.Id = row["act_id"] == DBNull.Value ? string.Empty : row["act_id"].ToString();
                activity.Name = row["act_name"] == DBNull.Value ? string.Empty : row["act_name"].ToString();
                activity.Type = row["act_type"] == DBNull.Value ? string.Empty : row["act_type"].ToString();
                activity.Order = row["act_order"] == DBNull.Value ? int.MinValue : int.Parse(row["act_order"].ToString());
                activity.Internal = row["internal_use"] == DBNull.Value ? false : row["internal_use"].ToString() == "1" ? true : false;
            }
            return activity;
        }
    }
}
