using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Yvtu.Core.Entities;

namespace Yvtu.Core.rpt
{
    public class MoneyTransferRpt
    {
        public MoneyTransferRpt()
        {
            Partner = new AppUser();
        }
        public string Channel { get; set; }
        public string CollDay { get; set; }
        public AppUser Partner { get; set; }
        public int Count { get; set; }
        public double Amount { get; set; }
    }
    public class MoneyTransferRptQueryParam
    {
        [StringLength(9)]
        [RegularExpression(@"^70\d*", ErrorMessage = "رقم موبايل غير صحيح")]
        public string PosId { get; set; }
        public string ChannelId { get; set; }
        public string LevelId { get; set; }
        public string TransTypeId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Title { get; set; }
    }
    public class MoneyTransferRptQuery
    {
        public MoneyTransferRptQuery()
        {
            Param = new MoneyTransferRptQueryParam();
        }
        
        public MoneyTransferRptQueryParam Param { get; set; }
        public List<CommonCode> Channels { get; set; }
        public List<CommonCode> TransType { get; set; }
        public List<MoneyTransferRpt> Results { get; set; }
    }

}
