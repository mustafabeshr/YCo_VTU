using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.Infra.Data
{
    public class MessageTemplateRepo
    {
        private readonly IAppDbContext db;
        private readonly IPartnerManager partnerManager;

        public MessageTemplateRepo(IAppDbContext db, IPartnerManager partnerManager)
        {
            this.db = db;
            this.partnerManager = partnerManager;
        }
        public OpertionResult Create(MessageTemplate template)
        {
            try
            {
                #region Parameters
                var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "retVal",OracleDbType = OracleDbType.Int32,  Direction = ParameterDirection.ReturnValue },
                 new OracleParameter{ ParameterName = "v_msg_name",OracleDbType = OracleDbType.Varchar2,  Value = template.Title },
                 new OracleParameter{ ParameterName = "v_msg_text",OracleDbType = OracleDbType.Varchar2,  Value = template.Message },
                 new OracleParameter{ ParameterName = "v_createdby",OracleDbType = OracleDbType.Varchar2,  Value = template.CreatedBy.Id },
                 new OracleParameter{ ParameterName = "v_createdbyacc",OracleDbType = OracleDbType.Int32,  Value = template.CreatedBy.Account },
                 new OracleParameter{ ParameterName = "v_towho",OracleDbType = OracleDbType.Int32,  Value = template.ToWho }
                };
                #endregion
                db.ExecuteStoredProc("pk_settings.fn_create_message_template", parameters);
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

        public OpertionResult Update(MessageTemplate template)
        {
            try
            {
                var old = GetSingle(template.Id);
                if (old == null) return new OpertionResult { AffectedCount = 0, Success = false, Error = "No Old Data" };
                if (old.Title == template.Title && old.Message == template.Message && old.ToWho == template.ToWho) 
                    return new OpertionResult { AffectedCount = 0, Success = false, Error = "Nothing to update" };
                #region Parameters
                var parameters = new List<OracleParameter> {
                     new OracleParameter{ ParameterName = "retVal",OracleDbType = OracleDbType.Int32,  Direction = ParameterDirection.ReturnValue },
                     new OracleParameter{ ParameterName = "v_msg_id",OracleDbType = OracleDbType.Varchar2,  Value = template.Id },
                     new OracleParameter{ ParameterName = "v_msg_name",OracleDbType = OracleDbType.Varchar2,  Value = template.Title },
                     new OracleParameter{ ParameterName = "v_msg_text",OracleDbType = OracleDbType.Varchar2,  Value = template.Message },
                     new OracleParameter{ ParameterName = "v_towho",OracleDbType = OracleDbType.Int32,  Value = template.ToWho }
                };
                #endregion
                db.ExecuteStoredProc("pk_settings.fn_update_message_template", parameters);
                var result = int.Parse(parameters.Find(x => x.ParameterName == "retVal").Value.ToString());

                if (result > 0)
                {
                    var audit = new DataAudit();
                    audit.Activity.Id = "MessageTemplate.Update";
                    audit.PartnerId = template.CreatedBy.Id;
                    audit.PartnerAccount = template.CreatedBy.Account;
                    audit.Action.Id = "Update";
                    audit.Success = true;
                    audit.OldValue = old.ToString();
                    audit.NewValue = template.ToString();
                    var auditResult = new DataAuditRepo(db).Create(audit);
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

        public List<MessageTemplate> GetAll(string NotInActivity = "")
        {
            DataTable masterDataTable;
            if (NotInActivity == "")
            {
                masterDataTable = db.GetData("Select * from message_template t  order by createdon", null);
            }else
            {
                masterDataTable = db.GetData("Select * from message_template t where not exists (select 1 from activity_message a where a.act_id='"+NotInActivity+"'"
                    + "and a.msg_id = t.msg_id) order by createdon", null);
            }

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;

            var results = new List<MessageTemplate>();
            foreach (DataRow row in masterDataTable.Rows)
            {
                var obj = new MessageTemplate();
                obj.Id = row["msg_id"] == DBNull.Value ? -1 : int.Parse(row["msg_id"].ToString());
                obj.ToWho = row["towho"] == DBNull.Value ? -1 : int.Parse(row["towho"].ToString());
                obj.Title = row["msg_name"] == DBNull.Value ? string.Empty : row["msg_name"].ToString();
                obj.Message = row["msg_text"] == DBNull.Value ? string.Empty : row["msg_text"].ToString();
                obj.CreatedOn = row["createdon"] == DBNull.Value ? DateTime.MinValue :DateTime.Parse(row["createdon"].ToString());
                obj.LastUpdatedOn = row["lastupdateon"] == DBNull.Value ? DateTime.MinValue :DateTime.Parse(row["lastupdateon"].ToString());
                var createdAccount = row["createdbyacc"] == DBNull.Value ? -1 : int.Parse(row["createdbyacc"].ToString());
                var partner = partnerManager.GetPartnerByAccount(createdAccount);
                obj.CreatedBy.Id = row["createdby"] == DBNull.Value ? string.Empty : row["createdby"].ToString();
                obj.CreatedBy.Name = partner.Name;
                obj.CreatedBy.Account = partner.Account;
                results.Add(obj);
            }
            return results;
        }

        public List<MessageTemplate> GetRelatedToActivity(string actId)
        {
            #region Parameters
            var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "actId",OracleDbType = OracleDbType.Varchar2,  Value = actId }
                };
            #endregion
            DataTable masterDataTable;
            masterDataTable = db.GetData("select * from message_template where exists (select 1 from activity_message" +
                "  where activity_message.msg_id = message_template.msg_id and activity_message.act_id = :actId )  order by createdon", parameters);

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;

            var results = new List<MessageTemplate>();
            foreach (DataRow row in masterDataTable.Rows)
            {
                var obj = new MessageTemplate();
                obj.Id = row["msg_id"] == DBNull.Value ? -1 : int.Parse(row["msg_id"].ToString());
                obj.ToWho = row["towho"] == DBNull.Value ? -1 : int.Parse(row["towho"].ToString());
                obj.Title = row["msg_name"] == DBNull.Value ? string.Empty : row["msg_name"].ToString();
                obj.Message = row["msg_text"] == DBNull.Value ? string.Empty : row["msg_text"].ToString();
                obj.CreatedOn = row["createdon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["createdon"].ToString());
                obj.LastUpdatedOn = row["lastupdateon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["lastupdateon"].ToString());
                var createdAccount = row["createdbyacc"] == DBNull.Value ? -1 : int.Parse(row["createdbyacc"].ToString());
                var partner = partnerManager.GetPartnerByAccount(createdAccount);
                obj.CreatedBy.Id = partner.Id;
                obj.CreatedBy.Name = partner.Name;
                obj.CreatedBy.Account = partner.Account;
                results.Add(obj);
            }
            return results;
        }

        public MessageTemplate GetSingle(int id)
        {
            #region Parameters
            var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "MsgId",OracleDbType = OracleDbType.Int32,  Value = id }
                };
            #endregion
            var masterDataTable = db.GetData("Select * from message_template t  Where msg_id =:MsgId", parameters);

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;

                  DataRow row = masterDataTable.Rows[0];
                var obj = new MessageTemplate();
                obj.Id = row["msg_id"] == DBNull.Value ? -1 : int.Parse(row["msg_id"].ToString());
                obj.ToWho = row["towho"] == DBNull.Value ? -1 : int.Parse(row["towho"].ToString());
                obj.Title = row["msg_name"] == DBNull.Value ? string.Empty : row["msg_name"].ToString();
                obj.Message = row["msg_text"] == DBNull.Value ? string.Empty : row["msg_text"].ToString();
                obj.CreatedOn = row["createdon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["createdon"].ToString());
                obj.LastUpdatedOn = row["lastupdateon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["lastupdateon"].ToString());
                var createdAccount = row["createdbyacc"] == DBNull.Value ? -1 : int.Parse(row["createdbyacc"].ToString());
                var partner = partnerManager.GetPartnerByAccount(createdAccount);
                obj.CreatedBy.Id = row["createdby"] == DBNull.Value ? string.Empty : row["createdby"].ToString();
                obj.CreatedBy.Name = partner.Name;
                obj.CreatedBy.Account = partner.Account;
               
            return obj;
        }

        public MessageTemplate GetByExactTitle(string title)
        {
            #region Parameters
            var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "MsgTitle",OracleDbType = OracleDbType.Varchar2,  Value = title }
                };
            #endregion
            var masterDataTable = db.GetData("Select * from message_template t  Where msg_name =:MsgTitle", parameters);

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;

            DataRow row = masterDataTable.Rows[0];
            var obj = new MessageTemplate();
            obj.Id = row["msg_id"] == DBNull.Value ? -1 : int.Parse(row["msg_id"].ToString());
            obj.ToWho = row["towho"] == DBNull.Value ? -1 : int.Parse(row["towho"].ToString());
            obj.Title = row["msg_name"] == DBNull.Value ? string.Empty : row["msg_name"].ToString();
            obj.Message = row["msg_text"] == DBNull.Value ? string.Empty : row["msg_text"].ToString();
            obj.CreatedOn = row["createdon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["createdon"].ToString());
            obj.LastUpdatedOn = row["lastupdateon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["lastupdateon"].ToString());
            var createdAccount = row["createdbyacc"] == DBNull.Value ? -1 : int.Parse(row["createdbyacc"].ToString());
            var partner = partnerManager.GetPartnerByAccount(createdAccount);
            obj.CreatedBy.Id = row["createdby"] == DBNull.Value ? string.Empty : row["createdby"].ToString();
            obj.CreatedBy.Name = partner.Name;
            obj.CreatedBy.Account = partner.Account;

            return obj;
        }
        public List<MessageTemplate> GetByPartTitle(string title, bool relatedActivities = false)
        {
            #region Parameters
            var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "MsgTitle",OracleDbType = OracleDbType.Varchar2,  Value = title }
                };
            #endregion
            var masterDataTable = db.GetData("Select * from message_template t  Where (msg_name LIKE '%' ||  :MsgTitle || '%') or (msg_text LIKE '%' ||  :MsgTitle || '%') ", parameters);

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;
            var results = new List<MessageTemplate>();
            foreach (DataRow row in masterDataTable.Rows)
            {
                var obj = new MessageTemplate();
                obj.Id = row["msg_id"] == DBNull.Value ? -1 : int.Parse(row["msg_id"].ToString());
                obj.ToWho = row["towho"] == DBNull.Value ? -1 : int.Parse(row["towho"].ToString());
                obj.Title = row["msg_name"] == DBNull.Value ? string.Empty : row["msg_name"].ToString();
                obj.Message = row["msg_text"] == DBNull.Value ? string.Empty : row["msg_text"].ToString();
                obj.CreatedOn = row["createdon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["createdon"].ToString());
                obj.LastUpdatedOn = row["lastupdateon"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(row["lastupdateon"].ToString());
                var createdAccount = row["createdbyacc"] == DBNull.Value ? -1 : int.Parse(row["createdbyacc"].ToString());
                var partner = partnerManager.GetPartnerByAccount(createdAccount);
                obj.CreatedBy.Id = row["createdby"] == DBNull.Value ? string.Empty : row["createdby"].ToString();
                obj.CreatedBy.Name = partner.Name;
                obj.CreatedBy.Account = partner.Account;
                //  get activities 
                if (relatedActivities)
                {
                    var activities = new ActivityMessageRepo(db, partnerManager).GetList(string.Empty, obj.Id);
                    if (activities != null)
                    {
                        foreach (var item in activities)
                        {
                            obj.Activities.Add(item.Activity);
                        }
                    }
                }
                //--------------------------------------------------
                results.Add(obj);
            }
            return results;
        }

        public List<MessageDictionary> GetDictionaryAll()
        {
            
            var masterDataTable = db.GetData("Select * from MSG_DICTIONARY t  order by word_order", null);

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;

            var results = new List<MessageDictionary>();
            foreach (DataRow row in masterDataTable.Rows)
            {
                var obj = new MessageDictionary();
                obj.Order = row["word_order"] == DBNull.Value ? -1 : int.Parse(row["word_order"].ToString());
                obj.Id = row["word_id"] == DBNull.Value ? string.Empty : row["word_id"].ToString();
                obj.Name = row["word_name"] == DBNull.Value ? string.Empty : row["word_name"].ToString();
                obj.ObjectMember = row["obj_member"] == DBNull.Value ? null : row["obj_member"].ToString().Split('|');
                results.Add(obj);
            }
            return results;
        }

        public MessageDictionary GetDictionarySingle(string id)
        {
            #region Parameters
            var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "MsgId",OracleDbType = OracleDbType.Varchar2,  Value = id }
                };
            #endregion

            var masterDataTable = db.GetData("Select * from MSG_DICTIONARY t where word_id = :MsgId", parameters);

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;

            DataRow row = masterDataTable.Rows[0];
                var obj = new MessageDictionary();
                obj.Order = row["word_order"] == DBNull.Value ? -1 : int.Parse(row["word_order"].ToString());
                obj.Id = row["word_id"] == DBNull.Value ? string.Empty : row["word_id"].ToString();
                obj.Name = row["word_name"] == DBNull.Value ? string.Empty : row["word_name"].ToString();
                obj.ObjectMember = row["obj_member"] == DBNull.Value ? null : row["obj_member"].ToString().Split('|');
            return obj;
        }

        public bool RemoveMessage(int id)
        {
            var parameters = new List<OracleParameter> {
                 new OracleParameter{ ParameterName = "MsgId", OracleDbType = OracleDbType.Int32,  Value = id }
            };
            return db.ExecuteSqlCommand("Delete from message_template Where msg_id=:MsgId", parameters) > 0;
        }

        public string TranslateMessage<T>(string rawMessage, T obj)
        {
            if (obj == null) return string.Empty;
            var wordsList = ExtractDicWord(rawMessage);
            var nextWord = false;
            if (wordsList != null && wordsList.Count > 0)
            {
                foreach (var word in wordsList)
                {
                    nextWord = false;
                    var members = GetDictionarySingle(word);
                    if (members != null && members.ObjectMember != null && members.ObjectMember.Length > 0)
                    {
                        foreach (var member in members.ObjectMember)
                        {
                            if (nextWord) break;
                            foreach (var prop in obj.GetType().GetProperties())
                            {
                                if (nextWord) break;
                                if (prop.Name == member)
                                {
                                    var value = prop.GetValue(obj).ToString();
                                    rawMessage = rawMessage.Replace(word, value);
                                    nextWord = true;
                                }
                            }
                        }
                        if (!nextWord)
                        {
                            rawMessage = rawMessage.Replace(word, " ");
                        }
                    } 
                }
            }
            return rawMessage;
        }

        private List<string> ExtractDicWord(string input)
        {
            var start = false;
            var length = input.Length;
            var words = new List<string>();
            var currWord = string.Empty;
            for(int i=0; i<length; i++)
            {
                var chr = input[i];
                if (chr == '}')
                {
                    currWord += chr;
                    words.Add(currWord);
                    currWord = string.Empty;
                    start = false;
                }else  if (start)
                {
                    currWord += chr;
                }
                else if (chr == '{')
                {
                    currWord += chr;
                    start = true;
                }
            }

            return words;
        }
        
    }
}
