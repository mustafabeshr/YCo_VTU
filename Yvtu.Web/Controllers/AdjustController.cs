using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NToastNotify;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data;
using Yvtu.Infra.Data.Interfaces;
using Yvtu.Web.Dto;

namespace Yvtu.Web.Controllers
{
    [Authorize]
    public class AdjustController : Controller
    {
        private readonly IAppDbContext db;
        private readonly IPartnerManager partnerManager;
        private readonly IPartnerActivityRepo partnerActivity;
        private readonly IToastNotification toastNotification;

        public AdjustController(IAppDbContext db
            , IPartnerManager partnerManager
            , IPartnerActivityRepo partnerActivity
            , IToastNotification toastNotification)
        {
            this.db = db;
            this.partnerManager = partnerManager;
            this.partnerActivity = partnerActivity;
            this.toastNotification = toastNotification;
        }
        public IActionResult Index()
        {
            var currRoleId = partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = partnerActivity.GetPartAct("MoneyTransfer.Adjustment.Query", currRoleId);
            if (permission == null || permission.Details == null)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية ");
                return Redirect(Request.Headers["Referer"].ToString());
            }
            var model = new AdjustmentQueryDto();
            model.StartDate = DateTime.Today.AddMonths(-1);
            model.EndDate = DateTime.Today;
            model.Paging.PageNo = 1;
            model.Paging.PageSize = 10;
            return View(model);
        }
        [HttpPost]
        public IActionResult Index(AdjustmentQueryDto model, [FromQuery(Name = "direction")] string direction)
        {
            var currRoleId = partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = partnerActivity.GetPartAct("MoneyTransfer.Adjustment.Query", currRoleId);
            if (permission == null || permission.Details == null)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية ");
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

            var results = new AdjustmentRepo(db, partnerManager, partnerActivity).GetListWithPaging(model.Id, model.MoneyTransferId, model.CreatedById,
                model.PartnerId, model.StartDate, model.EndDate, model.Paging);
            if (results != null)
            {
                model.Paging.Count = new AdjustmentRepo(db, partnerManager, partnerActivity).GetCount(model.Id, model.MoneyTransferId, model.CreatedById,
                model.PartnerId, model.StartDate, model.EndDate);
            }
            else
            {
                model.Paging.Count = 0;
            }
            model.Results = results;
            return View(model);
        }

        [HttpGet]
        public IActionResult Create(int mt)
        {
            var currentRoleId = partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = partnerActivity.GetPartAct("MoneyTransfer.Adjustment", currentRoleId);
            if (permission == null)
            {
                return Redirect(Request.Headers["Referer"].ToString());
            }
           
            if (permission.Details == null )
            {
                return Redirect(Request.Headers["Referer"].ToString());
            }
            if (permission.Details.Count == 0)
            {
                return Redirect(Request.Headers["Referer"].ToString());
            }
            var moneyTransfer = new MoneyTransferRepo(db, partnerManager, partnerActivity).GetSingleOrDefault(mt);
            if (moneyTransfer == null)
            {
                return Redirect(Request.Headers["Referer"].ToString());
            }
            if (moneyTransfer.Adjusted || moneyTransfer.AdjustmentNo > 0)
            {
                return Redirect(Request.Headers["Referer"].ToString());
            }
            if (!permission.Details.Any(x => x.ToRole.Id == moneyTransfer.CreatedBy.Role.Id))
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية ");
                return Redirect(Request.Headers["Referer"].ToString());
            }
            var model = new CreateAdjustmentDto();
            model.OriginTrans = moneyTransfer;
            model.TaxPercent = moneyTransfer.TaxPercent;
            model.BonusPercent = moneyTransfer.BonusPercent;
            model.BounsTaxPercent = moneyTransfer.BounsTaxPercent;
            return View(model);
        }
        [HttpPost]
        public IActionResult Create(CreateAdjustmentDto model)
        {
            if (ModelState.IsValid)
            {
                var adj = new Adjustment();
                adj.CreatedBy.Id = partnerManager.GetCurrentUserId(this.HttpContext);
                adj.CreatedBy.Account = partnerManager.GetCurrentUserAccount(this.HttpContext);
                adj.MoneyTransferId = model.OriginTrans.Id;
                adj.Amount = model.Amount;
                adj.AccessChannel.Id = "web";
                adj.Note = model.Note;
                var result = new AdjustmentRepo(db, partnerManager, partnerActivity).Create(adj);
                if (result.Success)
                {
                    return RedirectToAction("Index", "Home");
                }else
                {
                    switch (result.AffectedCount)
                    {
                        case -505:
                            model.Error = "هناك خطأ في احدى القيم ، مما ادى الى فشل اجراء التسوية";
                            break;
                        case -500:
                            model.Error = "هناك احدى البيانات المهمة لم تعد متوفرة";
                            break;
                        case -509:
                            model.Error = "تم عمل تسوية في وقت سابق";
                            break;
                        case -510:
                            model.Error = "مبلغ التسوية غير صحيح سوف يجعل الحركة السابقة غير صحيحة";
                            break;
                        case -507:
                            model.Error = "بيانات الارصدة لم تعد محدثة";
                            break;
                        case -508:
                            model.Error = "فات الاوان على اجراء تسوية لهذه العملية";
                            break;
                        case -501:
                            model.Error = "الرصيد غير كافي";
                            break;
                        default:
                            model.Error = "لم يتم اجراء التسوية بنجاح" + result.AffectedCount;
                            break;
                    }
                    
                }
            }
            return View(model);
        }

        public IActionResult Detail(int id)
        {
            var model = new AdjustmentRepo(db, partnerManager, partnerActivity).GetSingleOrDefault(id);
            return View(model);
        }
    }
}
