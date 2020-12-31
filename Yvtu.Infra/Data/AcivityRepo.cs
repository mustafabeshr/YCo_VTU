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
        private readonly IPartnerManager partnerManager;

        public ActivityRepo(IAppDbContext db, IPartnerManager partnerManager)
        {
            this.db = db;
            this.partnerManager = partnerManager;
        }

        public List<Activity> GetActivities(bool withInternals = true, bool relatedMessages = false)
        {
            var actDataTable = this.db.GetData("Select * from activities "+ (!withInternals ? " WHERE internal_use=0 " : string.Empty) +" order by  act_order", null);

            var activities = new List<Activity>();
            if (actDataTable != null)
            {
                foreach (DataRow row in actDataTable.Rows)
                {
                    var activity = ConvertDataRowToActivity(row);
                    if (relatedMessages)
                    {
                        activity.Messages = new MessageTemplateRepo(db, null).GetRelatedToActivity(activity.Id);
                    }
                    activities.Add(activity);
                }
            }
            return activities;
        }

        public List<Activity> GetActivities(string actName, bool relatedMessages = false)
        {
            var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "actName", OracleDbType = OracleDbType.Varchar2,  Value = actName },
            };

            var actDataTable = this.db.GetData("Select * from activities WHERE act_name LIKE  '%' ||  :actName || '%' AND internal_use = 0 order by  act_order", parameters);

            var activities = new List<Activity>();
            if (actDataTable != null)
            {
                foreach (DataRow row in actDataTable.Rows)
                {
                    var activity = ConvertDataRowToActivity(row);
                    if (relatedMessages)
                    {
                        activity.Messages = new MessageTemplateRepo(db, partnerManager).GetRelatedToActivity(activity.Id);
                    }
                    activities.Add(activity);
                }
            }
            return activities;
        }

        public Activity GetActivity(string id, bool relatedMessages = false)
        {
            var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "actId", OracleDbType = OracleDbType.Varchar2,  Value = id },
            };
            var actDataTable = this.db.GetData("Select * from activities where act_id = :actId", parameters);
            var activity = new Activity();
            if (actDataTable != null)
            {
                activity =  ConvertDataRowToActivity(actDataTable.Rows[0]);
                if (relatedMessages)
                {
                    activity.Messages = new MessageTemplateRepo(db, null).GetRelatedToActivity(activity.Id);
                }
            }
            return activity;
        }
        private Activity ConvertDataRowToActivity(DataRow row)
        {
            var activity = new Activity();
            activity.Id = row["act_id"] == DBNull.Value ? string.Empty : row["act_id"].ToString();
            activity.Name = row["act_name"] == DBNull.Value ? string.Empty : row["act_name"].ToString();
            activity.Description = row["act_desc"] == DBNull.Value ? string.Empty : row["act_desc"].ToString();
            activity.Type = row["act_type"] == DBNull.Value ? string.Empty : row["act_type"].ToString();
            activity.Order = row["act_order"] == DBNull.Value ? int.MinValue : int.Parse(row["act_order"].ToString());
            activity.Internal = row["internal_use"] == DBNull.Value ? false : row["internal_use"].ToString() == "1" ? true : false;
            return activity;
        }

        public List<Activity> GetDataAuditActivities()
        {
            var actDataTable = this.db.GetData("Select * from activities  a where exists (select 1 from DATA_AUDIT t where t.act_id = a.act_id) order by act_order", null);

            var activities = new List<Activity>();
            if (actDataTable != null)
            {
                foreach (DataRow row in actDataTable.Rows)
                {
                    var activity = ConvertDataRowToActivity(row);
                    activities.Add(activity);
                }
            }
            return activities;
        }
    }
}
