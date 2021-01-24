using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using Yvtu.Core.Queries;
using Yvtu.Infra.Data;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.Web.Controllers
{
    public class RechargeController : Controller
    {
        private readonly IAppDbContext db;
        private readonly IPartnerManager partner;
        private readonly IPartnerActivityRepo partnerActivity;
        private readonly IToastNotification toastNotification;

        public RechargeController(IAppDbContext db
            , IPartnerManager partner
            , IPartnerActivityRepo partnerActivity
            , IToastNotification toastNotification)
        {
            this.db = db;
            this.partner = partner;
            this.partnerActivity = partnerActivity;
            this.toastNotification = toastNotification;
        }
        public IActionResult Index()
        {
            var currentRoleId = partner.GetCurrentUserRole(this.HttpContext);
            var permission = partnerActivity.GetPartAct("Recharge.Query", currentRoleId);
            if (permission == null)
            {
                return Redirect(Request.Headers["Referer"].ToString());
            }
            var model = new RechargeQuery();
            model.Statuses = new CommonCodeRepo(db).GetCodesByType("Collection.Status");
            model.AccessChannel = new CommonCodeRepo(db).GetCodesByType("access.channel");
            return View(model);
        }
        [HttpPost]
        public IActionResult Index(RechargeQuery model)
        {
            model.Error = string.Empty;
            
            var currUserId = partner.GetCurrentUserId(this.HttpContext);
            var currRoleId = partner.GetCurrentUserRole(this.HttpContext);
            var currAccountId = partner.GetCurrentUserAccount(this.HttpContext);
            var permission = partnerActivity.GetPartAct("Recharge.Query", currRoleId);
            if (permission == null || permission.Details == null)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية");
                model.Statuses = new CommonCodeRepo(db).GetCodesByType("Collection.Status");
                model.AccessChannel = new CommonCodeRepo(db).GetCodesByType("access.channel");
                return View(model);
            }
            if (permission.Scope.Id == "CurOpOnly")
            {
                if (!string.IsNullOrEmpty(model.QPosId) && model.QPosId != currUserId)
                {
                    toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية للاستعلام عن هذا الرقم");
                    model.Statuses = new CommonCodeRepo(db).GetCodesByType("Collection.Status");
                    model.AccessChannel = new CommonCodeRepo(db).GetCodesByType("access.channel");
                    return View(model);
                }
                else if (model.QPosAccount > 0 && model.QPosAccount != currAccountId)
                {
                    toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية للاستعلام عن هذا الحساب");
                    model.Statuses = new CommonCodeRepo(db).GetCodesByType("Collection.Status");
                    model.AccessChannel = new CommonCodeRepo(db).GetCodesByType("access.channel");
                    return View(model);
                }
            }

            model.QueryScope = permission.Scope.Id;
            model.CurrentUserId = currUserId;
            model.CurrentUserAccount = currAccountId;
            var result = new RechargeQuery();  
            result = new RechargeRepo(db, partner).Query(model);
            result.Statuses = new CommonCodeRepo(db).GetCodesByType("Collection.Status");
            result.AccessChannel = new CommonCodeRepo(db).GetCodesByType("access.channel");
            if (result.Results == null)
            {
                toastNotification.AddInfoToastMessage("عذرا لا توجد بيانات");
            }
            
            return View(result);
            
           
        }

        public IActionResult Detail(int id)
        {
            var model = new RechargeRepo(db, partner).GetRecharge(id);
            return View(model);
        }
    }
}
