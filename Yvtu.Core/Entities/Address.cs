using System;
using System.Collections.Generic;
using System.Text;

namespace Yvtu.Core.Entities
{
    public class Address
    {
        public City City { get; set; }
        public District District { get; set; }
        public string Street { get; set; }
        public string Zone { get; set; }
        public string ExtraInfo { get; set; }
    }
}
