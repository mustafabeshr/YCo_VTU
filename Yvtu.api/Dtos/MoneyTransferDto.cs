﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yvtu.api.Dtos
{
    public class MoneyTransferDto
    {
        public string pid { get; set; }
        public int amt { get; set; }
        public int seq { get; set; }
        public string note { get; set; }
    }
}
