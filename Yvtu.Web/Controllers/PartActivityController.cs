using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data;
using Yvtu.Infra.Data.Interfaces;
using Yvtu.Web.Dto;

namespace Yvtu.Web.Controllers
{
    [Authorize]
    public class PartActivityController : Controller
    {
        private readonly IAppDbContext db;
        private readonly IPartnerActivityRepo _partActRepo;
        private readonly IDataAuditRepo _auditing;

        public IPartnerManager _PartnerManager { get; }

        public PartActivityController(IAppDbContext db, IPartnerActivityRepo partActRepo
            , IPartnerManager partnerManager, IDataAuditRepo auditing)
        {
            this.db = db;
            _partActRepo = partActRepo;
            _PartnerManager = partnerManager;
            _auditing = auditing;
        }
        [HttpGet]
        public IActionResult Index()
        {
          
             var  model = new ListPartnerActivityDto();
            

            model.Activities =  new SelectList(new ActivityRepo(db).GetActivities(), "Id", "Name");
            model.FromRoles =  new SelectList(new RoleRepo(db).GetRoles(), "Id", "Name");
            model.ToRoles =  new SelectList(new RoleRepo(db).GetRoles(), "Id", "Name");

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(ListPartnerActivityDto model)
        {
            if (model == null)
            {
                model = new ListPartnerActivityDto();
            }

            model.Activities = new SelectList(new ActivityRepo(db).GetActivities(), "Id", "Name");
            model.FromRoles = new SelectList(new RoleRepo(db).GetRoles(), "Id", "Name");
            model.ToRoles = new SelectList(new RoleRepo(db).GetRoles(), "Id", "Name");

            if (model != null && string.IsNullOrEmpty(model.ActivityId) && model.FromRoleId == 0 && model.ToRoleId == 0)
            {
                var result = _partActRepo.GetAllList();
                model.PartnerActivities = result;
            }
            else
            {
                if (model != null && !string.IsNullOrEmpty(model.ActivityId))
                {
                    var result = _partActRepo.GetListByActivity(model.ActivityId);
                    model.PartnerActivities = result;
                }
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var model = new CreatePartnerActivityDto();
            var result = _partActRepo.GetPartAct(id);
            if (result != null)
            {
                model.Activity = result.Activity;
                model.ActivityId = result.Activity.Id;
                model.FromRole = result.FromRole;
                model.FromRoleId = result.FromRole.Id;
                model.ToRole = result.ToRole;
                model.ToRoleId = result.ToRole.Id;
                model.CheckBalanceRequired = result.CheckBalanceRequired;
                model.MaxValue = result.MaxValue;
                model.MinValue = result.MinValue;
                model.TaxPercent = result.TaxPercent;
                model.BonusPercent = result.BonusPercent;
                model.BonusTaxPercent = result.BonusTaxPercent;
                model.MaxQueryDurationId = result.MaxQueryDuration.Id;
                model.MaxQueryRowsNo = result.MaxQueryRows;
                model.ScopeId = result.Scope.Id;
                model.OnlyPartnerChildren = result.OnlyPartnerChildren;

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

        [HttpPost]
        public async Task<IActionResult> Edit(CreatePartnerActivityDto model)
        {
            if (ModelState.IsValid)
            {
                var old = _partActRepo.GetPartAct(model.Id);
                if (old == null) return View(model);

                var pAct = new PartnerActivity();
                pAct.Id = model.Id;
                pAct.Activity.Id = model.ActivityId;
                pAct.FromRole.Id = model.FromRoleId ?? 0;
                pAct.ToRole.Id = model.ToRoleId ?? 0;
                pAct.CheckBalanceRequired = model.CheckBalanceRequired;
                pAct.MaxValue = model.MaxValue;
                pAct.MinValue = model.MinValue;
                pAct.BonusPercent = model.BonusPercent;
                pAct.TaxPercent = model.TaxPercent;
                pAct.BonusTaxPercent = model.BonusTaxPercent;
                pAct.MaxQueryRows = model.MaxQueryRowsNo;
                pAct.MaxQueryDuration.Id = model.MaxQueryDurationId;
                pAct.Scope.Id = model.ScopeId;
                pAct.OnlyPartnerChildren = model.OnlyPartnerChildren;
                pAct.LastEditOn = DateTime.Now;
                var result = await _partActRepo.EditAsync(pAct);
                if (result.Success)
                {
                    var audit = new DataAudit();
                    audit.Activity.Id = "PartnerActivity";
                    audit.PartnerId = _PartnerManager.GetCurrentUserId(this.HttpContext);
                    audit.Action.Id = "Update";
                    audit.Success = true;
                    audit.OldValue = old.ToString();
                    audit.NewValue = pAct.ToString();
                    await _auditing.CreateAsync(audit);
                    return RedirectToAction("Index");
                }
                else
                {
                    model.Error = result.Error;
                }
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
        public async Task<IActionResult> Delete(int id)
        {
            var old = _partActRepo.GetPartAct(id);
            if (old != null)
            {
                var audit = new DataAudit();
                audit.Activity.Id = "PartnerActivity";
                audit.PartnerId = _PartnerManager.GetCurrentUserId(this.HttpContext);
                audit.Action.Id = "Delete";
                audit.Success = true;
                audit.OldValue = old.ToString();
                await _auditing.CreateAsync(audit);
                _partActRepo.Delete(id);
            }
            return RedirectToAction("Index");
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
                pAct.MaxQueryDuration.Id = model.MaxQueryDurationId;
                pAct.Scope.Id = model.ScopeId;
                pAct.OnlyPartnerChildren = model.OnlyPartnerChildren;
                pAct.CreatedBy.Id = _PartnerManager.GetCurrentUserId(this.HttpContext);

                var result = await _partActRepo.CreateAsync(pAct);
                if (result.Success)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    model.Error = result.Error;
                }
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

        [HttpGet]
        public IActionResult Detail(int id)
        {
            var result =  _partActRepo.GetPartAct(id);

            return View(result);
        }
    }
}
