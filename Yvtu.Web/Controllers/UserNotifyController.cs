using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data;
using Yvtu.Infra.Data.Interfaces;
using Yvtu.Web.Dto;

namespace Yvtu.Web.Controllers
{
    [Authorize]
    public class UserNotifyController : Controller
    {
        private readonly IAppDbContext db;
        private readonly IPartnerManager partnerManager;
        private readonly IPartnerActivityRepo partnerActivity;
        private readonly IToastNotification toastNotification;

        public UserNotifyController(IAppDbContext db, IPartnerManager partnerManager,
            IPartnerActivityRepo partnerActivity, IToastNotification toastNotification)
        {
            this.db = db;
            this.partnerManager = partnerManager;
            this.partnerActivity = partnerActivity;
            this.toastNotification = toastNotification;
        }
        public IActionResult Index()
        {
            var currentRoleId = partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = partnerActivity.GetPartAct("UserNotify.Query", currentRoleId);
            if (permission == null)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                {
                    Title = "تنبيه"
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }
            var model = new UserNotifyQueryDto();
            model.Statuses = new CommonCodeRepo(db).GetCodesByType("UserInstructStatus");
            model.StartDate = DateTime.Today.AddMonths(-1);
            model.EndDate = DateTime.Today.AddDays(1);
            model.Paging.PageNo = 1;
            model.Paging.PageSize = 10;
            model.Paging.Count = 0;
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(UserNotifyQueryDto model, [FromQuery(Name = "direction")] string direction)
        {
            ModelState.Clear();
            if (direction == "pre" && model.Paging.PageNo > 1)
            {
                model.Paging.PageNo -= 1;
            }
            if (direction == "next")
            {
                model.Paging.PageNo += 1;
            }
            var result = new UserNotifyRepo(db).QueryWithPaging(model.Id, model.Content, model.StatusId, model.StartDate, model.EndDate, model.Paging);
            if (result != null)
            {
                model.Paging.Count = new UserNotifyRepo(db).GetCount(model.Id, model.Content, model.StatusId, model.StartDate, model.EndDate);
            }
            else
            {
                model.Paging.Count = 0;
            }
            model.Statuses = new CommonCodeRepo(db).GetCodesByType("UserInstructStatus");
            model.Results = result;
            return View(model);
        }
        [HttpGet]
        public IActionResult Create()
        {
            var currentRoleId = partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = partnerActivity.GetPartAct("UserNotify.Create", currentRoleId);
            if (permission == null)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                {
                    Title = "تنبيه"
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }
            var model = new UserNotifyDto();
            model.Priorities = new CommonCodeRepo(db).GetCodesByType("priority");
            model.Roles = new RoleRepo(db, partnerActivity).GetRoles();
            var expiredAfter = Convert.ToInt32(new AppGlobalSettingsRepo(db).GetSingle("UserInstructExpireDays").SettingValue);
            model.ExpireOn = DateTime.Today.AddDays(expiredAfter);
            return View(model);
        }
        [HttpPost]
        public IActionResult Create(UserNotifyDto model)
        {
            var currentRoleId = partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = partnerActivity.GetPartAct("UserNotify.Create", currentRoleId);
            if (permission == null)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                {
                    Title = "تنبيه"
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }
            if (model.Id > 0)
            {
                toastNotification.AddErrorToastMessage("تم الحفظ مسبقا", new ToastrOptions
                {
                    Title = "تنبيه"
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }
            if (ModelState.IsValid)
            {
                var created = new UserNotify();
                created.Content = model.Content;
                created.Subject = model.Subject;
                created.ExpireOn = model.ExpireOn;
                created.Priority.Id = model.PriorityId;
                created.CreatedBy.Id = partnerManager.GetCurrentUserId(this.HttpContext);
                created.CreatedBy.Account = partnerManager.GetCurrentUserAccount(this.HttpContext);
                if (!string.IsNullOrEmpty(model.SelectedRoles))
                {
                    var roles = model.SelectedRoles.Split(',');
                    foreach (string item in roles)
                    {
                        var notifyTo = new UserNotifyTo();
                        notifyTo.Role.Id = int.Parse(item);
                        created.NotifyToList.Add(notifyTo);
                    }
                }
                var result = new UserNotifyRepo(db).Create(created);
                if (result.AffectedCount > 0)
                {
                    toastNotification.AddSuccessToastMessage("تم اضافة التعميم بنجاح", new ToastrOptions
                    {
                        Title = "تنبيه"
                    });
                    ModelState.Clear();
                    model.Id = result.AffectedCount;
                }
            }
            model.Priorities = new CommonCodeRepo(db).GetCodesByType("priority");
            model.Roles = new RoleRepo(db, partnerActivity).GetRoles();
            return View(model);
        }
        public string delete(int id)
        {
            var result = new UserNotifyRepo(db).Delete(id);
            if (result > 0)
            {
                return "suceess";
            }
            return "failed";
        }
        public string PostNotify(int id)
        {
            var currentRoleId = partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = partnerActivity.GetPartAct("UserNotify.Post", currentRoleId);
            if (permission == null)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                {
                    Title = "تنبيه"
                });
                //return Redirect(Request.Headers["Referer"].ToString());
            }else
            {
                var result = new UserNotifyRepo(db).Post(id);
                if (result.Success)
                {
                    toastNotification.AddSuccessToastMessage("تم ترحيل التعميم على كافة المستخدمين", new ToastrOptions
                    {
                        Title = "تنبيه"
                    });
                    return "suceess";
                }
            }
            return "failed";
        }
        public IActionResult Detail(int id)
        {
            var model = new UserNotifyHistoryRepo(db).GetSingle(id);
            var currentUserId = partnerManager.GetCurrentUserId(this.HttpContext);
            if (model.Partner.Id == currentUserId)
            {
                var markRead = new UserNotifyHistoryRepo(db).Read(id);
            }
            return View(model);
        }


        public IActionResult MyMsg()
        {
            var currentRoleId = partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = partnerActivity.GetPartAct("UserNotifyHis.Query", currentRoleId);
            if (permission == null)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                {
                    Title = "تنبيه"
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }
            var model = new UserNotifyHisQueryDto();
            model.Statuses = new CommonCodeRepo(db).GetCodesByType("UserInstructHisStatus");
            model.StartDate = DateTime.Today.AddMonths(-1);
            model.EndDate = DateTime.Today.AddDays(1);
            model.Paging.PageNo = 1;
            model.Paging.PageSize = 10;
            model.Paging.Count = 0;
            model.PartnerId = partnerManager.GetCurrentUserId(this.HttpContext);
            return View(model);
        }

        [HttpPost]
        public IActionResult MyMsg(UserNotifyHisQueryDto model, [FromQuery(Name = "direction")] string direction)
        {
            var currentRoleId = partnerManager.GetCurrentUserRole(this.HttpContext);
            var currUserId = partnerManager.GetCurrentUserId(this.HttpContext);
            var permission = partnerActivity.GetPartAct("UserNotifyHis.Query", currentRoleId);
            if (permission == null)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                {
                    Title = "تنبيه"
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }
            Partner targetPartner = null;
            if (!string.IsNullOrEmpty(model.PartnerId) && model.PartnerId != currUserId)
            {
                var validateTargetPartnerResult = partnerManager.Validate(model.PartnerId);
                targetPartner = validateTargetPartnerResult.Success ? validateTargetPartnerResult.Partner : null;
                if (targetPartner == null)
                {
                    toastNotification.AddErrorToastMessage("يرجى التأكد من الرقم المراد الاستعلام عنه", new ToastrOptions
                    {
                        Title = "تنبيه"
                    });
                    return Redirect(Request.Headers["Referer"].ToString());
                }
            }
            if (permission.Scope.Id == "CurOpOnly" && model.PartnerId != currUserId)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية للاستعلام عن هذا الرقم", new ToastrOptions
                {
                    Title = "تنبيه"
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }
            else if (permission.Scope.Id == "Exclusive" && targetPartner != null && targetPartner.RefPartner.Id != currUserId)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية للاستعلام عن هذا الرقم", new ToastrOptions
                {
                    Title = "تنبيه"
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }
            ModelState.Clear();
            if (direction == "pre" && model.Paging.PageNo > 1)
            {
                model.Paging.PageNo -= 1;
            }
            if (direction == "next")
            {
                model.Paging.PageNo += 1;
            }
            var result = new UserNotifyHistoryRepo(db).QueryWithPaging(model.PartnerId, model.Content, model.StatusId, model.StartDate, model.EndDate, model.Paging);
            if (result != null)
            {
                model.Paging.Count = new UserNotifyHistoryRepo(db).GetCount(model.PartnerId, model.Content, model.StatusId, model.StartDate, model.EndDate);
            }
            else
            {
                model.Paging.Count = 0;
            }
            model.Statuses = new CommonCodeRepo(db).GetCodesByType("UserInstructHisStatus");
            model.Results = result;
            return View(model);
        }
    }
}
