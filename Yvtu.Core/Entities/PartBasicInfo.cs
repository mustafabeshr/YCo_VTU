﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Yvtu.Core.Entities
{
    public class PartBasicInfo
    {
        public string Id { get; set; }
        public int Account { get; set; }
        public string Name { get; set; }
        public Role Role { get; set; }
        public long Balance { get; set; }
        public string Error { get; set; }
    }
}
