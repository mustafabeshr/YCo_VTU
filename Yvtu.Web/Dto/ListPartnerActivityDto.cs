using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yvtu.Core.Entities;

namespace Yvtu.Web.Dto
{
    public class ListPartnerActivityDto
    {
        public SelectList Activities { get; set; }
        public string ActivityId { get; set; }
        public SelectList FromRoles { get; set; }
        public int FromRoleId { get; set; }

        public SelectList ToRoles { get; set; }
        public int ToRoleId { get; set; }

        public List<PartnerActivity> PartnerActivities { get; set; }
    }
}
