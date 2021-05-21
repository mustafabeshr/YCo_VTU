using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data;
using Yvtu.Infra.Data.Interfaces;
using Yvtu.Web.Dto;

namespace Yvtu.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SettingsController : Controller
    {
        private readonly IAppDbContext _db;
        private readonly IPartnerActivityRepo _partActRepo;
        private readonly IPartnerManager _partnerManager;
        private readonly IDataAuditRepo _auditing;
        private readonly IToastNotification _toastNotification;

        public SettingsController(IAppDbContext db, IPartnerActivityRepo partActRepo
            , IPartnerManager partnerManager, IDataAuditRepo auditing, IToastNotification toastNotification)
        {
            _db = db;
            _partActRepo = partActRepo;
            _partnerManager = partnerManager;
            _auditing = auditing;
            _toastNotification = toastNotification;
        }
        [HttpGet]
        public IActionResult PValues()
        {
            var currentRoleId = _partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = _partActRepo.GetPartAct("PayemntValues.View", currentRoleId);
            if (permission == null)
            {
                _toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
                return Redirect(Request.Headers["Referer"].ToString());
            }

            var model = new PaymentValuesRepo(_db, _partnerManager).GetAll();

            return View(model);
        }
        [HttpGet]
        public IActionResult CreatePVs()
        {
            var currentRoleId = _partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = _partActRepo.GetPartAct("PayemntValues.Create", currentRoleId);
            if (permission == null)
            {
                _toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
                return Redirect(Request.Headers["Referer"].ToString());
            }

            var model = new PaymentValuesDto();

            return View(model);
        }

        [HttpPost]
        public IActionResult CreatePVs(PaymentValuesDto model)
        {
            if (ModelState.IsValid)
            {
                if (model.PayValue <= 0)
                {
                    _toastNotification.AddErrorToastMessage("المبلغ غير صحيح");
                    return View();
                }
                if (model.ProfileId <= 0)
                {
                    _toastNotification.AddErrorToastMessage("رقم المعرف غير صحيح");
                    return View();
                }
                var created = new PaymentValues();
                created.PayValue = model.PayValue;
                created.ProfileId = model.ProfileId;
                created.CreatedBy.Id = _partnerManager.GetCurrentUserId(httpContext: this.HttpContext);
                created.CreatedBy.Account = _partnerManager.GetCurrentUserAccount(httpContext: this.HttpContext);
                var result = new PaymentValuesRepo(_db, _partnerManager).Create(created);
                if (!result.Success)
                {
                    if (result.AffectedCount == -504)
                    {
                        _toastNotification.AddErrorToastMessage($"المبلغ {model.PayValue.ToString("N2")} موجود مسبقا");
                        return Redirect(Request.Headers["Referer"].ToString());
                    }
                }

                PValues();
                return View("PValues");
            }
            else
            {
                return View();
            }
        }

        public IActionResult RemovePV(double pvalue)
        {
            var currentRoleId = _partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = _partActRepo.GetPartAct("PayemntValues.Remove", currentRoleId);
            if (permission == null)
            {
                _toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
                return Redirect(Request.Headers["Referer"].ToString());
            }

            var removedObj = new PaymentValuesRepo(_db, _partnerManager).GetSingleOrDefault(pvalue);
            if (removedObj == null)
            {
                _toastNotification.AddErrorToastMessage($"المبلغ {pvalue.ToString("N2")} غير موجود");
                return View("PValues");
            }
            else
            {
                var result = new PaymentValuesRepo(_db, _partnerManager).Remove(pvalue);
                if (result.Success)
                {
                    _toastNotification.AddSuccessToastMessage($"المبلغ {pvalue.ToString("N2")} تم حذفه");
                    var audit = new DataAudit();
                    audit.Activity.Id = "PayemntValues.Remove";
                    audit.PartnerId = _partnerManager.GetCurrentUserId(this.HttpContext);
                    audit.Action.Id = "Delete";
                    audit.Success = true;
                    audit.OldValue = removedObj.ToString();
                    audit.NewValue = string.Empty;
                    _auditing.Create(audit);
                }
                PValues();
                return View("PValues");
            }
        }

        [HttpGet]
        public IActionResult ApiIPsBlacklist()
        {
            var currentRoleId = _partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = _partActRepo.GetPartAct("API.IpBlacklist.View", currentRoleId);
            if (permission == null)
            {
                _toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
                return Redirect(Request.Headers["Referer"].ToString());
            }

            var model = new ApiIPBlacklistRepo(_db).GetList(string.Empty);

            return View(model);
        }

        public IActionResult ApiBlackListRemove(string ipAddress)
        {
            var currentRoleId = _partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = _partActRepo.GetPartAct("API.IpBlacklist.Remove", currentRoleId);
            if (permission == null)
            {
                _toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
                return Redirect(Request.Headers["Referer"].ToString());
            }

            var removedObj = new ApiIPBlacklistRepo(_db).GetSingleOrDefault(ipAddress);
            if (removedObj == null)
            {
                _toastNotification.AddErrorToastMessage($"العوان {ipAddress} غير موجود");
                return View("ApiIPsBlacklist");
            }
            else
            {
                var result = new ApiIPBlacklistRepo(_db).Remove(ipAddress);
                if (result.Success)
                {
                    _toastNotification.AddSuccessToastMessage($"العنوان {ipAddress} تم حذفه");
                    var audit = new DataAudit();
                    audit.Activity.Id = "API.IpBlacklist.Remove";
                    audit.PartnerId = _partnerManager.GetCurrentUserId(this.HttpContext);
                    audit.Action.Id = "Delete";
                    audit.Success = true;
                    audit.OldValue = removedObj.ToString();
                    audit.NewValue = string.Empty;
                    _auditing.Create(audit);
                }
                var model = new ApiIPBlacklistRepo(_db).GetList(string.Empty);
                return View("ApiIPsBlacklist", model);
            }
        }
    }
}
