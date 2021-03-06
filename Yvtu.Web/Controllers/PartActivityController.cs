﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using NToastNotify;
using System;
using System.Collections.Generic;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data;
using Yvtu.Infra.Data.Interfaces;
using Yvtu.Web.Dto;

namespace Yvtu.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PartActivityController : Controller
    {
        private readonly IAppDbContext db;
        private readonly IPartnerActivityRepo _partActRepo;
        private readonly IDataAuditRepo _auditing;
        private readonly IToastNotification toastNotification;
        private readonly ILogger<PartActivityController> _logger;

        public IPartnerManager _PartnerManager { get; }

        public PartActivityController(IAppDbContext db, IPartnerActivityRepo partActRepo
            , IPartnerManager partnerManager, IDataAuditRepo auditing
            , IToastNotification toastNotification,
            ILogger<PartActivityController> logger)
        {
            this.db = db;
            _partActRepo = partActRepo;
            _PartnerManager = partnerManager;
            _auditing = auditing;
            this.toastNotification = toastNotification;
            _logger = logger;
        }
        [HttpGet]
        public IActionResult Index()
        {

            var currentRoleId = _PartnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = _partActRepo.GetPartAct("PartnerActivity.Query", currentRoleId);
            if (permission == null)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                {
                    Title = ""
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }


            var model = new ListPartnerActivityDto();

            model.Activities = new SelectList(new ActivityRepo(db, _PartnerManager).GetActivities(), "Id", "Name");
            model.FromRoles = new SelectList(new RoleRepo(db, _partActRepo).GetRoles(), "Id", "Name");

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(ListPartnerActivityDto model)
        {

            var currentRoleId = _PartnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = _partActRepo.GetPartAct("PartnerActivity.Query", currentRoleId);
            if (permission == null)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                {
                    Title = ""
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }

            if (model == null)
            {
                model = new ListPartnerActivityDto();
            }

            model.Activities = new SelectList(new ActivityRepo(db, _PartnerManager).GetActivities(), "Id", "Name");
            model.FromRoles = new SelectList(new RoleRepo(db, _partActRepo).GetRoles(), "Id", "Name");

            if (model != null && string.IsNullOrEmpty(model.ActivityId) && model.FromRoleId == 0)
            {
                var result = _partActRepo.GetAllList();
                model.PartnerActivities = result;
            }
            else
            {
                if (model != null && !string.IsNullOrEmpty(model.ActivityId) && model.FromRoleId == 0)
                {
                    var result = _partActRepo.GetListByActivity(model.ActivityId);
                    model.PartnerActivities = result;
                }
                else if (model != null && string.IsNullOrEmpty(model.ActivityId) && model.FromRoleId > 0)
                {
                    var result = _partActRepo.GetListByFrom(model.FromRoleId);
                    model.PartnerActivities = result;
                }
                else
                {
                    var result = _partActRepo.GetListByActivityWithFromRole(model.ActivityId, model.FromRoleId);
                    model.PartnerActivities = result;
                }
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var currentRoleId = _PartnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = _partActRepo.GetPartAct("PartnerActivity.Edit", currentRoleId);
            if (permission == null)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                {
                    Title = ""
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }
            var model = new CreatePartnerActivity2Dto();
            var result = _partActRepo.GetPartAct(id);
            if (result != null)
            {
                model.Activity = result.Activity;
                model.ActivityId = result.Activity.Id;
                model.FromRole = result.FromRole;
                model.FromRoleId = result.FromRole.Id;
                model.MaxQueryDurationId = result.MaxQueryDuration.Id;
                model.MaxQueryRowsNo = result.MaxQueryRows;
                model.ScopeId = result.Scope.Id;
                model.OnlyPartnerChildren = result.OnlyPartnerChildren;

            }
            var fromRoles = new RoleRepo(db, _partActRepo).GetRoles();
            var activities = new ActivityRepo(db, _PartnerManager).GetActivities();
            var maxQueryDuration = new CommonCodeRepo(db).GetCodesByType("queryduration");
            var scopes = new CommonCodeRepo(db).GetCodesByType("activity.scope");

            model.FromRoles = fromRoles;
            model.Activities = activities;
            model.MaxQueryDuration = maxQueryDuration;
            model.Scopes = scopes;
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(CreatePartnerActivity2Dto model)
        {
            if (ModelState.IsValid)
            {
                var currentRoleId = _PartnerManager.GetCurrentUserRole(this.HttpContext);
                var permission = _partActRepo.GetPartAct("PartnerActivity.Edit", currentRoleId);
                if (permission == null)
                {
                    toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                    {
                        Title = ""
                    });
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                var old = _partActRepo.GetPartAct(model.Id);
                if (old == null) return View(model);

                var pAct = new PartnerActivity();
                pAct.Id = model.Id;
                pAct.Activity.Id = model.ActivityId;
                pAct.FromRole.Id = model.FromRoleId ?? 0;
                pAct.MaxQueryRows = model.MaxQueryRowsNo;
                pAct.MaxQueryDuration.Id = model.MaxQueryDurationId;
                pAct.Scope.Id = model.ScopeId;
                pAct.OnlyPartnerChildren = model.OnlyPartnerChildren;
                pAct.LastEditOn = DateTime.Now;
                var result = _partActRepo.Edit(pAct);
                if (result.Success)
                {
                    var audit = new DataAudit();
                    audit.Activity.Id = "PartnerActivity.Edit";
                    audit.PartnerId = _PartnerManager.GetCurrentUserId(this.HttpContext);
                    audit.PartnerAccount = _PartnerManager.GetCurrentUserAccount(this.HttpContext);
                    audit.Action.Id = "Update";
                    audit.Success = true;
                    audit.OldValue = old.ToString();
                    audit.NewValue = pAct.ToString();
                    _auditing.Create(audit);
                    return RedirectToAction("Index");
                }
                else
                {
                    model.Error = result.Error;
                }
            }
            var fromRoles = new RoleRepo(db, _partActRepo).GetRoles();
            var activities = new ActivityRepo(db, _PartnerManager).GetActivities();
            var maxQueryDuration = new CommonCodeRepo(db).GetCodesByType("queryduration");
            var scopes = new CommonCodeRepo(db).GetCodesByType("activity.scope");

            model.FromRoles = fromRoles;
            model.Activities = activities;
            model.MaxQueryDuration = maxQueryDuration;
            model.Scopes = scopes;
            return View(model);
        }
        public IActionResult Delete(int id)
        {
            var currentRoleId = _PartnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = _partActRepo.GetPartAct("PartnerActivity.Delete", currentRoleId);
            if (permission == null)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                {
                    Title = ""
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }

            var old = _partActRepo.GetPartAct(id);
            if (old != null)
            {
                var audit = new DataAudit();
                audit.Activity.Id = "PartnerActivity.Delete";
                audit.PartnerId = _PartnerManager.GetCurrentUserId(this.HttpContext);
                audit.PartnerAccount = _PartnerManager.GetCurrentUserAccount(this.HttpContext);
                audit.Action.Id = "Delete";
                audit.Success = true;
                audit.OldValue = old.ToString();
                _auditing.Create(audit);
                _partActRepo.Delete(id);
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Create()
        {
            var model = new CreatePartnerActivityDto();
            var fromRoles = new RoleRepo(db, _partActRepo).GetRoles();
            var toRoles = new RoleRepo(db, _partActRepo).GetRoles();
            var activities = new ActivityRepo(db, _PartnerManager).GetActivities();
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
        public IActionResult CreateRule()
        {

            var currentRoleId = _PartnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = _partActRepo.GetPartAct("PartnerActivity.Create", currentRoleId);
            if (permission == null)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                {
                    Title = ""
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }

            var model = new CreatePartnerActivity2Dto();
            var detailModel = new List<CreatePartnerActivityDetailDto>();
            var fromRoles = new RoleRepo(db, _partActRepo).GetRoles();
            var toRoles = new RoleRepo(db, _partActRepo).GetRoles();
            var activities = new ActivityRepo(db, _PartnerManager).GetActivities();
            var maxQueryDuration = new CommonCodeRepo(db).GetCodesByType("queryduration");
            var scopes = new CommonCodeRepo(db).GetCodesByType("activity.scope");

            model.FromRoles = fromRoles;
            //detailModel.ToRoles = toRoles;
            model.Activities = activities;
            model.MaxQueryDuration = maxQueryDuration;
            model.Scopes = scopes;
            model.Details = detailModel;

            ViewBag.Details = new CreatePartnerActivityDetailDto
            {
                ToRoles = new RoleRepo(db, _partActRepo).GetRoles()
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult CreateRule(CreatePartnerActivity2Dto model)
        {
            if (ModelState.IsValid)
            {
                var currentRoleId = _PartnerManager.GetCurrentUserRole(this.HttpContext);
                var permission = _partActRepo.GetPartAct("PartnerActivity.Create", currentRoleId);
                if (permission == null)
                {
                    toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                    {
                        Title = ""
                    });
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                var pAct = new PartnerActivity();
                pAct.Activity.Id = model.ActivityId;
                pAct.FromRole.Id = model.FromRoleId ?? 0;
                pAct.MaxQueryRows = model.MaxQueryRowsNo;
                pAct.MaxQueryDuration.Id = model.MaxQueryDurationId;
                pAct.Scope.Id = model.ScopeId;
                pAct.OnlyPartnerChildren = model.OnlyPartnerChildren;
                pAct.CreatedBy.Id = _PartnerManager.GetCurrentUserId(this.HttpContext);
                pAct.CreatedBy.Account = _PartnerManager.GetCurrentUserAccount(this.HttpContext);

                var result = _partActRepo.Create(pAct);
                if (result.Success)
                {
                    var listModel = new ListPartnerActivityDto();
                    listModel.Activities = new SelectList(new ActivityRepo(db, _PartnerManager).GetActivities(), "Id", "Name");
                    listModel.FromRoles = new SelectList(new RoleRepo(db, _partActRepo).GetRoles(), "Id", "Name");
                    //return View("Index", listModel);
                    return RedirectToAction("Detail", "PartActivity", new { id = result.AffectedCount });
                }
                else if (result.AffectedCount == -504)
                {
                    var permission2 = _partActRepo.GetPartAct("PartnerActivity.Query", currentRoleId);
                    if (permission2 == null)
                    {
                        toastNotification.AddErrorToastMessage("تم انشاء هذه الصلاحية مسبقا ", new ToastrOptions {Title = ""});
                    } else
                    {
                       return RedirectToAction("Detail", "PartActivity", new { id = permission2.Id });
                    }
                } else
                {
                    toastNotification.AddErrorToastMessage("لم يتم انشاء الصلاحية ، يرجى المحاولة لاحقا ", new ToastrOptions { Title = "" });
                }
            }

            var fromRoles = new RoleRepo(db, _partActRepo).GetRoles();
            var activities = new ActivityRepo(db, _PartnerManager).GetActivities();
            var maxQueryDuration = new CommonCodeRepo(db).GetCodesByType("queryduration");
            var scopes = new CommonCodeRepo(db).GetCodesByType("activity.scope");

            model.FromRoles = fromRoles;
            model.Activities = activities;
            model.MaxQueryDuration = maxQueryDuration;
            model.Scopes = scopes;
            return View(model);
        }

        [HttpGet]
        public IActionResult Detail(int id)
        {
            var currentRoleId = _PartnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = _partActRepo.GetPartAct("PartnerActivity.Query", currentRoleId);
            if (permission == null)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                {
                    Title = ""
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }

            var result = _partActRepo.GetPartAct(id);

            return View(result);
        }

        [HttpGet]
        public IActionResult AddDetail(int id)
        {
            var currentRoleId = _PartnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = _partActRepo.GetPartAct("PartnerActivity.Detail.Create", currentRoleId);
            if (permission == null)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                {
                    Title = ""
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }
            var masterModel = _partActRepo.GetPartAct(id);
            if (masterModel == null) return null;
            var toRoles = new RoleRepo(db, _partActRepo).GetRoles();
            var model = new CreatePartnerActivityDetailDto
            {
                ParentId = masterModel.Id,
                ActivityId = masterModel.Activity.Id,
                ActivityName = masterModel.Activity.Name,
                FromRoleId = masterModel.FromRole.Id,
                FromRoleName = masterModel.FromRole.Name,
                ToRoles = toRoles
            };

            return View(model);
        }
        [HttpGet]
        public IActionResult MoreDetail(int id, int parentId)
        {

            var currentRoleId = _PartnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = _partActRepo.GetPartAct("PartnerActivity.Detail.Query", currentRoleId);
            if (permission == null)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                {
                    Title = ""
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }

            var model = _partActRepo.GetDetail(id, parentId, true);
            if (model == null) return null;
            return View(model);
        }
        [HttpPost]
        public IActionResult AddDetail(CreatePartnerActivityDetailDto model)
        {
            if (ModelState.IsValid)
            {

                var currentRoleId = _PartnerManager.GetCurrentUserRole(this.HttpContext);
                var permission = _partActRepo.GetPartAct("PartnerActivity.Detail.Create", currentRoleId);
                if (permission == null)
                {
                    toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                    {
                        Title = ""
                    });
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                var masterModel = _partActRepo.GetPartAct(model.ParentId);
                if (masterModel == null) return null;
                var originObject = new PartnerActivityDetail();
                originObject.ParentId = model.ParentId;
                originObject.ToRole.Id = model.ToRoleId;
                originObject.CheckBalanceRequired = model.CheckBalanceRequired;
                originObject.MinValue = model.MinValue;
                originObject.MaxValue = model.MaxValue;
                originObject.TaxPercent = model.TaxPercent;
                originObject.BonusPercent = model.BonusPercent;
                originObject.BonusTaxPercent = model.BonusTaxPercent;
                originObject.FixedFactor = model.FixedFactor;
                originObject.CreatedBy.Id = _PartnerManager.GetCurrentUserId(this.HttpContext);
                var result = _partActRepo.CreateDetail(originObject);
                if (result.Success)
                {

                    return RedirectToAction("Detail", new { id = model.ParentId });
                }

            }
            model.ToRoles = new RoleRepo(db, _partActRepo).GetRoles();
            return View(model);
        }
        [HttpGet]
        public IActionResult EditDetail(int id, int parentId)
        {
            var currentRoleId = _PartnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = _partActRepo.GetPartAct("PartnerActivity.Detail.Edit", currentRoleId);
            if (permission == null)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                {
                    Title = ""
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }

            var model = _partActRepo.GetDetail(id, parentId, true);
            if (model == null) return null;
            var toRoles = new RoleRepo(db, _partActRepo).GetRoles();

            var viewModel = new CreatePartnerActivityDetailDto
            {
                ParentId = model.Parent.Id,
                ActivityId = model.Parent.Activity.Id,
                ActivityName = model.Parent.Activity.Name,
                ToRoleId = model.ToRole.Id,
                ToRole = model.ToRole,
                CheckBalanceRequired = model.CheckBalanceRequired,
                MinValue = model.MinValue,
                MaxValue = model.MaxValue,
                TaxPercent = model.TaxPercent,
                BonusPercent = model.BonusPercent,
                BonusTaxPercent = model.BonusTaxPercent,
                FixedFactor = model.FixedFactor,
                ToRoles = toRoles
            };
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult EditDetail(CreatePartnerActivityDetailDto model)
        {
            if (ModelState.IsValid)
            {
                var currentRoleId = _PartnerManager.GetCurrentUserRole(this.HttpContext);
                var permission = _partActRepo.GetPartAct("PartnerActivity.Detail.Edit", currentRoleId);
                if (permission == null)
                {
                    toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                    {
                        Title = ""
                    });
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                var old = _partActRepo.GetDetail(model.Id, model.ParentId);
                if (old == null) return View(model);


                var originObject = new PartnerActivityDetail();
                originObject.Id = model.Id;
                originObject.ParentId = model.ParentId;
                originObject.ToRole.Id = model.ToRoleId;
                originObject.CheckBalanceRequired = model.CheckBalanceRequired;
                originObject.MinValue = model.MinValue;
                originObject.MaxValue = model.MaxValue;
                originObject.TaxPercent = model.TaxPercent;
                originObject.BonusPercent = model.BonusPercent;
                originObject.BonusTaxPercent = model.BonusTaxPercent;
                originObject.FixedFactor = model.FixedFactor;
                originObject.CreatedBy.Id = _PartnerManager.GetCurrentUserId(this.HttpContext);
                var result = _partActRepo.UpdateDetail(originObject);
                if (result.Success)
                {
                    var audit = new DataAudit();
                    audit.Activity.Id = "PartnerActivity.Detail.Edit";
                    audit.PartnerId = _PartnerManager.GetCurrentUserId(this.HttpContext);
                    audit.PartnerAccount = _PartnerManager.GetCurrentUserAccount(this.HttpContext);
                    audit.Action.Id = "Update";
                    audit.Success = true;
                    audit.OldValue = old.ToString();
                    audit.NewValue = originObject.ToString();
                    var auditResult = _auditing.Create(audit);
                    if (!auditResult.Success)
                    {
                        _logger.LogError($"Edit Partner Activity Detail no[{originObject.Id}] , Error was {auditResult.Error}");
                    }
                }
                return RedirectToAction("Detail", new { id = model.ParentId });
            }
            model.ToRoles = new RoleRepo(db, _partActRepo).GetRoles();
            return View(model);
        }
        public IActionResult DeleteDetail(int id, int parentId)
        {
            var currentRoleId = _PartnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = _partActRepo.GetPartAct("PartnerActivity.Detail.Delete", currentRoleId);
            if (permission == null)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                {
                    Title = ""
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }

            var old = _partActRepo.GetDetail(id, parentId);
            if (old != null)
            {
                var audit = new DataAudit();
                audit.Activity.Id = "PartnerActivity.Detail";
                audit.PartnerId = _PartnerManager.GetCurrentUserId(this.HttpContext);
                audit.PartnerAccount = _PartnerManager.GetCurrentUserAccount(this.HttpContext);
                audit.Action.Id = "Delete";
                audit.Success = true;
                audit.OldValue = old.ToString();
                _auditing.Create(audit);
                _partActRepo.DeleteDetail(id, parentId);
            }
            return RedirectToAction("Detail", new { id = parentId });
        }


        public IActionResult AuditQuery()
        {
            var currentRoleId = _PartnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = _partActRepo.GetPartAct("DataAudit.Query", currentRoleId);
            if (permission == null)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                {
                    Title = ""
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }

            var model = new DataAuditQueryDto();
            model.StartDate = DateTime.Today.Subtract(TimeSpan.FromDays(10));
            model.EndDate = DateTime.Today.AddDays(1);
            model.Activities = new ActivityRepo(db, _PartnerManager).GetDataAuditActivities();
            return View(model);
        }
        [HttpPost]
        public IActionResult AuditQuery(DataAuditQueryDto model)
        {
            var currentRoleId = _PartnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = _partActRepo.GetPartAct("DataAudit.Query", currentRoleId);
            if (permission == null)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                {
                    Title = ""
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }

            var results = _auditing.GetAuditig(new IDataAuditRepo.GetListParam
            {
                ActivityId = model.ActivityId,
                CreatorId = model.CreatedById,
                CreatorAccount = model.CreatedByAccount,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                IncludeDates = model.IncludeDates
            });
            model.Results = results;
            model.Activities = new ActivityRepo(db, _PartnerManager).GetDataAuditActivities();
            return View(model);
        }

    }
}
