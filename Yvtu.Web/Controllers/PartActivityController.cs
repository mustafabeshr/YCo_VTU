using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data;
using Yvtu.Infra.Data.Interfaces;
using Yvtu.Web.Dto;

namespace Yvtu.Web.Controllers
{
    public class PartActivityController : Controller
    {
        private readonly IAppDbContext db;
        private readonly IPartnerActivityRepo _partActRepo;

        public IPartnerManager _PartnerManager { get; }

        public PartActivityController(IAppDbContext db, IPartnerActivityRepo partActRepo, IPartnerManager partnerManager)
        {
            this.db = db;
            _partActRepo = partActRepo;
            _PartnerManager = partnerManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Create()
        {
            var model = new CreatePartnerActivityDto();
            var fromRoles = new RoleRepo(db).GetRoles();
            var toRoles = new RoleRepo(db).GetRoles();
            var activities = new ActivityRepo(db).GetActivities();
            var maxQueryDuration = new CommonCodeRepo(db).GetCodesByType("queryduration");
            var scopes = new CommonCodeRepo(db).GetCodesByType("activity.scope");

            model.FromRoles = fromRoles;
            model.ToRoles = toRoles;
            model.Activities = activities;
            model.MaxQueryDuration = maxQueryDuration;
            model.Scopes = scopes;

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreatePartnerActivityDto model)
        {

            if (ModelState.IsValid)
            {
                var pAct = new PartnerActivity();
                pAct.Activity.Id = model.ActivityId;
                pAct.FromRole.Id = model.FromRoleId ?? 0;
                pAct.ToRole.Id = model.ToRoleId ?? 0 ;
                pAct.CheckBalanceRequired = model.CheckBalanceRequired;
                pAct.MaxValue = model.MaxValue;
                pAct.MinValue = model.MinValue;
                pAct.BonusPercent = model.BonusPercent;
                pAct.TaxPercent = model.TaxPercent;
                pAct.BonusTaxPercent = model.BonusTaxPercent;
                pAct.MaxQueryRows = model.MaxQueryRowsNo;
                pAct.MaxQueryDuration = model.MaxQueryDurationId;
                pAct.Scope = model.ScopeId;
                pAct.OnlyPartnerChildren = model.OnlyPartnerChildren;
                pAct.CreatedBy.Id = _PartnerManager.GetCurrentUserId(this.HttpContext);

                var result = await _partActRepo.CreateAsync(pAct);
            }

            var fromRoles = new RoleRepo(db).GetRoles();
            var toRoles = new RoleRepo(db).GetRoles();
            var activities = new ActivityRepo(db).GetActivities();
            var maxQueryDuration = new CommonCodeRepo(db).GetCodesByType("queryduration");
            var scopes = new CommonCodeRepo(db).GetCodesByType("activity.scope");

            model.FromRoles = fromRoles;
            model.ToRoles = toRoles;
            model.Activities = activities;
            model.MaxQueryDuration = maxQueryDuration;
            model.Scopes = scopes;
            return View(model);
        }
    }
}
