using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Yvtu.Core.rpt;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.Infra.Data
{
    public class CollectionRepo
    {
        private readonly IAppDbContext db;

        public CollectionRepo(IAppDbContext db)
        {
            this.db = db;
        }

        public List<CollectionRpt> GetStatReport(CollectionRptQueryParam param)
        {
            #region Parameters
            param.Title = "تقرير احصائي يوضح اجمالي التحصيلات";
            var parameters = new List<OracleParameter>();
            var whereCluase = new StringBuilder();
            if (param != null)
            {
                if (!string.IsNullOrEmpty(param.PosId))
                {
                    whereCluase.Append(" WHERE pos_id = :PosId");
                    var p = new OracleParameter { ParameterName = "PosId", OracleDbType = OracleDbType.Varchar2, Value = param.PosId };
                    parameters.Add(p);
                    param.Title += $"{Environment.NewLine} للرقم {param.PosId} ";
                }
                if (!string.IsNullOrEmpty(param.SubsNo))
                {
                    whereCluase.Append(whereCluase.Length > 0 ? " AND subs_no = :SubsNo" : " WHERE subs_no = :SubsNo");
                    var p = new OracleParameter { ParameterName = "SubsNo", OracleDbType = OracleDbType.Varchar2, Value = param.SubsNo };
                    parameters.Add(p);
                    param.Title += $"{Environment.NewLine} للمشترك {param.SubsNo} ";
                }
                if (!string.IsNullOrEmpty(param.ChannelId) && param.ChannelId != "-1")
                {
                    whereCluase.Append(whereCluase.Length > 0 ? " AND access_channel = :ChannelId" : " WHERE access_channel = :ChannelId");
                    var p = new OracleParameter { ParameterName = "ChannelId", OracleDbType = OracleDbType.Varchar2, Value = param.ChannelId };
                    parameters.Add(p);
                }
                if (!string.IsNullOrEmpty(param.StatusId) && param.StatusId != "-1")
                {
                    whereCluase.Append(whereCluase.Length > 0 ? " AND status = :StatusId" : " WHERE status = :StatusId");
                    var p = new OracleParameter { ParameterName = "StatusId", OracleDbType = OracleDbType.Varchar2, Value = param.StatusId };
                    parameters.Add(p);
                }
                if (param.StartDate > DateTime.MinValue && param.StartDate != null)
                {
                    whereCluase.Append(whereCluase.Length > 0 ? " AND createdon >= :StartDate" : " WHERE createdon >= :StartDate");
                    var p = new OracleParameter { ParameterName = "StartDate", OracleDbType = OracleDbType.Date, Value = param.StartDate };
                    parameters.Add(p);
                }
                if (param.EndDate > DateTime.MinValue && param.EndDate != null)
                {
                    whereCluase.Append(whereCluase.Length > 0 ? " AND createdon <= :EndDate" : " WHERE createdon <= :EndDate");
                    var p = new OracleParameter { ParameterName = "EndDate", OracleDbType = OracleDbType.Date, Value = param.EndDate.AddDays(1) };
                    parameters.Add(p);
                }
            }

            #endregion

            string strSql = string.Empty;
            if (param.LevelId == "pos")
            {
                param.Title += $"{Environment.NewLine}  للفترة من {param.StartDate.ToShortDateString()} الى {param.EndDate.ToShortDateString()}  " + "على مستوى نقطة البيع";
                strSql = $"select  t.access_channel,t.access_channel_name, t.pos_id, t.pos_name,status, count(*) cnt, " +
                    $"sum(t.amount) amt,count(distinct t.pos_id) dist_cnt   from V_COLLECTION t {whereCluase}" +
                    $" group by t.access_channel, t.pos_id, t.pos_name,t.access_channel_name,status ";
            }
            else
            {
                param.Title += $"{Environment.NewLine}  للفترة من {param.StartDate.ToShortDateString()} الى {param.EndDate.ToShortDateString()}  " + "على مستوى اليوم";
                strSql = $"select  t.access_channel,t.access_channel_name, to_char(createdon,'yyyy/mm/dd') d, count(*) cnt, sum(t.amount) amt,count(distinct t.pos_id) dist_cnt " +
                    $" ,status from V_COLLECTION t {whereCluase}" +
                    $" group by t.access_channel,to_char(createdon,'yyyy/mm/dd'),t.access_channel_name,status ";
            }

            DataTable masterDataTable;
            masterDataTable = db.GetData(strSql, parameters);

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;

            var results = new List<CollectionRpt>();
            foreach (DataRow row in masterDataTable.Rows)
            {
                var obj = new CollectionRpt();
                obj.Amount = row["amt"] == DBNull.Value ? 0 : double.Parse(row["amt"].ToString());
                obj.Count = row["cnt"] == DBNull.Value ? 0 : int.Parse(row["cnt"].ToString());
                obj.DistinctCount = row["dist_cnt"] == DBNull.Value ? 0 : int.Parse(row["dist_cnt"].ToString());
                obj.Channel = row["access_channel_name"] == DBNull.Value ? string.Empty : row["access_channel_name"].ToString();
                var statusId  = row["status"] == DBNull.Value ? string.Empty : row["status"].ToString();
                obj.Status = new CommonCodeRepo(db).GetCodesById(statusId, "Collection.Status");
                if (param.LevelId == "pos")
                {
                    obj.Partner.Id = row["pos_id"] == DBNull.Value ? string.Empty : row["pos_id"].ToString();
                    obj.Partner.Name = row["pos_name"] == DBNull.Value ? string.Empty : row["pos_name"].ToString();
                } else if (param.LevelId == "day")
                {
                    obj.CollDay = row["d"] == DBNull.Value ? string.Empty : row["d"].ToString();
                }
                results.Add(obj);
            }
            return results;
        }
        public async Task<List<CollectionRpt>> GetStatReportAsync(CollectionRptQueryParam param)
        {
            #region Parameters
            param.Title = "تقرير احصائي يوضح اجمالي التحصيلات";
            var parameters = new List<OracleParameter>();
            var whereCluase = new StringBuilder();
            if (param != null)
            {
                if (!string.IsNullOrEmpty(param.PosId))
                {
                    whereCluase.Append(" WHERE pos_id = :PosId");
                    var p = new OracleParameter { ParameterName = "PosId", OracleDbType = OracleDbType.Varchar2, Value = param.PosId };
                    parameters.Add(p);
                    param.Title += $"للرقم {param.PosId} ";
                }
                if (!string.IsNullOrEmpty(param.SubsNo))
                {
                    whereCluase.Append(whereCluase.Length > 0 ? " AND subs_no = :SubsNo" : " WHERE subs_no = :SubsNo");
                    var p = new OracleParameter { ParameterName = "SubsNo", OracleDbType = OracleDbType.Varchar2, Value = param.SubsNo };
                    parameters.Add(p);
                    param.Title += $"للمشترك {param.SubsNo} ";
                }
                if (!string.IsNullOrEmpty(param.ChannelId) && param.ChannelId != "-1")
                {
                    whereCluase.Append(whereCluase.Length > 0 ? " AND access_channel = :ChannelId" : " WHERE access_channel = :ChannelId");
                    var p = new OracleParameter { ParameterName = "ChannelId", OracleDbType = OracleDbType.Varchar2, Value = param.ChannelId };
                    parameters.Add(p);
                }
                if (!string.IsNullOrEmpty(param.StatusId) && param.StatusId != "-1")
                {
                    whereCluase.Append(whereCluase.Length > 0 ? " AND status = :StatusId" : " WHERE status = :StatusId");
                    var p = new OracleParameter { ParameterName = "StatusId", OracleDbType = OracleDbType.Varchar2, Value = param.StatusId };
                    parameters.Add(p);
                }
                if (param.StartDate > DateTime.MinValue && param.StartDate != null)
                {
                    whereCluase.Append(whereCluase.Length > 0 ? " AND createdon >= :StartDate" : " WHERE createdon >= :StartDate");
                    var p = new OracleParameter { ParameterName = "StartDate", OracleDbType = OracleDbType.Date, Value = param.StartDate };
                    parameters.Add(p);
                }
                if (param.EndDate > DateTime.MinValue && param.EndDate != null)
                {
                    whereCluase.Append(whereCluase.Length > 0 ? " AND createdon <= :EndDate" : " WHERE createdon <= :EndDate");
                    var p = new OracleParameter { ParameterName = "EndDate", OracleDbType = OracleDbType.Date, Value = param.EndDate.AddDays(1) };
                    parameters.Add(p);
                }
            }

            #endregion

            string strSql = string.Empty;
            if (param.LevelId == "pos")
            {
                param.Title += $"\n للفترة من {param.StartDate.ToShortDateString()} الى {param.EndDate.ToShortDateString()}  " + "على مستوى نقطة البيع";
                strSql = $"select  t.access_channel,t.access_channel_name, t.pos_id, t.pos_name,status, count(*) cnt, " +
                    $"sum(t.amount) amt,count(distinct t.pos_id) dist_cnt   from V_COLLECTION t {whereCluase}" +
                    $" group by t.access_channel, t.pos_id, t.pos_name,t.access_channel_name,status ";
            }
            else
            {
                param.Title += $"\n للفترة من {param.StartDate.ToShortDateString()} الى {param.EndDate.ToShortDateString()}  " + "على مستوى اليوم";
                strSql = $"select  t.access_channel,t.access_channel_name, to_char(createdon,'yyyy/mm/dd') d, count(*) cnt, sum(t.amount) amt,count(distinct t.pos_id) dist_cnt " +
                    $" ,status from V_COLLECTION t {whereCluase}" +
                    $" group by t.access_channel,to_char(createdon,'yyyy/mm/dd'),t.access_channel_name,status ";
            }

            DataTable masterDataTable;
            masterDataTable = await db.GetDataAsync(strSql, parameters);

            if (masterDataTable == null) return null;
            if (masterDataTable.Rows.Count == 0) return null;

            var results = new List<CollectionRpt>();
            foreach (DataRow row in masterDataTable.Rows)
            {
                var obj = new CollectionRpt();
                obj.Amount = row["amt"] == DBNull.Value ? 0 : double.Parse(row["amt"].ToString());
                obj.Count = row["cnt"] == DBNull.Value ? 0 : int.Parse(row["cnt"].ToString());
                obj.DistinctCount = row["dist_cnt"] == DBNull.Value ? 0 : int.Parse(row["dist_cnt"].ToString());
                obj.Channel = row["access_channel_name"] == DBNull.Value ? string.Empty : row["access_channel_name"].ToString();
                var statusId = row["status"] == DBNull.Value ? string.Empty : row["status"].ToString();
                obj.Status = new CommonCodeRepo(db).GetCodesById(statusId, "Collection.Status");
                if (param.LevelId == "pos")
                {
                    obj.Partner.Id = row["pos_id"] == DBNull.Value ? string.Empty : row["pos_id"].ToString();
                    obj.Partner.Name = row["pos_name"] == DBNull.Value ? string.Empty : row["pos_name"].ToString();
                }
                else if (param.LevelId == "day")
                {
                    obj.CollDay = row["d"] == DBNull.Value ? string.Empty : row["d"].ToString();
                }
                results.Add(obj);
            }
            return results;
        }
    }
}
