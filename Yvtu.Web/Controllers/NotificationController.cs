using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using Yvtu.Core.Entities;
using Yvtu.Core.Queries;
using Yvtu.Infra.Data;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.Web.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly IAppDbContext db;
        private readonly IPartnerManager partnerManager;
        private readonly IPartnerActivityRepo partnerActivity;
        private readonly IToastNotification toastNotification;

        public NotificationController(IAppDbContext db, IPartnerManager partnerManager
            , IPartnerActivityRepo partnerActivity, IToastNotification toastNotification)
        {
            this.db = db;
            this.partnerManager = partnerManager;
            this.partnerActivity = partnerActivity;
            this.toastNotification = toastNotification;
        }

        public IActionResult Index()
        {
            var model = new NotificationQuery();
            return View(model);
        }
        [HttpPost]
        public IActionResult Index(NotificationQuery model)
        {
            var partner = partnerManager.GetPartnerBasicInfo(model.QPartnerId);
            var currentRole = partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = partnerActivity.GetPartAct("Notification.Query", currentRole);
            if (permission == null)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
            }
            else if (permission.Details == null || permission.Details.Count == 0)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
            }
            else if (permission.Scope.Id == "CurOpOnly")
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
            }
            else if (permission.Scope.Id == "Exclusive" && partner.RefPartnerId != partnerManager.GetCurrentUserId(this.HttpContext))
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
            }
            else
            { 
                if (!string.IsNullOrEmpty(model.QPartnerId))
                {
                    model.Results = new NotificationRepo(db, partnerManager).GetList(model.QPartnerId);
                }
            }

            return View(model);
        }
    }
}
