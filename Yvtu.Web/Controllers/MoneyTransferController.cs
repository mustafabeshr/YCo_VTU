using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data.Interfaces;
using Yvtu.Web.Dto;

namespace Yvtu.Web.Controllers
{
    [Authorize]
    public class MoneyTransferController : Controller
    {
        private readonly IAppDbContext _db;
        private readonly IPartnerManager _partnerManager;
        private readonly IPartnerActivityRepo _partnerActivity;

        public MoneyTransferController(IAppDbContext db, IPartnerManager partnerManager
            , IPartnerActivityRepo partnerActivity)
        {
            this._db = db;
            this._partnerManager = partnerManager;
            this._partnerActivity = partnerActivity;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new CreateMoneyTransferDto();
            var currentPartId = _partnerManager.GetCurrentUserId(this.HttpContext);
            var currentPart = _partnerManager.Validate(currentPartId).Partner;
            model.CreatorBalance = currentPart.Balance - currentPart.Reserved;
            return View(model);
        }

        public CreateMoneyTransferDto GetBasicInfo(string pId)
        {
            if (pId.Length != 9) return new CreateMoneyTransferDto { Error = "رقم خاطئ" };
            var validateResult =  _partnerManager.Validate(pId);
            if (validateResult.Success)
            {
                var partner = validateResult.Partner;
                var roleId = User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.GivenName).Value;
                var moneyTransferSettings = _partnerActivity.GetPartAct("Money.Transfer", roleId, partner.Role.Id.ToString());
                if (moneyTransferSettings == null) return new CreateMoneyTransferDto { Error = "لم يتم تعريف هذا الاجراء او ليس لديك الصلاحية الكافية" };
                return new CreateMoneyTransferDto
                {
                    PartnerId = partner.Id,
                    PartnerName = partner.Name,
                    PartnerRoleName = partner.Role.Name,
                    PartnerBalance = partner.Balance,
                    TaxPercent = moneyTransferSettings.TaxPercent,
                    BonusPercent = moneyTransferSettings.BonusPercent,
                    BounsTaxPercent = moneyTransferSettings.BonusTaxPercent,
                    Error = "N/A"
                };
            }
            else
            {
                return new CreateMoneyTransferDto { Error = "غير موجود" };
            }
            

        }
    }
}
