using System;

namespace Yvtu.api.Dtos
{
    public class MoneyTransferDto
    {
        public string pid { get; set; }
        public int amt { get; set; }
        public long seq { get; set; }
        public string note { get; set; }
    }

    public class MoneyTransferMyReportDto
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
