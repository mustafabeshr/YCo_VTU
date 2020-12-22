using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.Infra.Data
{
    public class ActivityMessageRepo
    {
        private readonly IAppDbContext db;
        private readonly IPartnerManager partnerManager;

        public ActivityMessageRepo(IAppDbContext db, IPartnerManager partnerManager)
        {
            this.db = db;
            this.partnerManager = partnerManager;
        }

        public OpertionResult Create(ActivityMessage msg)
        {
            try
            {
                #region Parameters
                var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "retVal",OracleDbType = OracleDbType.Int32,  Direction = ParameterDirection.ReturnValue },
                 new OracleParameter{ ParameterName = "v_act_id",OracleDbType = OracleDbType.Varchar2,  Value = msg.Activity.Id },
                 new OracleParameter{ ParameterName = "v_msg_id",OracleDbType = OracleDbType.Int32,  Value = msg.Message.Id },
                 new OracleParameter{ ParameterName = "v_sending_time",OracleDbType = OracleDbType.Varchar2,  Value = msg.SendingTime.Id},
                 new OracleParameter{ ParameterName = "v_createdby",OracleDbType = OracleDbType.Varchar2,  Value = msg.CreatedBy.Id },
                 new OracleParameter{ ParameterName = "v_createdbyacc",OracleDbType = OracleDbType.Int32,  Value = msg.CreatedBy.Account },
                 new OracleParameter{ ParameterName = "v_msg_order",OracleDbType = OracleDbType.Int32,  Value = msg.MessageOrder }
                };
                #endregion
                db.ExecuteStoredProc("pk_settings.fn_create_activity_message", parameters);
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

        public bool RemoveMessage(string actId, int msgId)
        {
            var parameters = new List<OracleParameter> {
                  new OracleParameter{ ParameterName = "retVal",OracleDbType = OracleDbType.Int32,  Direction = ParameterDirection.ReturnValue },
                 new OracleParameter{ ParameterName = "v_act_id", OracleDbType = OracleDbType.Varchar2,  Value = actId },
                 new OracleParameter{ ParameterName = "v_msg_id", OracleDbType = OracleDbType.Int32,  Value = msgId }
            };
            db.ExecuteStoredProc("pk_settings.fn_delete_activity_message", parameters);
            var result = int.Parse(parameters.Find(x => x.ParameterName == "retVal").Value.ToString());
            return result > 0;
        }

        public List<ActivityMessage> GetList(string activityId, int messageId)
        {
            string whereClause = string.Empty;
            #region Parameters
            var parameters = new List<OracleParameter>();
            if (!string.IsNullOrEmpty(activityId))
            {
               var activityIdParamenter = new OracleParameter { ParameterName = "activityId", OracleDbType = OracleDbType.Varchar2, Value = activityId };
               whereClause = " WHERE act_id=:activityId ";
               parameters.Add(activityIdParamenter);
            }
            if (messageId > 0)
            {
                var messageIdParamenter = new OracleParameter { ParameterName = "messageId", OracleDbType = OracleDbType.Varchar2, Value = messageId };
                whereClause += string.IsNullOrEmpty(whereClause) ? " WHERE msg_id=:messageId " : " AND msg_id=:messageId ";
                parameters.Add(messageIdParamenter);
            }

            #endregion
            var masterDataTable = db.GetData("Select * from activity_message t  " + whereClause + "order by msg_order" +
                "", parameters);

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;

            var list = new List<ActivityMessage>();
            foreach (DataRow row in masterDataTable.Rows)
            {
                var obj = new ActivityMessage();
                var actId = row["act_id"] == DBNull.Value ? string.Empty : row["act_id"].ToString();
                obj.Activity = new ActivityRepo(db, partnerManager).GetActivity(actId);
                var msgId = row["msg_id"] == DBNull.Value ? 0 : int.Parse(row["msg_id"].ToString());
                obj.MessageOrder = row["msg_order"] == DBNull.Value ? 0 : int.Parse(row["msg_order"].ToString());
                obj.Message = new MessageTemplateRepo(db, partnerManager).GetSingle(msgId);
                var sendingTime = row["sending_time"] == DBNull.Value ? string.Empty : row["sending_time"].ToString();
                obj.SendingTime = new CommonCodeRepo(db).GetCodesById(sendingTime, "ActivityMessage.SendingTime");
                obj.CreatedOn = row["createdon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["createdon"].ToString());
                var createdAccount = row["createdbyacc"] == DBNull.Value ? -1 : int.Parse(row["createdbyacc"].ToString());
                var partner = partnerManager.GetPartnerByAccount(createdAccount);
                obj.CreatedBy.Id = row["createdby"] == DBNull.Value ? string.Empty : row["createdby"].ToString();
                obj.CreatedBy.Name = partner.Name;
                obj.CreatedBy.Account = partner.Account;
                list.Add(obj);
            }
            return list;
        }

        public ActivityMessage GetSingle(string activityId, int messageId)
        {
            string whereClause = string.Empty;
            #region Parameters
            var parameters = new List<OracleParameter>();
            if (string.IsNullOrEmpty(activityId)) return null;
            var activityIdParamenter = new OracleParameter { ParameterName = "activityId", OracleDbType = OracleDbType.Varchar2, Value = activityId };
            whereClause = " WHERE act_id=:activityId ";
            parameters.Add(activityIdParamenter);

            if (messageId <= 0) return null;
            var messageIdParamenter = new OracleParameter { ParameterName = "messageId", OracleDbType = OracleDbType.Varchar2, Value = messageId };
            whereClause += string.IsNullOrEmpty(whereClause) ? " WHERE msg_id=:messageId " : " AND msg_id=:messageId ";
            parameters.Add(messageIdParamenter);

            #endregion
            var masterDataTable = db.GetData("Select * from activity_message t  " + whereClause + " order by msg_order", parameters);

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;

            DataRow row = masterDataTable.Rows[0];
            var obj = new ActivityMessage();
            var actId = row["act_id"] == DBNull.Value ? string.Empty : row["act_id"].ToString();
            obj.Activity = new ActivityRepo(db, partnerManager).GetActivity(actId);
            var msgId = row["msg_id"] == DBNull.Value ? 0 : int.Parse(row["msg_id"].ToString());
            obj.MessageOrder = row["msg_order"] == DBNull.Value ? 0 : int.Parse(row["msg_order"].ToString());
            obj.Message = new MessageTemplateRepo(db, partnerManager).GetSingle(msgId);
            var sendingTime = row["sending_time"] == DBNull.Value ? string.Empty : row["sending_time"].ToString();
            obj.SendingTime = new CommonCodeRepo(db).GetCodesById(sendingTime, "ActivityMessage.SendingTime");
            obj.CreatedOn = row["createdon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["createdon"].ToString());
            var createdAccount = row["createdbyacc"] == DBNull.Value ? -1 : int.Parse(row["createdbyacc"].ToString());
            var partner = partnerManager.GetPartnerByAccount(createdAccount);
            obj.CreatedBy.Id = row["createdby"] == DBNull.Value ? string.Empty : row["createdby"].ToString();
            obj.CreatedBy.Name = partner.Name;
            obj.CreatedBy.Account = partner.Account;
            return obj;
        }

        public bool UpdateOrder(string actId, int msgId, int newOrder)
        {
            var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "actId", OracleDbType = OracleDbType.Varchar2,  Value = actId },
                 new OracleParameter{ ParameterName = "msgId", OracleDbType = OracleDbType.Int32,  Value = msgId }
                 //new OracleParameter{ ParameterName = "MOrder", OracleDbType = OracleDbType.Int32,  Value = newOrder }
            };
            var result = db.ExecuteSqlCommand("UPDATE ACTIVITY_MESSAGE SET msg_order="+ newOrder + " WHERE act_id=:actId AND msg_id=:msgId", parameters);
            return result > 0;
        }
    }
}
