using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Yvtu.Core.Entities;

namespace Yvtu.Core.Queries
{
    public class MoneyTransferQueryDto
    {
        public MoneyTransferQueryDto()
        {
            Paging = new Paging();
        }
        public int QId { get; set; }
        [StringLength(9)]
        public string QPartnerId { get; set; }
        public int QPartnerAccount { get; set; }
        [DataType(DataType.Date)]
        public DateTime QFromDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime QToDate { get; set; }
        public string QListTypeId { get; set; }
        public int QShortItems { get; set; }
        public Paging  Paging { get; set; }
        public string Error { get; set; }
        public string QScope { get; set; }
        public string QueryUser { get; set; }

        public virtual List<MoneyTransferDetailQueryDto> Results { get; set; }
    }
    public class MoneyTransferDetailQueryDto
    {
        public int Seq { get; set; }
        public int Id { get; set; }
        public string PartnerId { get; set; }
        public string PartnerName { get; set; }
        public int PartnerRoleId { get; set; }
        public string PartnerRoleName { get; set; }
        public int PartnerAccount { get; set; }
        public double PartnerBalance { get; set; }
        public string PayTypeId { get; set; }
        public string PayTypeName { get; set; }
        public string PayNo { get; set; }
        public DateTime PayDate { get; set; }
        public string PayBank { get; set; }
        public string CreatorBy { get; set; }
        public string CreatorByName { get; set; }
        public int CreatorByRoleId { get; set; }
        public string CreatorByRoleName { get; set; }
        public int CreatorAccount { get; set; }
        public double CreatorBalance { get; set; }
        public DateTime CreatedOn { get; set; }
        public string AccessChannelId { get; set; }
        public string AccessChannelName { get; set; }
        public double Amount { get; set; }
        public double TaxPercent { get; set; }
        public double TaxAmount { get; set; }
        public double BonusPercent { get; set; }
        public double BounsAmount { get; set; }
        public double BounsTaxPercent { get; set; }
        public double BounsTaxAmount { get; set; }
        public double ReceivedAmount { get; set; }
        public double NetAmount { get; set; }
        public double RequestAmount { get; set; }
        public string BillNo { get; set; }
        public string RequestNo { get; set; }
        public string Note { get; set; }
        public bool Adjusted { get; set; }
        public int AdjustmentNo { get; set; }
    }
}
