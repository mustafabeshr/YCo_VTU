using System;
using System.Collections.Generic;
using System.Text;

namespace Yvtu.Core.Entities
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte Order { get; set; }

        public virtual ICollection<District> Districts { get; set; }
    }
}
