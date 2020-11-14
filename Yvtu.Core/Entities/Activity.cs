using System;
using System.Collections.Generic;
using System.Text;

namespace Yvtu.Core.Entities
{
   public  class Activity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Order { get; set; }
        public bool Internal { get; set; }

    }
}
