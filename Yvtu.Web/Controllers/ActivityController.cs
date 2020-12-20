using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yvtu.Core.Queries;
using Yvtu.Infra.Data;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.Web.Controllers
{
    [Authorize]
    public class ActivityController : Controller
    {
        private readonly IAppDbContext db;
        private readonly IPartnerManager partnerManager;

        public ActivityController(IAppDbContext db, IPartnerManager partnerManager)
        {
            this.db = db;
            this.partnerManager = partnerManager;
        }
        public IActionResult Index()
        {
            var activities = new ActivityRepo(db).GetActivities(false);
            var model = new ActivityQuery();
            model.Results = activities;
            return View(model);
        }
        [HttpPost]
        public IActionResult Index(ActivityQuery model)
        {
            var activities = new ActivityRepo(db).GetActivities(model.QName);
            model.Results = activities;
            return View(model);
        }
    }
}
