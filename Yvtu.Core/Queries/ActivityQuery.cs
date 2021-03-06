﻿using System;
using System.Collections.Generic;
using System.Text;
using Yvtu.Core.Entities;

namespace Yvtu.Core.Queries
{
    public class ActivityQuery
    {
        public ActivityQuery()
        {
            Results = new List<Activity>();
        }
        public string QName { get; set; }

        public List<Activity> Results { get; set; }
    }
}
