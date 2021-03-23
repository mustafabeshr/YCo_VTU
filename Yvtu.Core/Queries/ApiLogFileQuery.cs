using System;
using System.Collections.Generic;
using System.Text;
using Yvtu.Core.Entities;

namespace Yvtu.Core.Queries
{
    public class ApiLogFileQuery
    {
        public ApiLogFileQuery()
        {
            Paging = new Paging();
        }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string Data { get; set; }
        public string User { get; set; }
        public string Ip { get; set; }
        public int Level { get; set; }
        public string Action { get; set; }
        public string SortBy { get; set; }
        public string SortType { get; set; }
        public Paging Paging { get; set; }

        public virtual List<ApiLogFile> Results { get; set; }
    }
}
