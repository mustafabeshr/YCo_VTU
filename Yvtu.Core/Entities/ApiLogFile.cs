using System;
using System.Collections.Generic;
using System.Text;

namespace Yvtu.Core.Entities
{
    public class ApiLogFile
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Data { get; set; }
        public string User { get; set; }
        public string Ip { get; set; }
        public int Level { get; set; }
        public string Action { get; set; }
    }

}
