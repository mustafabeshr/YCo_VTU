﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using Yvtu.Core.Entities;

namespace Yvtu.Web.Dto
{
    public class ListPartnerActivityDto
    {
        public SelectList Activities { get; set; }
        public string ActivityId { get; set; }
        public SelectList FromRoles { get; set; }
        public int FromRoleId { get; set; }
        public string Error { get; set; }
        public List<PartnerActivity> PartnerActivities { get; set; }
    }
}
