using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.Infra.Data
{
    public class NotificationRepo
    {
        private readonly IAppDbContext db;
        private readonly IPartnerManager partnerManager;

        public NotificationRepo(IAppDbContext db, IPartnerManager partnerManager)
        {
            this.db = db;
            this.partnerManager = partnerManager;
        }
        public OpertionResult Create(SMSOut sms)
        {
            try
            {
                #region Parameters
                var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "retVal",OracleDbType = OracleDbType.Int32,  Direction = ParameterDirection.ReturnValue },
                 new OracleParameter{ ParameterName = "v_message",OracleDbType = OracleDbType.NVarchar2,  Value = sms.Message },
                 new OracleParameter{ ParameterName = "v_receiver",OracleDbType = OracleDbType.Varchar2,  Value = sms.Receiver },
                };
                #endregion
                db.ExecuteStoredProc("pk_infra.fn_createOutSMS", parameters);
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

        public List<Notification> GetList(string receiver)
        {
            var whereClause = new StringBuilder();
            #region Parameters
            var parameters = new List<OracleParameter>();
            if (!string.IsNullOrEmpty(receiver))
            {
                var activityIdParamenter = new OracleParameter { ParameterName = "receiver", OracleDbType = OracleDbType.Varchar2, Value = receiver };
                whereClause.Append(" WHERE partner_id = :receiver ");
                parameters.Add(activityIdParamenter);
            }
            whereClause.Append(!string.IsNullOrEmpty(whereClause.ToString()) ? " AND ROWNUM < 200 " : " WHERE ROWNUM < 200 ");
            #endregion
            var masterDataTable = db.GetData("Select * from NOTIFICATION t  " + whereClause + " order by nf_id desc", parameters);

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;

            var list = new List<Notification>();
            foreach (DataRow row in masterDataTable.Rows)
            {
                var obj = new Notification();
                obj.Id = row["nf_id"] == DBNull.Value ? 0 : int.Parse(row["nf_id"].ToString());
                obj.CreatedOn = row["createdon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["createdon"].ToString());
                var createdAccount = row["partner_acc"] == DBNull.Value ? -1 : int.Parse(row["partner_acc"].ToString());
                if (createdAccount > 0)
                {
                    var partner = partnerManager.GetPartnerByAccount(createdAccount);
                    obj.Partner.Id = partner.Id;
                    obj.Partner.Account = partner.Account;
                    obj.Partner.Name = partner.Name;
                }
                else
                {
                    obj.Partner.Id = row["partner_id"] == DBNull.Value ? string.Empty : row["partner_id"].ToString(); ;
                    obj.Partner.Account = createdAccount;
                    obj.Partner.Name = string.Empty;
                }
                obj.Message = row["notify_msg"] == DBNull.Value ? string.Empty : row["notify_msg"].ToString();
                obj.RefNo = row["ref_no"] == DBNull.Value ? -1 : int.Parse(row["ref_no"].ToString());
                var activityId = row["act_id"] == DBNull.Value ? string.Empty : row["act_id"].ToString();
                obj.Activity = new ActivityRepo(db, partnerManager).GetActivity(activityId, false);
                list.Add(obj);
            }
            return list;
        }
        public void SendNotification<T>(string activityId, int refNo, T data, int suppress = 0)
        {
            var activity = new ActivityRepo(db, partnerManager).GetActivity(activityId);
            var messages = new ActivityMessageRepo(db, partnerManager).GetList(activity.Id, -1);
            var toNumber = string.Empty;
            foreach (var m in messages)
            {   if (suppress > 0 && suppress == m.Message.ToWho) continue;
                if (m.Message.ToWho == 1) {
                    toNumber = new MessageTemplateRepo(db, partnerManager).TranslateMessage<T>("{mobile.1}", data);
                }else
                {
                    toNumber = new MessageTemplateRepo(db, partnerManager).TranslateMessage<T>("{mobile.2}", data);
                }
                var readyMsg = new MessageTemplateRepo(db, partnerManager).TranslateMessage<T>(m.Message.Message, data);

                if (!string.IsNullOrEmpty(toNumber) && !string.IsNullOrEmpty(readyMsg))
                {
                    var sms = new SMSOut();
                    sms.Receiver = toNumber;
                    sms.Message = readyMsg;
                    var result = Create(sms);
                }
            }
        }
    }
}
