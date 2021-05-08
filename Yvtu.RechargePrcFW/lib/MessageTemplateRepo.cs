using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yvtu.RechargePrcFW.lib
{
    public class MessageTemplateRepo
    {
        public List<Entities.MessageTemplate> GetMessages(string actId)
        {
            var strSql =
                $"select mt.msg_text, mt.towho from ACTIVITY_MESSAGE am, MESSAGE_TEMPLATE mt where am.msg_id = mt.msg_id " +
                $" and am.act_id = '{actId}' order by am.msg_order";
            var masterDataTable = DB.GetDataTable(strSql, null);

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;
            var msgList = new List<Entities.MessageTemplate>();
            foreach (DataRow row in masterDataTable.Rows)
            {
                var obj = new Entities.MessageTemplate();
                obj.ToWho = row["towho"] == DBNull.Value ? -1 : int.Parse(row["towho"].ToString());
                obj.Message = row["msg_text"] == DBNull.Value ? string.Empty : row["msg_text"].ToString();
                msgList.Add(obj);
            }
            return msgList;
        }
    }
}
