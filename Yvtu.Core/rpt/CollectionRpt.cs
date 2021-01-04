using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Yvtu.Core.Entities;

namespace Yvtu.Core.rpt
{
    public class CollectionRpt
    {
        public CollectionRpt()
        {
            Partner = new AppUser();
        }
        public string Channel { get; set; }
        public string CollDay { get; set; }
        public AppUser Partner { get; set; }
        public CommonCode Status { get; set; }
        public int Count { get; set; }
        public double Amount { get; set; }
        public int DistinctCount { get; set; }
    }
    public class CollectionRptQueryParam
    {
        [StringLength(9)]
        [RegularExpression(@"^70\d*", ErrorMessage = "رقم موبايل غير صحيح")]
        public string PosId { get; set; }
        [StringLength(9)]
        [RegularExpression(@"^70\d*", ErrorMessage = "رقم موبايل غير صحيح")]
        public string SubsNo { get; set; }
        public string ChannelId { get; set; }
        public string StatusId { get; set; }
        public string LevelId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Title { get; set; }
    }
    public class CollectionRptQuery
    {
        public CollectionRptQuery()
        {
            Param = new CollectionRptQueryParam();
        }
        
        public CollectionRptQueryParam Param { get; set; }
        public List<CommonCode> Channels { get; set; }
        public List<CommonCode> Statuses { get; set; }
        public List<CollectionRpt> Results { get; set; }
    }

}
