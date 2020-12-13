using System;
using System.Collections.Generic;
using System.Text;

namespace Yvtu.RechargePrc.Dtos
{
    public class RechargeResponseDto
    {
        public int ResultCode { get; set; }
        public string ResultDesc { get; set; }
        public int Duration { get; set; }
        public string TransNo { get; set; }
    }
}
