using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Yvtu.Core.Entities;

namespace Yvtu.Core.Queries
{
    public class RechargeQuery
    {
        [StringLength(9)]
        public string QPosId { get; set; }
        [StringLength(9)]
        public string QSubsId { get; set; }
        public int QPosAccount { get; set; }
        [DataType(DataType.Date)]
        public DateTime QFromDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime QToDate { get; set; }
        public int? StatusId { get; set; }
        public  List<CommonCode> Statuses { get; set; }
        public string AccessChannelId { get; set; }
        public List<CommonCode> AccessChannel { get; set; }
        public string Error { get; set; }
        public string QueryScope { get; set; }
        public string CurrentUserId { get; set; }
        public int CurrentUserAccount { get; set; }
        public Paging Paging { get; set; }
        public RechargeQuery()
        {
            Statuses = new List<CommonCode>();
            AccessChannel = new List<CommonCode>();
            Paging = new Paging();
        }

        public virtual List<RechargeQueryResult> Results { get; set; }
    }

    public class RechargeQueryResult
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public string SubsNo { get; set; }
        public double Amount { get; set; }
        public string POSId { get; set; }
        public string POSName { get; set; }
        public int POSAccount { get; set; }
        public double POSBalance { get; set; }
        public int POSRoleId { get; set; }
        public string POSRoleName { get; set; }
        public string AccessChannelId { get; set; }
        public string AccessChannelName { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public DateTime StatusOn { get; set; }
        public int QueueNo { get; set; }
        public string RefNo { get; set; }
        public string RefMessage { get; set; }
        public string RefTransNo { get; set; }
        public DateTime RefTime { get; set; }
        public string DebugInfo { get; set; }
        public long ApiTransaction { get; set; }

    }
}
