using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Yvtu.Core.Entities;

namespace Yvtu.Web.Dto
{
    public class AdjustmentQueryDto
    {

        public AdjustmentQueryDto()
        {
            Paging = new Paging();
        }
        public int Id { get; set; }
        public int MoneyTransferId { get; set; }
        [StringLength(9)]
        public string CreatedById { get; set; }
        [StringLength(9)]
        public string PartnerId { get; set; }
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        public Paging Paging { get; set; }
        public List<Adjustment> Results { get; set; }
    }
}
