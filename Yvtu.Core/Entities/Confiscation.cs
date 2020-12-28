using System;
using System.Collections.Generic;
using System.Text;

namespace Yvtu.Core.Entities
{
    public class Confiscation
    {
        public Confiscation()
        {
            CreatedBy = new AppUser();
            Partner = new Partner();
        }
        public int Id { get; set; }
        public AppUser CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public Partner Partner { get; set; }
        public double Amount { get; set; }
        public string Note { get; set; }
    }
}

