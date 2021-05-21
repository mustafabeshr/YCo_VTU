using System;
using System.Collections.Generic;
using System.Text;

namespace Yvtu.Core.Entities
{
    public class ApiIPBlacklist
    {
        public string IPAddress { get; set; }
        public DateTime CreatedOn { get; set; }

        public override string ToString()
        {
            return
                $"IPAddress={IPAddress}\n CreatedOn={CreatedOn.ToString("dd/MM/yyyy HH:mm:ss")}";
        }
    }
}
