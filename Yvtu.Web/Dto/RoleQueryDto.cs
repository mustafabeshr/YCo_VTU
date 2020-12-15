using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yvtu.Web.Dto
{
    public class RoleQueryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
        public int Weight { get; set; }
        public byte Order { get; set; }
        public int PartnerCount { get; set; }
    }
}
