using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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

        public AdjustController(IAppDbContext db
            , IPartnerManager partnerManager
            , IPartnerActivityRepo partnerActivity)
        {
            this.db = db;
            this.partnerManager = partnerManager;
            this.partnerActivity = partnerActivity;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create(int mt)
        {
            var currentRoleId = partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = partnerActivity.GetPartAct("Adjustment", currentRoleId);
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
                var result = new AdjustmentRepo(db, partnerManager).Create(adj);
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

    }
}
