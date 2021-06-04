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
using Yvtu.Web.Dto;

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
            var currentRole = partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = partnerActivity.GetPartAct("Notification.Query", currentRole);
            if (permission == null)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
                return Redirect(Request.Headers["Referer"].ToString());
            }
            else
            {
                var model = new SMSOutBackDto();
                model.StartDate = DateTime.Today.AddMonths(-1);
                model.EndDate = DateTime.Today;
                return View(model);
            }
        }
        [HttpPost]
        public IActionResult Index(SMSOutBackDto model)
        {
            var partner = partnerManager.GetPartnerBasicInfo(model.Receiver);
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
                
                    model.Results = new SMSOutBackRepo(db, partnerManager).GetList(new SMSOutBackRepo.GetListParam
                    {
                        Message = model.Message,
                        Receiver = model.Receiver,
                        IncludeDates = model.IncludeDates,
                        StartDate = model.StartDate,
                        EndDate = model.EndDate
                    });
            }

            return View(model);
        }
    }
}
