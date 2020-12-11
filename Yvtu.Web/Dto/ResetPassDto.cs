using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yvtu.Web.Dto
{
    public class ResetPassDto
    {
        public string Message { get; set; }
        public string PartnerId { get; set; }
        public string PartnerName { get; set; }
        public string Error { get; set; }
        public string Success { get; set; }
    }
}
