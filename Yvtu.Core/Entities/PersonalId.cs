using System;
using System.Collections.Generic;
using System.Text;

namespace Yvtu.Core.Entities
{
    public class PersonalId
    {
        public string Id { get; set; }
        public string Place { get; set; }
        public DateTime Issued { get; set; }

        public virtual IdType IdType { get; set; }
    }
}
